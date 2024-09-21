using System;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using UnityEngine.InputSystem;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Input
{
    [UsedImplicitly]
    public class InputService : ServiceBase
    {
        private readonly GameplayInput _gameplayInput;
        
        public IObservable<InputAction.CallbackContext> OnLeftStickStarted;
        public IObservable<InputAction.CallbackContext> OnLeftStickPerformed;
        public IObservable<InputAction.CallbackContext> OnLeftStickCanceled;
        
        public IObservable<InputAction.CallbackContext> OnRightStickStarted;
        public IObservable<InputAction.CallbackContext> OnRightStickPerformed;
        public IObservable<InputAction.CallbackContext> OnRightStickCanceled;
        
        public IObservable<InputAction.CallbackContext> OnXButtonStarted;
        public IObservable<InputAction.CallbackContext> OnXButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnXButtonCanceled;
        
        public IObservable<InputAction.CallbackContext> OnYButtonStarted;
        public IObservable<InputAction.CallbackContext> OnYButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnYButtonCanceled;
        
        public IObservable<InputAction.CallbackContext> OnAButtonStarted;
        public IObservable<InputAction.CallbackContext> OnAButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnAButtonCanceled;
        
        public IObservable<InputAction.CallbackContext> OnBButtonStarted;
        public IObservable<InputAction.CallbackContext> OnBButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnBButtonCanceled;

        public InputService(GameplayInput gameplayInput)
        {
            _gameplayInput = gameplayInput;

            InitInput();
        }

        private void InitInput()
        {
            OnLeftStickStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.started += t, t => _gameplayInput.Player.Move.started -= t);
            OnLeftStickPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.performed += t, t => _gameplayInput.Player.Move.performed -= t);
            OnLeftStickCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.canceled += t, t => _gameplayInput.Player.Move.canceled -= t);
            
            OnRightStickStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.started += t, t => _gameplayInput.Player.Move.started -= t);
            OnRightStickPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.performed += t, t => _gameplayInput.Player.Move.performed -= t);
            OnRightStickCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.canceled += t, t => _gameplayInput.Player.Move.canceled -= t);
            
            OnXButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.started += t, t => _gameplayInput.Player.ButtonX.started -= t);
            OnXButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.performed += t, t => _gameplayInput.Player.ButtonX.performed -= t);
            OnXButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.canceled += t, t => _gameplayInput.Player.ButtonX.canceled -= t);
            
            OnYButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.started += t, t => _gameplayInput.Player.ButtonY.started -= t);
            OnYButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.performed += t, t => _gameplayInput.Player.ButtonY.performed -= t);
            OnYButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.canceled += t, t => _gameplayInput.Player.ButtonY.canceled -= t);
            
            OnAButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.started += t, t => _gameplayInput.Player.ButtonA.started -= t);
            OnAButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.performed += t, t => _gameplayInput.Player.ButtonA.performed -= t);
            OnAButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.canceled += t, t => _gameplayInput.Player.ButtonA.canceled -= t);
            
            OnBButtonStarted = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.started += t, t => _gameplayInput.Player.ButtonB.started -= t);
            OnBButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.performed += t, t => _gameplayInput.Player.ButtonB.performed -= t);
            OnBButtonCanceled = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.canceled += t, t => _gameplayInput.Player.ButtonB.canceled -= t);

            _gameplayInput.Player.Enable();
        }
    }
}