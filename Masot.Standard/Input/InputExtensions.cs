using Masot.Standard.Input.EventArguments;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Scroll;
using UnityEngine;

namespace Masot.Standard.Input
{
    public static class InputExtensions
    {
        private static IInput _input = Input.Instance;

        public static Vector3 MouseWorldPosition => _input.MouseWorldPosition;

        public static InputDelegate<MouseAxisDragEventArgs> MouseDragEventHandler
        {
            get => _input.MouseDragEventHandler;
            set => _input.MouseDragEventHandler = value;
        }

        public static InputDelegate<MouseScrollEventArgs> MouseScrollEventHandler
        {
            get => _input.MouseScrollEventHandler;
            set => _input.MouseScrollEventHandler = value;
        }

        public static void SyncCameraWithInputs(this Transform cameraTransform, Vector3 oldCameraPosition)
        {
            _input.SyncCameraWithInputs(cameraTransform, oldCameraPosition);
        }

        // ----------------- EventArgsBase -----------------
        public static void RegisterInput(this object obj, KeyCode key, InputDelegate<EventArgsBase> handler, params object[] keys)
        {
            obj.RegisterInput(new InputData(key), handler, obj, keys);
        }

        public static void RegisterInput(this object obj, KeyCode key, GetKeyType keyType, InputDelegate<EventArgsBase> handler, params object[] keys)
        {
            obj.RegisterInput(new InputData(key, keyType), handler, obj, keys);
        }

        public static void RegisterInput(this object obj, InputData inputData, InputDelegate<EventArgsBase> handler, params object[] keys)
        {
            obj.RegisterInput(new InputDefine(inputData), handler, obj, keys);
        }

        public static void RegisterInput(this object obj, IInputDefine input, InputDelegate<EventArgsBase> handler, params object[] keys)
        {
            _input.Register(input, handler, obj, keys);
        }

        public static void RemoveInput(this object obj, InputDelegate<EventArgsBase> handler, params object[] key)
        {
            _input.Remove(obj, handler, key);
        }

        // ----------------- _EVENTARGS -----------------
        public static void RegisterInput<TDelegateType>(this object obj, KeyCode key, InputDelegate<TDelegateType> handler, params object[] keys)
           where TDelegateType : EventArgsBase
        {
            obj.RegisterInput(new InputData(key), handler, obj, keys);
        }

        public static void RegisterInput<TDelegateType>(this object obj, KeyCode key, GetKeyType keyType, InputDelegate<TDelegateType> handler, params object[] keys)
           where TDelegateType : EventArgsBase
        {
            obj.RegisterInput(new InputData(key, keyType), handler, obj, keys);
        }

        public static void RegisterInput<TDelegateType>(this object obj, InputData inputData, InputDelegate<TDelegateType> handler, params object[] keys)
           where TDelegateType : EventArgsBase
        {
            obj.RegisterInput(new InputDefine(inputData), handler, obj, keys);
        }

        public static void RegisterInput<TDelegateType>(this object obj, IInputDefine input, InputDelegate<TDelegateType> handler, params object[] keys)
            where TDelegateType : EventArgsBase
        {
            _input.Register(input, handler, obj, keys);
        }

        public static void RemoveInput<TDelegateType>(this object obj, InputDelegate<TDelegateType> handler)
        {
            obj.RemoveInputs(handler as object);
        }

        public static void RemoveInput<TDelegateType>(this object obj, InputDelegate<TDelegateType> handler, params object[] key)
        {
            _input.Remove(obj, handler, key);
        }

        public static void RemoveInputs(this object obj, params object[] key)
        {
            _input.Remove(obj, key);
        }
    }
}
