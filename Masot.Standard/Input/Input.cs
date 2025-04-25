using Masot.Standard.Input.EventArguments;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Scroll;
using Masot.Standard.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Masot.Standard.Input
{

    /// <summary>
    /// InputController for all things input-wise
    /// </summary>
    internal interface IInput
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keys">if no key is defined the key will be bound to its handler</param>
        public void Register<_EVENTARGS>(IInputDefine input, InputDelegate<_EVENTARGS> handler, params object[] keys)
            where _EVENTARGS : EventArgsBase;
        public void Remove(params object[] key);

        public InputDelegate<MouseAxisDragEventArgs> MouseDragEventHandler { get; set; }
        public InputDelegate<MouseScrollEventArgs> MouseScrollEventHandler { get; set; }

        public Vector3 MouseWorldPosition { get; }

        public void ProcessEvents();
        public void AddGuiEvent(Event @event);

        public void SyncCameraWithInputs(Transform cameraTransform, Vector3 oldCameraPosition);
    }

    internal class Input : SingletonBase<Input>, IInput
    {
        private readonly CommandBuffer<EventCommand> eventBuffer;
        private readonly HashSet<KeyCode> activelyHeldKeys;
        private readonly NonBlockingKeyCodeEventContainer InputContainer;

        private static KeyCode TranslateMouseButton(int button) => button switch
        {
            0 => KeyCode.Mouse0,
            1 => KeyCode.Mouse1,
            2 => KeyCode.Mouse2,
            3 => KeyCode.Mouse3,
            4 => KeyCode.Mouse4,
            5 => KeyCode.Mouse5,
            6 => KeyCode.Mouse6,
            _ => KeyCode.None
        };

        private Input()
        {
            InputContainer = new NonBlockingKeyCodeEventContainer();
            activelyHeldKeys = new HashSet<KeyCode>();
            eventBuffer = new CommandBuffer<EventCommand>();
        }

        public void Register<_EVENTARGS>(IInputDefine input, InputDelegate<_EVENTARGS> handler, params object[] keys)
            where _EVENTARGS : EventArgsBase
        {
            InputContainer.Add(new MultiKeyDictionaryKey(keys), input.InputData, new KeyCodeEventData<_EVENTARGS>(handler));
        }

        public void Remove(params object[] keys)
        {
            InputContainer.Remove(new MultiKeyDictionaryKey(keys));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true on frame it is pressed</returns>
        public bool Pressed(KeyCode key)
        {
            return UnityEngine.Input.GetKeyDown(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if key is pressed</returns>
        public bool Held(KeyCode key)
        {
            return UnityEngine.Input.GetKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if key was released on this frame</returns>
        public bool Released(KeyCode key)
        {
            return UnityEngine.Input.GetKeyUp(key);
        }

        /// <summary>
        /// main trigger event functio
        /// everything goes here if a key is to be triggered
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private void TriggerEvent(IInputData input)
        {
            InputContainer.Process(input);
        }

        private GetKeyType KeyUp(KeyCode k)
        {
            activelyHeldKeys.Remove(k);
            return GetKeyType.Release;
        }

        private GetKeyType KeyDown(KeyCode k)
        {
            activelyHeldKeys.Add(k);
            return GetKeyType.Press;
        }

        //Get events from key event detector
        private void KeyCodeEvent(EventData e)
        {
            GetKeyType type = e.type is EventType.KeyUp ?
                KeyUp(e.keyCode) : activelyHeldKeys.Contains(e.keyCode) ?
                GetKeyType.Hold :
                KeyDown(e.keyCode);

            if (type is GetKeyType.Hold)
            {
                //theres a delay from windows on the second key down if holding
                //it doenst cause double triggers, you are fine
                return;
            }

            //raycast on UI
            //var worldRaycastResult = RaycastOntoWorld();
            //var screenRaycastResult = RaycastOntoScreen();

            //if ((worldRaycastResult is not null && worldRaycastResult.Count != 0) ||
            //    (screenRaycastResult is not null && screenRaycastResult.Count != 0))
            //{
            //    //hit Ui element
            //    //cancel trigger invoke
            //    KeyDown(e.keyCode);
            //    return;
            //}

            TriggerEvent(new InputData(e.keyCode, type));
        }

        //---Camera---

        private Camera _mainCamera = null;

        public Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }
                return _mainCamera;
            }
        }

        //---Mouse---
        public bool IsMouseOverGameWindow => !(0 > UnityEngine.Input.mousePosition.x || 0 > UnityEngine.Input.mousePosition.y || Screen.width < UnityEngine.Input.mousePosition.x || Screen.height < UnityEngine.Input.mousePosition.y);
        public Vector2 ScreenPositionBL
        {
            get { return Vector2.zero; }
        }

        public Vector2 ScreenPositionTL
        {
            get { return new Vector2(0, Screen.height); }
        }

        public Vector2 ScreenMiddle
        {
            get { return new Vector2(Screen.width / 2, Screen.height / 2); }
        }

        public Vector2 MouseScreenPosition
        {
            get { return UnityEngine.Input.mousePosition; }
        }

        private Vector3 GetCameraTranslatedToAxisBase()
        {
            return  _transitionMatrix.MultiplyPoint3x4(MainCamera.transform.position);
        }

        //todo it works only while the camera is not tilted
        public Vector3 MouseWorldPosition
        {
            get
            {
                return _transitionMatrix.MultiplyPoint3x4(
                    MainCamera.ScreenToWorldPoint(new Vector3(MouseScreenPosition.x, MouseScreenPosition.y, -GetCameraTranslatedToAxisBase().z))
                );
            }
        }

        public Vector3 WorldToScreenMulti
        {
            get
            {
                return MainCamera.WorldToScreenPoint(MainCamera.transform.position + Vector3.one) -
                   MainCamera.WorldToScreenPoint(MainCamera.transform.position);
            }
        }

        private Vector2 HalfSize
        {
            get => new Vector2(Screen.width, Screen.height) / 2;
        }

        public Vector3 WorldToScreen(Vector3 world)
        {
            var half = HalfSize;

            return new Vector3(
                (world.x - MainCamera.transform.position.x) * WorldToScreenMulti.x + half.x,
                (world.y - MainCamera.transform.position.y) * WorldToScreenMulti.y + half.y,
                (world.z - MainCamera.transform.position.z) * WorldToScreenMulti.z
                );
        }

        // kinda scuffed
        public Vector3 ScreenToWorld(Vector3 screen)
        {
            var half = HalfSize;
            var m = WorldToScreenMulti;
            var ok1 = MainCamera.ScreenToWorldPoint(MainCamera.transform.position + Vector3.one);
            var ok2 = MainCamera.ScreenToWorldPoint(MainCamera.transform.position);
            m = ok1 - ok2;

            var mainCameraPosition = GetCameraTranslatedToAxisBase();

            return _transitionMatrix.MultiplyPoint3x4(new Vector3(
                (screen.x - half.x) / m.x + mainCameraPosition.x,
                (screen.y - half.y) / m.y + mainCameraPosition.y,
                (screen.z) / m.z));
        }

        //---Mouse drag---
        public Vector2 MouseDrag
        {
            get
            {
                return new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            }
        }

        //---Mouse scroll---
        public float MouseScroll
        {
            get
            {
                return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            }
        }

        public InputDelegate<MouseAxisDragEventArgs> MouseDragEventHandler { get; set; }
        private void OnMouseDragEvent(Vector2 drag)
        {
            MouseDragEventHandler?.Invoke(new MouseAxisDragEventArgs(drag, MouseScreenPosition, MouseWorldPosition, InputData.Default));
        }

        public InputDelegate<MouseScrollEventArgs> MouseScrollEventHandler { get; set; }
        private void OnMouseScrollEvent(float scroll)
        {
            MouseScrollEventHandler?.Invoke(new MouseScrollEventArgs(scroll, MouseScreenPosition, MouseWorldPosition, InputData.Default));
        }

        private void MouseKeyEvent(EventData e)
        {
            GetKeyType type = e.type is EventType.MouseDown ? activelyHeldKeys.Contains(e.keyCode) ?
                GetKeyType.Hold :
                KeyDown(e.keyCode) :
                KeyUp(e.keyCode);

            if (type is GetKeyType.Hold)
            {
                return;
            }

            TriggerEvent(new InputData(e.keyCode, type));
        }

        private void MouseScrollEvent(EventData e)
        {
            OnMouseScrollEvent(e.delta.y);
        }

        private void CheckForHeldKeys()
        {
            foreach (var item in activelyHeldKeys)
            {
                TriggerEvent(new InputData(item, GetKeyType.Hold));
            }
        }

        private void CheckForMouseDrag()
        {
            var drag = MouseDrag;
            if (drag != Vector2.zero)
            {
                OnMouseDragEvent(drag);
            }
        }

        private Vector2 GetMouseDragFromPositionDelta(Vector3 oldPosition, Vector3 newPosition)
        {
            return WorldToScreen(newPosition) - WorldToScreen(oldPosition);
        }

        public void SyncCameraWithInputs(Transform cameraTransform, Vector3 oldCameraPosition)
        {
            OnMouseDragEvent(GetMouseDragFromPositionDelta(oldCameraPosition, cameraTransform.position));
        }

        /// <summary>
        /// called once a frame
        /// depending on update type
        /// Fixed, update, onGui ...
        /// <seealso cref="Masot.Standard.Input.InputEvent.E_UnityUpdateCycle"/>
        /// </summary>
        public void ProcessEvents()
        {
            DetectNonEvents();
            eventBuffer.Process();
        }

        private class EventData
        {
            public readonly KeyCode keyCode;
            public readonly EventType type;
            public readonly Vector2 delta;

            public EventData(Event e)
            {
                keyCode = e.isMouse ? TranslateMouseButton(e.button) : e.keyCode;
                type = e.type;
                delta = e.delta;
            }

            public override string ToString()
            {
                return $"key:{keyCode}|type:{type}|delta:{delta}";
            }
        }

        private class EventCommand : ICommand
        {
            private readonly UnityAction<EventData> action;
            private readonly EventData e;
            public EventCommand(UnityAction<EventData> action, EventData e)
            {
                this.action = action;
                this.e = e;
            }

            public void Execute()
            {
                action?.Invoke(e);
            }
        }

        //todo make event more strict event sift
        //key has to have keyCode
        //mouse doesnt need keycode, uses event.button for keycode and mouse drag doesnt need either
        //scroll is always scroll
        public void AddGuiEvent(Event e)
        {
            AddEventCommand(GetEventCommand(e));
        }

        //GraphicRaycaster m_Raycaster;
        //PointerEventData m_PointerEventData;
        //EventSystem m_EventSystem;

        //// Helper method to check if the pointer is over a UI element
        //private bool IsPointerOverUIElement()
        //{
        //    //Set up the new Pointer Event
        //    m_PointerEventData = new PointerEventData(m_EventSystem);
        //    //Set the Pointer Event Position to that of the mouse position
        //    m_PointerEventData.position = Input.mousePosition;

        //    //Create a list of Raycast Results
        //    List<RaycastResult> results = new List<RaycastResult>();

        //    //Raycast using the Graphics Raycaster and mouse click position
        //    m_Raycaster.Raycast(m_PointerEventData, results);

        //    //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        //    foreach (RaycastResult result in results)
        //    {
        //        Debug.Log("Hit " + result.gameObject.name);
        //    }
        //}

        //event filter
        private EventCommand? GetEventCommand(Event e)
        {
            // Check if the event is a mouse click and if it's over a UI element
            if (e.isMouse && (e.type == EventType.MouseDown || e.type == EventType.MouseUp))//&& IsPointerOverUIElement())
            {
                // If the mouse click is over a UI element, return null to ignore the event
                return null;
            }

            return e switch
            {
                { isKey: true, keyCode: not KeyCode.None } => new EventCommand(KeyCodeEvent, new EventData(e)),
                { isMouse: true, type: EventType.MouseDown or EventType.MouseUp } => new EventCommand(MouseKeyEvent, new EventData(e)),
                { isScrollWheel: true } => new EventCommand(MouseScrollEvent, new EventData(e)),
                _ => null
            };
        }

        private void AddEventCommand(EventCommand cmd)
        {
            if (cmd is null)
            {
                return;
            }

            eventBuffer.Add(cmd);
        }

        private void DetectNonEvents()
        {
            if (!IsMouseOverGameWindow)
            {
                return;
            }

            CheckForMouseDrag();
            CheckForHeldKeys();
        }

        // -- Axis mode
        private Matrix4x4 _transitionMatrix = Matrix4x4.identity; // Default to identity matrix (XY mode)

        public void SetAxisMode(AxisMode mode)
        {
            _transitionMatrix = mode switch
            {
                AxisMode.XY => Matrix4x4.identity, // No change for XY
                AxisMode.XZ => new Matrix4x4(
                    new Vector4(1, 0, 0, 0), // X remains X
                    new Vector4(0, 0, 1, 0), // Z becomes Y
                    new Vector4(0, 1, 0, 0), // Y becomes Z
                    new Vector4(0, 0, 0, 1)  // Homogeneous coordinate
                ),
                _ => Matrix4x4.identity // Default to identity matrix
            };
        }
    }
}
