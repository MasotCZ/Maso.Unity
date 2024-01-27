using Masot.Standard.Input.EventArguments;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Scroll;
using System;
using UnityEngine;

namespace Masot.Standard.Input
{
    public static class InputExtensions
    {
        private static IInput _input = Input.Instance;

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

        public static void RegisterInput<_EVENTARGS>(this object obj, KeyCode key, InputDelegate<_EVENTARGS> handler, params object[] keys)
           where _EVENTARGS : EventArgsBase
        {
            obj.RegisterInput(new InputData(key), handler, obj, keys);
        }

        public static void RegisterInput<_EVENTARGS>(this object obj, InputData inputData, InputDelegate<_EVENTARGS> handler, params object[] keys)
           where _EVENTARGS : EventArgsBase
        {
            obj.RegisterInput(new InputDefine(inputData), handler, obj, keys);
        }

        public static void RegisterInput<_EVENTARGS>(this object obj, IInputDefine input, InputDelegate<_EVENTARGS> handler, params object[] keys)
            where _EVENTARGS : EventArgsBase
        {
            _input.Register(input, handler, obj, keys);
        }

        public static void RemoveInput<_EVENTARGS>(this object obj, InputDelegate<_EVENTARGS> handler)
        {
            obj.RemoveInput(handler as object);
        }

        public static void RemoveInput(this object obj, params object[] key)
        {
            _input.Remove(obj, key);
        }
    }
}
