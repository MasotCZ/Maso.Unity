using Masot.Standard.Input.EventArguments;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Scroll;
using Masot.Standard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;

namespace Masot.Standard.Input
{
    public delegate void InputDelegate<_T>(_T argument);

    public enum GetKeyType
    {
        Release,
        Hold,
        Press
    }

    /// <summary>
    /// defines where the input can be intercepted
    /// </summary>
    [Flags]
    public enum InputLayer
    {
        // defaul layer
        Gameplay = 1,
        Ui = 2
    }

    public interface IInputConfiguration
    {
        public InputLayer InputLayer { get; }
    }

    public interface IInputData
    {
        public KeyCode KeyCode { get; }
        public GetKeyType GetKeyType { get; }
    }

    public struct InputData : IInputData
    {
        public static InputData Default = new InputData(KeyCode.None);

        public KeyCode KeyCode { get; }
        public GetKeyType GetKeyType { get; }

        public InputData(KeyCode keyCode, GetKeyType getKeyType = GetKeyType.Press)
        {
            KeyCode = keyCode;
            GetKeyType = getKeyType;
        }
    }

    public interface IInputDefine
    {
        public IInputData InputData { get; }
        public IInputConfiguration InputConfiguration { get; }
    }

    public class InputDefine : IInputDefine
    {
        public InputDefine(IInputData inputData, IInputConfiguration inputConfiguration = null)
        {
            InputData = inputData;
            InputConfiguration = inputConfiguration;
        }

        public IInputData InputData { get; }
        public IInputConfiguration InputConfiguration { get; }
    }

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

        public void ProcessEvents();
        public void AddGuiEvent(Event @event);

        public void SyncCameraWithInputs(Transform cameraTransform, Vector3 oldCameraPosition);
    }

    internal readonly struct MultiKeyDictionaryKey
    {
        private readonly object[] _keys;

        public MultiKeyDictionaryKey(object[] keys)
        {
            _keys = keys;
        }

        public override bool Equals(object? obj)
        {
            return obj is MultiKeyDictionaryKey key &&
                   EqualityComparer<object[]>.Default.Equals(_keys, key._keys);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_keys);
        }
    }

    internal class MultiKeyDictionaryValue
    {

    }

    internal interface IKeyCodeEventData
    {
        void Trigger(IInputData input);
    }

    internal interface IKeyCodeEventContainer
    {
        void Add(MultiKeyDictionaryKey key, IInputData input, IKeyCodeEventData data);
        void Remove(MultiKeyDictionaryKey key);
        bool Process(IInputData input);
    }

    internal abstract class KeyCodeEventContainerBase : IKeyCodeEventContainer
    {
        private readonly CommandBuffer commandBuffer;

        /// <summary>
        /// TODO - currently every trigger gets a key, actually every key combination gets a trigger
        /// but really a key should have a trigger and then another dictionary with multkeys should be used to reference it
        /// </summary>
        private readonly Dictionary<MultiKeyDictionaryKey, (IInputData inputData, IKeyCodeEventData keyCodeEventData)> _keyToDataAndTriggerDictionary;
        private readonly Dictionary<IInputData, ICollection<IKeyCodeEventData>> _inputToTriggerDictionary;

        private class IInputDataKeyComparer : IEqualityComparer<IInputData>
        {
            public bool Equals(IInputData x, IInputData y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(IInputData obj)
            {
                return obj.GetHashCode();
            }
        }

        protected KeyCodeEventContainerBase()
        {
            commandBuffer = new CommandBuffer();
            _keyToDataAndTriggerDictionary = new Dictionary<MultiKeyDictionaryKey, (IInputData, IKeyCodeEventData)>();
            _inputToTriggerDictionary = new Dictionary<IInputData, ICollection<IKeyCodeEventData>>(new IInputDataKeyComparer());
        }

        public void Add(MultiKeyDictionaryKey key, IInputData input, IKeyCodeEventData data)
        {
            // TODO - jesus
            // probly no problem since it just adds, only a problem if multiple add at the same timne ???
            //_keyDictionary.Add(key, (input, data));

            //commandBuffer.Add(new CollectionAddCommand<IKeyCodeEventData>(this[input], data));
            //commandBuffer.Add(new DictionaryRemoveCommand<IInputData, ICollection<IKeyCodeEventData>>(this, input, () => { return this[input].Count == 0; }));

            commandBuffer.Add(new Command(() =>
            {
                _keyToDataAndTriggerDictionary.Add(key, (input, data));
                if (!_inputToTriggerDictionary.ContainsKey(input))
                {
                    _inputToTriggerDictionary.Add(input, new List<IKeyCodeEventData>());
                }

                _inputToTriggerDictionary[input].Add(data);
            }));
        }

        public void Remove(MultiKeyDictionaryKey key)
        {
            // TODO - jesus
            //var keyData = _keyDictionary[key];
            //commandBuffer.Add(new CollectionRemoveCommand<IKeyCodeEventData>(this[keyData.Item1], keyData.Item2));

            commandBuffer.Add(new Command(() =>
            {
                (IInputData inputData, IKeyCodeEventData keyCodeEventData) keyData;
                var removed = _keyToDataAndTriggerDictionary.Remove(key, out keyData);

                if (!removed)
                    return;

                var curr = _inputToTriggerDictionary[keyData.inputData];
                curr.Remove(keyData.keyCodeEventData);
                if (curr.Count() == 0)
                {
                    _inputToTriggerDictionary.Remove(keyData.inputData);
                }
            }));
        }

        protected void ProcessCommandBuffer()
        {
            commandBuffer.Process();
        }

        public bool Process(IInputData input)
        { 
            ProcessCommandBuffer();
            return Process(input, _inputToTriggerDictionary);
        }

        protected abstract bool Process(IInputData input, Dictionary<IInputData, ICollection<IKeyCodeEventData>> data);
    }

    internal class KeyCodeEventData<_T> : IKeyCodeEventData where _T : EventArgsBase
    {
        private readonly InputDelegate<_T> callback;

        public KeyCodeEventData(InputDelegate<_T> callback)
        {
            this.callback = callback;
        }


        public void Trigger(IInputData input)
        {
            //2 ways to go about it
            //1 create default Argument and then give input to it
            //problem - input could be used in the constructor and hence the values in the argument could be invalid

            //var arg = new _T();
            //arg.Input = input;
            //callback.Invoke(arg);

            //2 create Argument at runtime via reflection and supply the argument
            //probly best , could be slow and have problems with arguments
            //could be slowing down the event system, probly not

            callback.Invoke(Activator.CreateInstance(typeof(_T), input) as _T);
        }
    }

    class NonBlockingKeyCodeEventContainer : KeyCodeEventContainerBase
    {
        protected override bool Process(IInputData input, Dictionary<IInputData, ICollection<IKeyCodeEventData>> data)
        {
            if (!data.ContainsKey(input))
            {
                return false;
            }

            foreach (var item in data[input])
            {
                item.Trigger(input);
            }

            return true;
        }
    }

    internal class MultiKeyDictionary
    {
        private readonly Dictionary<MultiKeyDictionaryKey, IInputDefine> _functorDict;
        private readonly Dictionary<IInputData, HashSet<IInputDefine>> _inputTriggerDict;

        // TODO - optimize
        public void Add(IInputDefine inputDefine, MultiKeyDictionaryKey key)
        {
            _inputTriggerDict.AddToSubCollection(inputDefine.InputData, inputDefine);
            _functorDict.Add(key, inputDefine);
        }

        public void Remove(MultiKeyDictionaryKey key)
        {
            var cock = _functorDict[key];
            _functorDict.Remove(key);
            _inputTriggerDict.RemoveFromSubCollection(cock.InputData, cock);
        }

        // TODO - optimize
        public IEnumerable<IInputDefine> GetTriggers(IInputData inputData)
        {
            return _inputTriggerDict
                .Where(kv => kv.Key.Equals(inputData))
                .Select(kv => kv.Value)
                .SelectMany(v => v);
        }
    }

    internal class Input : SingletonBase<Input>, IInput
    {
        private readonly CommandBuffer<EventCommand> eventBuffer;
        private readonly HashSet<KeyCode> heldKeys;
        private readonly NonBlockingKeyCodeEventContainer InputContainer;

        private static KeyCode GetMouseButton(int button) => button switch
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
            heldKeys = new HashSet<KeyCode>();
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
            heldKeys.Remove(k);
            return GetKeyType.Release;
        }

        private GetKeyType KeyDown(KeyCode k)
        {
            heldKeys.Add(k);
            return GetKeyType.Press;
        }

        //Get events from key event detector
        private void KeyCodeEvent(EventData e)
        {
            GetKeyType type = e.type is EventType.KeyUp ?
                KeyUp(e.keyCode) : heldKeys.Contains(e.keyCode) ?
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

        //todo it works only while the camera is not tilted
        public Vector3 MouseWorldPosition
        {
            get { return MainCamera.ScreenToWorldPoint(new Vector3(MouseScreenPosition.x, MouseScreenPosition.y, -MainCamera.transform.position.z)); }
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

        public Vector3 ScreenToWorld(Vector3 screen)
        {
            var half = HalfSize;
            var m = WorldToScreenMulti;
            var ok1 = MainCamera.ScreenToWorldPoint(MainCamera.transform.position + Vector3.one);
            var ok2 = MainCamera.ScreenToWorldPoint(MainCamera.transform.position);
            m = ok1 - ok2;

            return new Vector3(
                (screen.x - half.x) / m.x + MainCamera.transform.position.x,
                (screen.y - half.y) / m.y + MainCamera.transform.position.y,
                (screen.z) / m.z);
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
            GetKeyType type = e.type is EventType.MouseDown ? heldKeys.Contains(e.keyCode) ?
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
            //Debug.Log($"Scroll: {e.delta}");
            OnMouseScrollEvent(e.delta.y);
        }

        private void CheckForHeldKeys()
        {
            foreach (var item in heldKeys)
            {
                TriggerEvent(new InputData(item, GetKeyType.Hold));
            }
        }

        private void CheckForMouseDrag()
        {
            var drag = MouseDrag;
            if (drag != Vector2.zero)
            {
                //Debug.Log($"Mouse Dragging deez nuts: {drag}");
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
                keyCode = e.isMouse ? GetMouseButton(e.button) : e.keyCode;
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
                //Debug.Log($"event {e}");
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

        //event filter
        private EventCommand GetEventCommand(Event e) => e switch
        {
            { isKey: true, keyCode: not KeyCode.None } => new EventCommand(KeyCodeEvent, new EventData(e)),
            { isMouse: true, type: EventType.MouseDown or EventType.MouseUp } => new EventCommand(MouseKeyEvent, new EventData(e)),
            { isScrollWheel: true } => new EventCommand(MouseScrollEvent, new EventData(e)),
            _ => null
        };

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
    }
}
