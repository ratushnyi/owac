using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.InputSystem;

namespace TendedTarsier.Script.Modules.General.Services.Input
{
    [UsedImplicitly]
    public class InputService : ServiceBase
    {
        private readonly GameplayInput _gameplayInput;

        public IObservable<InputAction.CallbackContext> OnLeftStickStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnLeftStickPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnLeftStickCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnRightStickStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnRightStickPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnRightStickCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnXButtonStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnXButtonPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnXButtonCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnYButtonStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnYButtonPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnYButtonCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnAButtonStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnAButtonPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnAButtonCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnBButtonStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnBButtonPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnBButtonCanceled { get; private set; }

        public IObservable<InputAction.CallbackContext> OnMenuButtonStarted { get; private set; }
        public IObservable<InputAction.CallbackContext> OnMenuButtonPerformed { get; private set; }
        public IObservable<InputAction.CallbackContext> OnMenuButtonCanceled { get; private set; }

        public InputService(GameplayInput gameplayInput)
        {
            _gameplayInput = gameplayInput;
        }

        public override void Initialize()
        {
            InitInput();
        }

        private void InitInput()
        {
            OnLeftStickStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.LeftStick.started += t, t => _gameplayInput.Gameplay.LeftStick.started -= t);
            OnLeftStickPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.LeftStick.performed += t, t => _gameplayInput.Gameplay.LeftStick.performed -= t);
            OnLeftStickCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.LeftStick.canceled += t, t => _gameplayInput.Gameplay.LeftStick.canceled -= t);

            OnRightStickStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.RightStick.started += t, t => _gameplayInput.Gameplay.RightStick.started -= t);
            OnRightStickPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.RightStick.performed += t, t => _gameplayInput.Gameplay.RightStick.performed -= t);
            OnRightStickCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.RightStick.canceled += t, t => _gameplayInput.Gameplay.RightStick.canceled -= t);

            OnXButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonX.started += t, t => _gameplayInput.Gameplay.ButtonX.started -= t);
            OnXButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonX.performed += t, t => _gameplayInput.Gameplay.ButtonX.performed -= t);
            OnXButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonX.canceled += t, t => _gameplayInput.Gameplay.ButtonX.canceled -= t);

            OnYButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonY.started += t, t => _gameplayInput.Gameplay.ButtonY.started -= t);
            OnYButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonY.performed += t, t => _gameplayInput.Gameplay.ButtonY.performed -= t);
            OnYButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonY.canceled += t, t => _gameplayInput.Gameplay.ButtonY.canceled -= t);

            OnAButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonA.started += t, t => _gameplayInput.Gameplay.ButtonA.started -= t);
            OnAButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonA.performed += t, t => _gameplayInput.Gameplay.ButtonA.performed -= t);
            OnAButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonA.canceled += t, t => _gameplayInput.Gameplay.ButtonA.canceled -= t);

            OnBButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonB.started += t, t => _gameplayInput.Gameplay.ButtonB.started -= t);
            OnBButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonB.performed += t, t => _gameplayInput.Gameplay.ButtonB.performed -= t);
            OnBButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.ButtonB.canceled += t, t => _gameplayInput.Gameplay.ButtonB.canceled -= t);

            OnMenuButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.Menu.started += t, t => _gameplayInput.Gameplay.Menu.started -= t);
            OnMenuButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.Menu.performed += t, t => _gameplayInput.Gameplay.Menu.performed -= t);
            OnMenuButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Gameplay.Menu.canceled += t, t => _gameplayInput.Gameplay.Menu.canceled -= t);

            _gameplayInput.Gameplay.Enable();
        }
    }
}