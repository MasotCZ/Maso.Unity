
using UnityEngine;

namespace Masot.Standard.Utility
{
    public abstract class ControllerMonoBase<_T> : MonoBehaviour where _T : Object
    {
        private static _T _instance;
        public static _T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<_T>();
                    Debug.Assert(_instance != null, $"{nameof(_instance)} not assigned");
                }

                return _instance;
            }
            private set => _instance = value;
        }
    }
}
