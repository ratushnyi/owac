using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier.Character
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private GameplayConfig _gameplayConfig;

        private Vector3Int _currentDirection = Vector3Int.down;
        private Vector3Int? _previousTilePosition;
        private Vector3Int? _currentTilePosition;
        private Tilemap _currentGround;

        [Inject]
        private void Construct(GameplayConfig gameplayConfig)
        {
            _gameplayConfig = gameplayConfig;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
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
                            _animator.SetInteger(Direction, 3);
                            break;
                        case > 0:
                            direction = Vector2.right;
                            _animator.SetInteger(Direction, 2);
                            break;
                    }
                }

                if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
                {
                    switch (direction.y)
                    {
                        case > 0:
                            direction = Vector2.up;
                            _animator.SetInteger(Direction, 1);
                            break;
                        case < 0:
                            direction = Vector2.down;
                            _animator.SetInteger(Direction, 0);
                            break;
                    }
                }

                ProcessTiles();
            }

            _currentDirection = Vector3Int.RoundToInt(direction);
            var modifier = Gamepad.current.aButton.isPressed ? 2 : 1;
            _rigidbody2D.velocity = _gameplayConfig.MovementSpeed * modifier * direction;
            _animator.SetBool(IsMoving, direction.magnitude > 0);
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
    }
}