using TendedTarsier.Script.Modules.Gameplay.Inventory;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private InputService _inputService;
        private GameplayConfig _gameplayConfig;
        private PlayerProfile _playerProfile;
        private GameplayController _gameplayController;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;
        private Transform _itemsTransform;

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

            _inputService.OnMovePerformed.First().Subscribe(_ => _gameplayController.OnGameplayStarted());
            CurrentTilemap.SkipLatestValueOnSubscribe().First().Subscribe(_ => OnMove(Vector2.down));
        }

        private void FixedUpdate()
        {
            ProcessMovement();
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

        private void ProcessMovement()
        {
            var moveDirection = Vector2.zero;
            if (Gamepad.current.leftStick.IsActuated())
            {
                moveDirection = OnMove(Gamepad.current.leftStick.ReadValue());
            }

            var speedModifier = Gamepad.current.aButton.isPressed ? 2 : 1;
            _rigidbody2D.velocity = _gameplayConfig.MovementSpeed * speedModifier * moveDirection;
            _animator.SetBool(_isMovingAnimatorKey, moveDirection.magnitude > 0);
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
        }
    }
}