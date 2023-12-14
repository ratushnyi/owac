using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier
{
    public class PlayerController : MonoBehaviour
    {
        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ReactiveProperty<Tilemap> _currentTilemap = new();

        private IObservable<InputAction.CallbackContext> _onMovePerformed;
        private IObservable<InputAction.CallbackContext> _onXButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onYButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onAButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onBButtonPerformed;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private Vector3Int _direction;
        private Vector3Int _targetPosition;

        private GameplayConfig _gameplayConfig;
        private PlayerProfile _playerProfile;
        private GameplayController _gameplayController;
        private GameplayInput _gameplayInput;
        private InventoryService _inventoryService;
        private TilemapService _tilemapService;
        private Transform _itemsTransform;

        [Inject]
        private void Construct(
            GameplayConfig gameplayConfig,
            PlayerProfile playerProfile,
            GameplayController gameplayController,
            GameplayInput gameplayInput,
            InventoryService inventoryService,
            TilemapService tilemapService)
        {
            _tilemapService = tilemapService;
            _inventoryService = inventoryService;
            _gameplayInput = gameplayInput;
            _gameplayController = gameplayController;
            _playerProfile = playerProfile;
            _gameplayConfig = gameplayConfig;
        }

        private void Start()
        {
            InitInput();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            transform.SetLocalPositionAndRotation(_playerProfile.PlayerPosition, Quaternion.identity);

            _onMovePerformed.First().Subscribe(_ => _gameplayController.OnGameplayStarted());
            _currentTilemap.SkipLatestValueOnSubscribe().First().Subscribe(_ => OnMove(Vector2.down));
        }

        private void InitInput()
        {
            _onMovePerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.performed += t, t => _gameplayInput.Player.Move.performed -= t);
            _onXButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.performed += t, t => _gameplayInput.Player.ButtonX.performed -= t);
            _onYButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.performed += t, t => _gameplayInput.Player.ButtonY.performed -= t);
            _onAButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.performed += t, t => _gameplayInput.Player.ButtonA.performed -= t);
            _onBButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.performed += t, t => _gameplayInput.Player.ButtonB.performed -= t);

            _gameplayInput.Player.Enable();

            _onXButtonPerformed.Subscribe(OnXButtonPerformed).AddTo(_compositeDisposable);
            _onYButtonPerformed.Subscribe(OnYButtonPerformed).AddTo(_compositeDisposable);
            _onAButtonPerformed.Subscribe(OnAButtonPerformed).AddTo(_compositeDisposable);
            _onBButtonPerformed.Subscribe(OnBButtonPerformed).AddTo(_compositeDisposable);
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                _currentTilemap.Value = other.GetComponent<Tilemap>();
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
                if (_currentTilemap.Value.gameObject == other.gameObject)
                {
                    _currentTilemap.Value = null;
                }
            }
        }

        private void OnXButtonPerformed(InputAction.CallbackContext _)
        {
            if (_currentTilemap.Value != null)
            {
                _tilemapService.ChangedTile(_currentTilemap.Value, _targetPosition, TileModel.TileType.Stone);
            }
        }

        private void OnYButtonPerformed(InputAction.CallbackContext _)
        {
            _inventoryService.SwitchInventory();
        }

        private void OnAButtonPerformed(InputAction.CallbackContext _)
        {
            //
        }

        private void OnBButtonPerformed(InputAction.CallbackContext _)
        {
            _inventoryService.Drop(_targetPosition, _targetPosition + _direction * 3).Forget();
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
            _direction = Vector3Int.RoundToInt(direction);
            _targetPosition = new Vector3Int(Mathf.FloorToInt(transformPosition.x), Mathf.RoundToInt(transformPosition.y)) + _direction;
            _tilemapService.ProcessTiles(_currentTilemap.Value, _targetPosition);

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