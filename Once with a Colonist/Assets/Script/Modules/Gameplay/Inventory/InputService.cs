using System;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using UnityEngine.InputSystem;

namespace TendedTarsier.Script.Modules.Gameplay.Inventory
{
    [UsedImplicitly]
    public class InputService : ServiceBase
    {
        private readonly GameplayInput _gameplayInput;
        public IObservable<InputAction.CallbackContext> OnMovePerformed;
        public IObservable<InputAction.CallbackContext> OnXButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnYButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnAButtonPerformed;
        public IObservable<InputAction.CallbackContext> OnBButtonPerformed;
        
        public InputService(GameplayInput gameplayInput)
        {
            _gameplayInput = gameplayInput;
            
            InitInput();
        }
        
        private void InitInput()
        {
            OnMovePerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.Move.performed += t, t => _gameplayInput.Player.Move.performed -= t);
            OnXButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonX.performed += t, t => _gameplayInput.Player.ButtonX.performed -= t);
            OnYButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonY.performed += t, t => _gameplayInput.Player.ButtonY.performed -= t);
            OnAButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonA.performed += t, t => _gameplayInput.Player.ButtonA.performed -= t);
            OnBButtonPerformed = Observable.FromEvent<InputAction.CallbackContext>(t => _gameplayInput.Player.ButtonB.performed += t, t => _gameplayInput.Player.ButtonB.performed -= t);

            _gameplayInput.Player.Enable();
        }
    }
}