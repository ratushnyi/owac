//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/GameplayInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameplayInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameplayInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameplayInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""60439d12-88e2-438c-86e6-2bbbc4be5f88"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""abd02a35-fc0c-4529-9990-17f07490fc77"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ButtonA"",
                    ""type"": ""Button"",
                    ""id"": ""6a14bf71-664e-4533-b78e-67053e0add9d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ButtonB"",
                    ""type"": ""Button"",
                    ""id"": ""1e7385af-12dc-4363-95a3-7b5178c7b239"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ButtonX"",
                    ""type"": ""Button"",
                    ""id"": ""8bedf50c-47d5-4107-b846-eb80481b3ba3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ButtonY"",
                    ""type"": ""Button"",
                    ""id"": ""6948f76c-ea8d-4c65-b6f8-b347aba4ecdc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f3b0830c-fe5c-4af0-97cb-a9814563f9f3"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f414280d-d1cb-4bba-9af8-f8b1c3f1f3da"",
                    ""path"": ""/<GamePad>/A"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ButtonA"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d20cfb18-7ce5-48ea-bb79-63943a8781e9"",
                    ""path"": ""/<GamePad>/B"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ButtonB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1521496-a266-4ece-a5e3-e1ff62ad9177"",
                    ""path"": ""/<GamePad>/X"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ButtonX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0326f32d-3bf4-4362-85b1-bf614045b580"",
                    ""path"": ""/<GamePad>/Y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ButtonY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_ButtonA = m_Player.FindAction("ButtonA", throwIfNotFound: true);
        m_Player_ButtonB = m_Player.FindAction("ButtonB", throwIfNotFound: true);
        m_Player_ButtonX = m_Player.FindAction("ButtonX", throwIfNotFound: true);
        m_Player_ButtonY = m_Player.FindAction("ButtonY", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_ButtonA;
    private readonly InputAction m_Player_ButtonB;
    private readonly InputAction m_Player_ButtonX;
    private readonly InputAction m_Player_ButtonY;
    public struct PlayerActions
    {
        private @GameplayInput m_Wrapper;
        public PlayerActions(@GameplayInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @ButtonA => m_Wrapper.m_Player_ButtonA;
        public InputAction @ButtonB => m_Wrapper.m_Player_ButtonB;
        public InputAction @ButtonX => m_Wrapper.m_Player_ButtonX;
        public InputAction @ButtonY => m_Wrapper.m_Player_ButtonY;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @ButtonA.started += instance.OnButtonA;
            @ButtonA.performed += instance.OnButtonA;
            @ButtonA.canceled += instance.OnButtonA;
            @ButtonB.started += instance.OnButtonB;
            @ButtonB.performed += instance.OnButtonB;
            @ButtonB.canceled += instance.OnButtonB;
            @ButtonX.started += instance.OnButtonX;
            @ButtonX.performed += instance.OnButtonX;
            @ButtonX.canceled += instance.OnButtonX;
            @ButtonY.started += instance.OnButtonY;
            @ButtonY.performed += instance.OnButtonY;
            @ButtonY.canceled += instance.OnButtonY;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @ButtonA.started -= instance.OnButtonA;
            @ButtonA.performed -= instance.OnButtonA;
            @ButtonA.canceled -= instance.OnButtonA;
            @ButtonB.started -= instance.OnButtonB;
            @ButtonB.performed -= instance.OnButtonB;
            @ButtonB.canceled -= instance.OnButtonB;
            @ButtonX.started -= instance.OnButtonX;
            @ButtonX.performed -= instance.OnButtonX;
            @ButtonX.canceled -= instance.OnButtonX;
            @ButtonY.started -= instance.OnButtonY;
            @ButtonY.performed -= instance.OnButtonY;
            @ButtonY.canceled -= instance.OnButtonY;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnButtonA(InputAction.CallbackContext context);
        void OnButtonB(InputAction.CallbackContext context);
        void OnButtonX(InputAction.CallbackContext context);
        void OnButtonY(InputAction.CallbackContext context);
    }
}