using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier.Character
{
    public class PlayerController : MonoBehaviour
    {
        private readonly int _directionAnimatorKey = Animator.StringToHash("Direction");
        private readonly int _isMovingAnimatorKey = Animator.StringToHash("IsMoving");

        private IObservable<InputAction.CallbackContext> _onMovePerformed;
        private IObservable<InputAction.CallbackContext> _onXButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onYButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onAButtonPerformed;
        private IObservable<InputAction.CallbackContext> _onBButtonPerformed;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private Vector3Int _currentDirection = Vector3Int.down;
        private Vector3Int? _previousTilePosition;
        private Vector3Int? _currentTilePosition;
        private Tilemap _currentGround;

        private GameplayConfig _gameplayConfig;
        private GameplayProfile _gameplayProfile;
        private GameplayController _gameplayController;
        private GameplayInput _gameplayInput;

        [Inject]
        private void Construct(GameplayConfig gameplayConfig, GameplayProfile gameplayProfile, GameplayController gameplayController, GameplayInput gameplayInput)
        {
            _gameplayInput = gameplayInput;
            _gameplayController = gameplayController;
            _gameplayProfile = gameplayProfile;
            _gameplayConfig = gameplayConfig;
        }

        private void Start()
        {
            InitInput();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            transform.SetLocalPositionAndRotation(_gameplayProfile.PlayerPosition, Quaternion.identity);

            _onMovePerformed.First().Subscribe(_ => _gameplayController.OnGameplayStarted());
        }

        private void InitInput()
        {
            _onMovePerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.performed += t, t => _gameplayInput.Player.Move.performed -= t);
            _onXButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.performed += t, t => _gameplayInput.Player.ButtonX.performed -= t);
            _onYButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.performed += t, t => _gameplayInput.Player.ButtonY.performed -= t);
            _onAButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.performed += t, t => _gameplayInput.Player.ButtonA.performed -= t);
            _onBButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.performed += t, t => _gameplayInput.Player.ButtonB.performed -= t);

            _gameplayInput.Player.Enable();
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }

        private void Update()
        {
            ProcessTools();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                _currentGround = other.GetComponent<Tilemap>();
                ProcessTiles();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                if (_currentGround.gameObject == other.gameObject)
                {
                    _currentGround = null;
                    _currentTilePosition = null;
                }
            }
        }

        private void ProcessTools()
        {
            if (Gamepad.current.xButton.isPressed)
            {
                if (_currentTilePosition != null)
                {
                    _currentGround.SetTile(_currentTilePosition.Value, _gameplayConfig.PerformedTile);
                }
            }
        }

        private void ProcessMovement()
        {
            var direction = Gamepad.current.leftStick.ReadValue();
            if (Gamepad.current.leftStick.IsActuated())
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

                ProcessTiles();
            }

            _currentDirection = Vector3Int.RoundToInt(direction);
            var modifier = Gamepad.current.aButton.isPressed ? 2 : 1;
            _rigidbody2D.velocity = _gameplayConfig.MovementSpeed * modifier * direction;
            _animator.SetBool(_isMovingAnimatorKey, direction.magnitude > 0);
        }

        private void ProcessTiles()
        {
            if (_currentGround != null)
            {
                var transformPosition = transform.position;
                var playerPosition = new Vector3Int(Mathf.FloorToInt(transformPosition.x), Mathf.RoundToInt(transformPosition.y));
                var currentPosition = _currentGround.WorldToCell(playerPosition + _currentDirection);

                if (_previousTilePosition == null || currentPosition != _previousTilePosition)
                {
                    if (_previousTilePosition != null)
                    {
                        _currentGround.SetColor(_previousTilePosition.Value, Color.white);
                    }

                    var tile = _currentGround.GetTile(currentPosition);
                    if (tile != null)
                    {
                        _currentGround.SetColor(currentPosition, Color.red);
                        _currentTilePosition = currentPosition;
                        _previousTilePosition = currentPosition;
                    }
                    else
                    {
                        _currentTilePosition = null;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _gameplayProfile.PlayerPosition = transform.position;
            _gameplayProfile.Save();
        }
    }
}