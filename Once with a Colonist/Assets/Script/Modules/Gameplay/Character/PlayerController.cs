using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerController : MonoBehaviour
    {
        public readonly ReactiveProperty<Vector3Int> TargetDirection = new();
        public readonly ReactiveProperty<Vector3Int> TargetPosition = new();

        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        private Vector2 _moveDirection;
        private float _speedModifier = 1;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private InputService _inputService;
        private InventoryService _inventoryService;
        private StatsService _statsService;
        private TilemapService _tilemapService;

        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            InputService inputService,
            InventoryService inventoryService,
            StatsService statsService,
            TilemapService tilemapService)
        {
            _inputService = inputService;
            _inventoryService = inventoryService;
            _statsService = statsService;
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

            SubscribeOnInput();
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
            _speedModifier = isRunning ? 2 : 1;
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
            _rigidbody2D.velocity = _statsService.MovementSpeed * _speedModifier * _moveDirection;
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

            var transformPosition = transform.position;
            if (direction != Vector2.zero)
            {
                TargetDirection.Value = Vector3Int.RoundToInt(direction);
            }
            TargetPosition.Value = new Vector3Int(Mathf.FloorToInt(transformPosition.x), Mathf.RoundToInt(transformPosition.y)) + TargetDirection.Value;
            _tilemapService.ProcessTiles(_tilemapService.CurrentTilemap.Value, TargetPosition.Value);

            return direction;
        }

        private void OnDestroy()
        {
            _statsService.OnSessionEnded(transform.position);
            _compositeDisposable.Dispose();
        }
    }
}