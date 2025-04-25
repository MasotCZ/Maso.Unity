using Masot.Standard.Input.EventArguments;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Scroll;
using Masot.Standard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Masot.Standard.Input
{
    public delegate void InputDelegate<_T>(_T argument);
    public delegate void InputDelegate();

    public enum MovementType
    {
        Up,
        Left,
        Down,
        Right
    }

    [Serializable]
    public class MovementInputDefine
    {
        public InputData input;
        public MovementType movementType;

        public MovementInputDefine(InputData input, MovementType movementType)
        {
            this.input = input;
            this.movementType = movementType;
        }
    }

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
}
