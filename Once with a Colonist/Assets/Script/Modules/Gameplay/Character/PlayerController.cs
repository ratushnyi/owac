using System;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.Gameplay.Services.Player;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerController : MonoBehaviour
    {
        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private List<SpriteRenderer> _spriteRenderers;

        private Vector2 _moveDirection;
        private int _currentSpeed;
        private float _runFeeDelay;

        private PlayerConfig _playerConfig;
        private PlayerService _playerService;
        private StatsService _statsService;
        private InputService _inputService;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;
        private CinemachineVirtualCamera _virtualCamera;

        private IDisposable _useButtonDisposable;
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            PlayerConfig playerConfig,
            PlayerService playerService,
            StatsService statsService,
            InputService inputService,
            InventoryService inventoryService,
            TilemapService tilemapService,
            CinemachineVirtualCamera virtualCamera)
        {
            _playerService = playerService;
            _playerConfig = playerConfig;
            _statsService = statsService;
            _inputService = inputService;
            _inventoryService = inventoryService;
            _tilemapService = tilemapService;
            _virtualCamera = virtualCamera;
        }

        private void Start()
        {
            _virtualCamera.Follow = transform;
            _currentSpeed = _playerConfig.WalkSpeed;

            SubscribeOnInput();
        }

        private void Update()
        {
            UpdateTarget();
            UpdateSpeed();
        }

        private void UpdateTarget()
        {
            if (_playerService.PlayerPosition.Value == transform.position)
            {
                return;
            }

            _playerService.PlayerPosition.Value = transform.position;

            if (_moveDirection != Vector2.zero)
            {
                _playerService.TargetDirection.Value = Vector3Int.RoundToInt(_moveDirection);
            }

            _tilemapService.ProcessTarget(
                new Vector3Int(Mathf.FloorToInt(_playerService.PlayerPosition.Value.x), Mathf.RoundToInt(_playerService.PlayerPosition.Value.y)) + _playerService.TargetDirection.Value);
        }

        private void UpdateSpeed()
        {
            if (_currentSpeed > _playerConfig.WalkSpeed && _rigidbody2D.velocity.magnitude > 0)
            {
                _runFeeDelay -= Time.deltaTime;

                if (_runFeeDelay <= 0)
                {
                    _statsService.ApplyValue(_playerConfig.RunFee.Type, _playerConfig.RunFee.Value);

                    _runFeeDelay = _playerConfig.RunFee.Rate;

                    OnSpeedChanged(true);
                }
            }
        }

        private void SubscribeOnInput()
        {
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

            SubscribeOnUseInput(_inventoryService).AddTo(_compositeDisposable);
        }

        private IDisposable SubscribeOnUseInput(IPerformable performableEntity)
        {
            _useButtonDisposable?.Dispose();
            _useButtonDisposable = _inputService.OnXButtonPerformed
                .Subscribe(_ => performableEntity.Perform());

            return _useButtonDisposable;
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
                case GeneralConstants.DeviceTag:
                    var deviceMapObject = other.GetComponent<DeviceMapObject>();
                    SubscribeOnUseInput(deviceMapObject);
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
                case GeneralConstants.DeviceTag:
                    SubscribeOnUseInput(_inventoryService);
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
            var newSpeedValue = _playerConfig.WalkSpeed;
            if (isRunning && _statsService.IsSuitable(_playerConfig.RunFee.Type, _playerConfig.RunFee.Value))
            {
                newSpeedValue = _playerConfig.RunSpeed;
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
            _rigidbody2D.velocity = _currentSpeed * _moveDirection;
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

        public void ApplyLayer(int layer, int sortingLayer)
        {
            gameObject.layer = layer;
            _playerService.PlayerLayerID.Value = gameObject.layer;
            _playerService.PlayerSortingLayerID.Value = sortingLayer;

            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.sortingLayerID = sortingLayer;
            }

            _playerService.UpdatePlayerMapModel();
        }

        private void OnDestroy()
        {
            _playerService.UpdatePlayerMapModel();
            _compositeDisposable.Dispose();
        }
    }
}