using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Field;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerController : MonoBehaviour
    {
        public readonly ReactiveProperty<Vector3Int> TargetDirection = new();
        public readonly ReactiveProperty<Vector3Int> TargetPosition = new();

        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        private Vector3 _playerPosition;
        private Vector2 _moveDirection;
        private int _currentSpeed;
        private float _runFeeDelay;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private StatsConfig _statsConfig;
        private StatsService _statsService;
        private InputService _inputService;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;

        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            StatsConfig statsConfig,
            StatsService statsService,
            InputService inputService,
            InventoryService inventoryService,
            TilemapService tilemapService)
        {
            _statsConfig = statsConfig;
            _statsService = statsService;
            _inputService = inputService;
            _inventoryService = inventoryService;
            _tilemapService = tilemapService;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            _inventoryService.GetTargetPosition = () => TargetPosition.Value;
            _inventoryService.GetTargetDirection = () => TargetDirection.Value;
            _inventoryService.GetCharacterPosition = () => transform.position;

            transform.SetLocalPositionAndRotation(_statsService.PlayerPosition, Quaternion.identity);

            _currentSpeed = _statsConfig.WalkSpeed;

            SubscribeOnInput();
        }

        private void Update()
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
            _tilemapService.ProcessTiles(TargetPosition.Value);

            if (_currentSpeed == _statsConfig.RunSpeed)
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
                case "Ground":
                    var tilemap = other.GetComponent<Tilemap>();
                    _tilemapService.OnGroundEnter(tilemap);
                    break;
                case "Item":
                    var mapItem = other.GetComponent<MapItemBase>();
                    _inventoryService.TryPut(mapItem, transform);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "Ground":
                    var tilemap = other.GetComponent<Tilemap>();
                    _tilemapService.OnGroundExit(tilemap);
                    break;
            }
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

        private void OnDestroy()
        {
            _statsService.OnSessionEnded(transform.position);
            _compositeDisposable.Dispose();
        }
    }
}