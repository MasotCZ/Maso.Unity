using System;
using UnityEngine;

namespace Masot.Standard.Utility
{
    public abstract class ControllerScriptableObjectBase<_T> : ScriptableObject where _T : ScriptableObject
    {
        private static _T _instance;
        public static _T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ScriptableObject.CreateInstance<_T>();
                    (_instance as ControllerScriptableObjectBase<_T>).Init();
                }

                return _instance;
            }
        }

        protected virtual void Init()
        {
        }
    }

    public abstract class SingletonBase<_T> where _T : SingletonBase<_T>
    {
        private static readonly Lazy<_T> Lazy = new Lazy<_T>(() => Activator.CreateInstance(typeof(_T), nonPublic: true) as _T);

        public static _T Instance => Lazy.Value;
    }
}