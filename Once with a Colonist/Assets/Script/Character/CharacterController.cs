using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TendedTarsier.Character
{
    public class CharacterController : MonoBehaviour
    {
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private GameplayConfig _gameplayConfig;

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

        private void Update()
        {
            ProcessMovement();
            ProcessButtons();
        }

        private void ProcessMovement()
        {
            var direction = Gamepad.current.leftStick.ReadValue();

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                switch (direction.x)
                {
                    case < 0:
                        _animator.SetInteger(Direction, 3);
                        break;
                    case > 0:
                        _animator.SetInteger(Direction, 2);
                        break;
                }
            }

            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                switch (direction.y)
                {
                    case > 0:
                        _animator.SetInteger(Direction, 1);
                        break;
                    case < 0:
                        _animator.SetInteger(Direction, 0);
                        break;
                }
            }


            _animator.SetBool(IsMoving, direction.magnitude > 0);

            var modifier = Gamepad.current.aButton.isPressed ? 2 : 1;
            _rigidbody2D.velocity = _gameplayConfig.MovementSpeed * modifier * direction;
        }

        private void ProcessButtons()
        {
            if (Gamepad.current.aButton.isPressed)
            {
                Debug.Log("A Button");
            }

            if (Gamepad.current.bButton.isPressed)
            {
                Debug.Log("B Button");
            }

            if (Gamepad.current.xButton.isPressed)
            {
                Debug.Log("X Button");
            }

            if (Gamepad.current.yButton.isPressed)
            {
                Debug.Log("Y Button");
            }
        }
    }
}