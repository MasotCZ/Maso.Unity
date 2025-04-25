using System;
using UnityEngine;

namespace Masot.Standard.Utility
{
    //dosesnt work with editor but oh well,
    //just make it work with like ui elements and stuff
    [Serializable]
    public class BindableProperty<TType>
    {
        [HideInInspector]
        public Action<BindableProperty<TType>>? OnChange { get; set; }
        [HideInInspector]
        public Func<BindableProperty<TType>, TType, bool>? CanChange { get; set; }

        public BindableProperty(TType value, Action<BindableProperty<TType>>? onChange, Func<BindableProperty<TType>, TType, bool>? canChange)
        {
            _value = value;
            OnChange = onChange;
            CanChange = canChange;
        }

        public BindableProperty(TType value, Action<BindableProperty<TType>> onChange)
            : this(value, onChange, default) { }

        public BindableProperty(TType value, Func<BindableProperty<TType>, TType, bool> canChange)
            : this(value, default, canChange) { }

        public BindableProperty(TType value)
            : this(value, default, default) { }


        public static implicit operator TType(BindableProperty<TType> property) => property.Value;

        [SerializeField]
        private TType _value;
        public TType Value
        {
            get => _value;
            set
            {
                if (_value!.Equals(value))
                {
                    return;
                }

                if (CanChange is not null && CanChange.Invoke(this, value))
                {
                    return;
                }

                _value = value;
                OnChange?.Invoke(this);
            }
        }

        /// <summary>
        /// doesnt fire on change method
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(TType value)
        {
            _value = value;
        }
    }
}
