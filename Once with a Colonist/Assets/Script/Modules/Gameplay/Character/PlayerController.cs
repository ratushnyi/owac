using TendedTarsier.Script.Modules.Gameplay.Inventory;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerController : MonoBehaviour
    {
        public readonly ReactiveProperty<Vector3Int> TargetDirection = new();
        public readonly ReactiveProperty<Vector3Int> TargetPosition = new();
        public readonly ReactiveProperty<Tilemap> CurrentTilemap = new();

        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        private Vector2 _moveDirection;
        private float _speedModifier = 1;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private InputService _inputService;
        private GameplayConfig _gameplayConfig;
        private PlayerProfile _playerProfile;
        private GameplayController _gameplayController;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;
        private Transform _itemsTransform;

        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            InputService inputService,
            GameplayConfig gameplayConfig,
            PlayerProfile playerProfile,
            GameplayController gameplayController,
            InventoryService inventoryService,
            HUDService hudService,
            TilemapService tilemapService)
        {
            _tilemapService = tilemapService;
            _inventoryService = inventoryService;
            _gameplayController = gameplayController;
            _playerProfile = playerProfile;
            _gameplayConfig = gameplayConfig;
            _inputService = inputService;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            transform.SetLocalPositionAndRotation(_playerProfile.PlayerPosition, Quaternion.identity);

            CurrentTilemap.SkipLatestValueOnSubscribe().First().Subscribe(_ => OnMove(Vector2.down));
            SubscribeOnInput();
        }

        private void SubscribeOnInput()
        {
            _inputService.OnLeftStickPerformed
                .First()
                .Subscribe(_ => _gameplayController.OnGameplayStarted());

            _inputService.OnLeftStickPerformed
                .Subscribe(t => ProcessMovement(t.ReadValue<Vector2>()))
                .AddTo(_compositeDisposable);

            _inputService.OnLeftStickCanceled
                .Subscribe(_ => ProcessMovement(Vector2.zero))
                .AddTo(_compositeDisposable);

            _inputService.OnAButtonStarted
                .Subscribe(_ => OnSpeedChanged(true))
                .AddTo(_compositeDisposable);

            _inputService.OnAButtonCanceled
                .Subscribe(_ => OnSpeedChanged(false))
                .AddTo(_compositeDisposable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                CurrentTilemap.Value = other.GetComponent<Tilemap>();
            }
            else if (other.CompareTag("Item"))
            {
                var mapItem = other.GetComponent<MapItemBase>();
                _inventoryService.TryPut(mapItem);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                if (CurrentTilemap.Value.gameObject == other.gameObject)
                {
                    CurrentTilemap.Value = null;
                }
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
            _rigidbody2D.velocity = _gameplayConfig.MovementSpeed * _speedModifier * _moveDirection;
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
            TargetDirection.Value = Vector3Int.RoundToInt(direction);
            TargetPosition.Value = new Vector3Int(Mathf.FloorToInt(transformPosition.x), Mathf.RoundToInt(transformPosition.y)) + TargetDirection.Value;
            _tilemapService.ProcessTiles(CurrentTilemap.Value, TargetPosition.Value);

            return direction;
        }

        private void OnDestroy()
        {
            _playerProfile.PlayerPosition = transform.position;
            _playerProfile.Save();
            _compositeDisposable.Dispose();
        }
    }
}