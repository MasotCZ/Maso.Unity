using System;
using System.Collections.Generic;
using System.Text;

namespace Masot.Standard.Utility
{

    public interface ICommand
    {
        void Execute();
    }

    public class ListAddAtCommand<_T> : ICommand
    {
        private readonly IList<_T> collection;
        private readonly _T data;
        private int index;

        public ListAddAtCommand(IList<_T> collection, _T data, int index)
        {
            this.collection = collection;
            this.data = data;
            this.index = index;
        }

        public void Execute()
        {
            collection.Insert(index, data);
        }
    }

    public class Command : ICommand
    {
        private readonly Action action;

        public Command(Action action)
        {
            this.action = action;
        }

        public void Execute()
        {
            action.Invoke();
        }
    }

    public class CollectionAddCommand<_T> : ICommand
    {
        private readonly ICollection<_T> collection;
        private readonly _T data;

        public CollectionAddCommand(ICollection<_T> collection, _T data)
        {
            this.collection = collection;
            this.data = data;
        }

        public void Execute()
        {
            collection.Add(data);
        }
    }

    public class CollectionRemoveCommand<_T> : ICommand
    {
        private readonly ICollection<_T> collection;
        private readonly _T data;

        public CollectionRemoveCommand(ICollection<_T> collection, _T data)
        {
            this.collection = collection;
            this.data = data;
        }

        public void Execute()
        {
            collection.Remove(data);
        }
    }

    public class CollectionTryRemoveCommand<_T> : ICommand
    {
        private readonly ICollection<_T> collection;
        private readonly _T data;

        public CollectionTryRemoveCommand(ICollection<_T> collection, _T data)
        {
            this.collection = collection;
            this.data = data;
        }

        public void Execute()
        {
            if (collection.Contains(data))
                collection.Remove(data);
        }
    }

    public class DictionaryAddOnIndexCommand<_K, _C, _T> : ICommand where _C : ICollection<_T>
    {
        private readonly IDictionary<_K, _C> dictionary;
        private readonly _K index;
        private readonly _T data;

        public DictionaryAddOnIndexCommand(IDictionary<_K, _C> dictionary, _K index, _T data)
        {
            this.dictionary = dictionary;
            this.index = index;
            this.data = data;
        }

        public void Execute()
        {
            dictionary[index].Add(data);
        }
    }

    public class DictionaryAddCommand<_K, _T> : ICommand
    {
        private readonly IDictionary<_K, _T> dictionary;
        private readonly _K key;
        private readonly _T data;

        public DictionaryAddCommand(IDictionary<_K, _T> dictionary, _K key, _T data)
        {
            this.dictionary = dictionary;
            this.key = key;
            this.data = data;
        }

        public void Execute()
        {
            if (dictionary.ContainsKey(key))
            {
                return;
            }

            dictionary.Add(key, data);
        }
    }

    public class DictionaryRemoveCommand<_K, _T> : ICommand
    {
        private readonly IDictionary<_K, _T> dictionary;
        private readonly _K key;
        private readonly Func<bool> cond;

        public DictionaryRemoveCommand(IDictionary<_K, _T> dictionary, _K key, Func<bool> cond = null)
        {
            this.dictionary = dictionary;
            this.key = key;
            this.cond = cond;
        }

        public void Execute()
        {
            if (cond != null && !cond.Invoke())
            {
                return;
            }

            dictionary.Remove(key);
        }
    }

    public class CommandBuffer<_T> where _T : ICommand
    {
        private readonly List<_T> _buffer = new List<_T>();

        public void Add(_T toAdd)
        {
            _buffer.Add(toAdd);
        }

        public void Remove(_T toRemove)
        {
            _buffer.Remove(toRemove);
        }

        public int Count
        {
            get
            {
                return _buffer.Count;
            }
        }

        public bool Empty
        {
            get
            {
                return _buffer.Count == 0;
            }
        }

        public void Clear()
        {
            _buffer.Clear();
        }

        public void Process()
        {
            if (Empty)
            {
                return;
            }

            foreach (var command in _buffer)
            {
                command.Execute();
            }

            _buffer.Clear();
        }
    }

    public class CommandBuffer : CommandBuffer<ICommand>
    {
    }
}
