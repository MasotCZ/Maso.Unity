using Masot.Framework.DataStructures.Factories;
using System;

namespace Masot.Unity.DataStructures.DataPools
{
    public class RefPoolCollection<_T> : IRefPoolCollection<_T> where _T : class
    {
        private readonly int _refSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr));
        private IntPtr _start;
        private int _unusedIndex = 0;
        private int _used = 0;
        private _T[] _data;
        private readonly IDataPoolFactory<_T> _factory;

        public RefPoolCollection(IDataPoolFactory<_T> factory, int initSize)
        {
            _data = new _T[initSize];
            _start = GetPtr(_data);
            _factory = factory;
        }

        public int Count => _data.Length;
        public int Used => _used;
        public IDataPoolFactory<_T> DataFactory => _factory;

        unsafe
        private IntPtr GetPtr(object obj)
        {
            TypedReference tr = __makeref(obj);
            return **(IntPtr**)(&tr);
        }

        private void Swap(int a , int b)
        {
            var temp = _data[a];
            _data[a] = _data[b];
            _data[b] = temp;
        }

        private void Populate(_T[] arr, int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                arr[start + i] = _factory.Create();
            }
        }

        private void Resize(int newSize)
        {
            var _newData = new _T[newSize];
            System.Buffer.BlockCopy(_data, 0, _newData, 0, _data.Length);
            Populate(_newData, _data.Length, newSize - _data.Length);
            _data = _newData;
            _start = GetPtr(_data);
        }

        _T IRefPoolCollection<_T>.Get()
        {
            _used++;

            if (_used + 1 > Count)
            {
                //naive
                Resize(Count * 2);
            }

            return _data[_unusedIndex++];
        }

        public void Remove(_T item)
        {
            var ptr = GetPtr(item);

            Swap(_start.ToInt32() - ptr.ToInt32(), _unusedIndex - 1);

            _unusedIndex--;
            _used--;
        }
    }
}
