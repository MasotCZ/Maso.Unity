using System;

namespace Game.Serialization
{
    //only used for serialization
    [Serializable]
    public class MetaHolder<_T> where _T : IObjectMeta
    {
        public _T[] Data { get; }

        public MetaHolder(_T[] data)
        {
            Data = data;
        }
    }
}