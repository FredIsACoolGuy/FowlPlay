// GENERATED AUTOMATICALLY FROM 'Assets/Input/GameControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Multiplayer.GameControls
{
    public class @GameControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @GameControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""b2e5f5aa-3519-4ba2-bf93-9b5c2e85f77d"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""aa500376-fc4b-404c-9d2a-cf68d12da7ca"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""72d3da62-ef73-4b63-b271-ec78546d8ead"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""592dddf9-ea09-4ec2-88c9-1f37b094dac4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""SimpleFire"",
                    ""type"": ""Button"",
                    ""id"": ""3f5dd4e4-fa0e-4e38-af10-30b598e581a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2c25d903-7562-4733-8343-aaa8b429ff13"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""be53d65b-ff27-481b-bea5-64f0bf272f49"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""04b6b5dd-b357-4e3e-85df-2b02cfc7313e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""86bbeda6-805f-44fc-8571-373b97313038"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8cd36988-35be-4114-a2d2-e6bb44e4ac8b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""343ec1bf-8ec1-4264-bd32-9d9da06cd279"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""864db247-4f17-4459-ad3e-aa3c48406c8d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43e49f6a-a247-419e-a8fb-f78b6bb92599"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboadAndMouse"",
                    ""action"": ""SimpleFire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboadAndMouse"",
            ""bindingGroup"": ""KeyboadAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
            m_Player_SimpleFire = m_Player.FindAction("SimpleFire", throwIfNotFound: true);
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

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Look;
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Fire;
        private readonly InputAction m_Player_SimpleFire;
        public struct PlayerActions
        {
            private @GameControls m_Wrapper;
            public PlayerActions(@GameControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Look => m_Wrapper.m_Player_Look;
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Fire => m_Wrapper.m_Player_Fire;
            public InputAction @SimpleFire => m_Wrapper.m_Player_SimpleFire;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @SimpleFire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSimpleFire;
                    @SimpleFire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSimpleFire;
                    @SimpleFire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSimpleFire;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                    @SimpleFire.started += instance.OnSimpleFire;
                    @SimpleFire.performed += instance.OnSimpleFire;
                    @SimpleFire.canceled += instance.OnSimpleFire;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        private int m_KeyboadAndMouseSchemeIndex = -1;
        public InputControlScheme KeyboadAndMouseScheme
        {
            get
            {
                if (m_KeyboadAndMouseSchemeIndex == -1) m_KeyboadAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboadAndMouse");
                return asset.controlSchemes[m_KeyboadAndMouseSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnLook(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnSimpleFire(InputAction.CallbackContext context);
        }
    }
}
