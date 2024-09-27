using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Map;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs.Stats;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private List<SpriteRenderer> _spriteRenderers;

        public readonly ReactiveProperty<Vector3Int> TargetDirection = new();
        public readonly ReactiveProperty<Vector3Int> TargetPosition = new();

        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        private Vector3 _playerPosition;
        private Vector2 _moveDirection;
        private int _currentSpeed;
        private int _soringLayerID;
        private float _runFeeDelay;
        
        private StatsConfig _statsConfig;
        private MapService _mapService;
        private StatsService _statsService;
        private InputService _inputService;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;

        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            StatsConfig statsConfig,
            MapService mapService,
            StatsService statsService,
            InputService inputService,
            InventoryService inventoryService,
            TilemapService tilemapService)
        {
            _statsConfig = statsConfig;
            _mapService = mapService;
            _statsService = statsService;
            _inputService = inputService;
            _inventoryService = inventoryService;
            _tilemapService = tilemapService;
        }

        private void Start()
        {
            _inventoryService.GetTargetPosition = () => TargetPosition.Value;
            _mapService.GetTargetDirection = () => TargetDirection.Value;
            _mapService.GetPlayerTransform = () => transform;
            _mapService.GetPlayerSortingLayerID = () => _soringLayerID;

            if (!_statsService.IsFirstLoad)
            {
                transform.SetLocalPositionAndRotation(_statsService.PlayerPosition, Quaternion.identity);
                ApplyLayer(_statsService.Layer, _statsService.SoringLayerID);
            }
            else
            {
                _soringLayerID = _spriteRenderers[0].sortingLayerID;
            }

            _currentSpeed = _statsConfig.WalkSpeed;

            SubscribeOnInput();
        }

        private void Update()
        {
            UpdateTarget();
            UpdateSpeed();
        }

        private void UpdateTarget()
        {
            if (_playerPosition == transform.position)
            {
                return;
            }

            _playerPosition = transform.position;

            if (_moveDirection != Vector2.zero)
            {
                TargetDirection.Value = Vector3Int.RoundToInt(_moveDirection);
            }

            TargetPosition.Value = new Vector3Int(Mathf.FloorToInt(_playerPosition.x), Mathf.RoundToInt(_playerPosition.y)) + TargetDirection.Value;
            _tilemapService.ProcessTarget(TargetPosition.Value);
        }

        private void UpdateSpeed()
        {
            if (_currentSpeed > _statsConfig.WalkSpeed && _rigidbody2D.velocity.magnitude > 0)
            {
                _runFeeDelay -= Time.deltaTime;

                if (_runFeeDelay <= 0)
                {
                    _statsService.ApplyValue(_statsConfig.RunFee.Type, _statsConfig.RunFee.Value);

                    _runFeeDelay = _statsConfig.RunFee.Rate;

                    OnSpeedChanged(true);
                }
            }
        }

        private void SubscribeOnInput()
        {
            _inputService.OnLeftStickPerformed
                .First()
                .Subscribe(_ => _statsService.OnSessionStarted());

            _inputService.OnLeftStickPerformed
                .Subscribe(t => ProcessMovement(t.ReadValue<Vector2>()))
                .AddTo(_compositeDisposable);

            _inputService.OnLeftStickCanceled
                .Subscribe(_ => ProcessMovement(Vector2.zero))
                .AddTo(_compositeDisposable);

            _inputService.OnBButtonStarted
                .Subscribe(_ => OnSpeedChanged(true))
                .AddTo(_compositeDisposable);

            _inputService.OnBButtonCanceled
                .Subscribe(_ => OnSpeedChanged(false))
                .AddTo(_compositeDisposable);

            _inputService.OnXButtonPerformed
                .Subscribe(_ => _inventoryService.Perform())
                .AddTo(_compositeDisposable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.tag)
            {
                case GeneralConstants.GroundTag:
                    var tilemap = other.GetComponent<Tilemap>();
                    _tilemapService.OnGroundEnter(tilemap);
                    break;
                case GeneralConstants.ItemTag:
                    var mapItem = other.GetComponent<ItemMapObject>();
                    _inventoryService.TryPut(mapItem);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case GeneralConstants.GroundTag:
                    var tilemap = other.GetComponent<Tilemap>();
                    _tilemapService.OnGroundExit(tilemap);
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D _)
        {
            UpdateVelocity();
        }

        private void OnCollisionStay2D(Collision2D _)
        {
            UpdateVelocity();
        }

        private void OnCollisionExit2D(Collision2D _)
        {
            UpdateVelocity();
        }

        private void OnSpeedChanged(bool isRunning)
        {
            var newSpeedValue = _statsConfig.WalkSpeed;
            if (isRunning && _statsService.IsSuitable(_statsConfig.RunFee.Type, _statsConfig.RunFee.Value))
            {
                newSpeedValue = _statsConfig.RunSpeed;
            }

            if (_currentSpeed == newSpeedValue)
            {
                return;
            }

            _currentSpeed = newSpeedValue;
            UpdateVelocity();
        }

        private void ProcessMovement(Vector2 moveDirection)
        {
            _moveDirection = OnMove(moveDirection);
            UpdateVelocity();
            _animator.SetBool(_isMovingAnimatorKey, _moveDirection.magnitude > 0);
        }

        private void UpdateVelocity()
        {
            _rigidbody2D.velocity = _statsService.MovementSpeed * _currentSpeed * _moveDirection;
        }

        private Vector2 OnMove(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                switch (direction.x)
                {
                    case < 0:
                        direction = Vector2.left;
                        _animator.SetInteger(_directionAnimatorKey, 3);
                        break;
                    case > 0:
                        direction = Vector2.right;
                        _animator.SetInteger(_directionAnimatorKey, 2);
                        break;
                }
            }

            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                switch (direction.y)
                {
                    case > 0:
                        direction = Vector2.up;
                        _animator.SetInteger(_directionAnimatorKey, 1);
                        break;
                    case < 0:
                        direction = Vector2.down;
                        _animator.SetInteger(_directionAnimatorKey, 0);
                        break;
                }
            }

            return direction;
        }

        public void ApplyLayer(string layer, string sortingLayer)
        {
            ApplyLayer(LayerMask.NameToLayer(layer), SortingLayer.NameToID(sortingLayer));
        }

        private void ApplyLayer(int layer, int sortingLayer)
        {
            gameObject.layer = layer;
            _soringLayerID = sortingLayer;

            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.sortingLayerID = _soringLayerID;
            }

            _statsService.UpdateStatsProfile(transform.position, _soringLayerID, gameObject.layer);
        }

        private void OnDestroy()
        {
            _statsService.UpdateStatsProfile(transform.position, _soringLayerID, gameObject.layer);
            _compositeDisposable.Dispose();
        }
    }
}