using Masot.Standard.Input.EventArguments.Mouse.Position;
using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Scroll
{

    public class MouseScrollEventArgs : MousePositionEventArgs
    {
        public float ScrollValue { get; }

        public MouseScrollEventArgs(float scrollValue, Vector2 screenPosition, Vector2 worldPosition, IInputData input) : base(screenPosition, worldPosition, input)
        {
            ScrollValue = scrollValue;
        }

        public MouseScrollEventArgs(IInputData input)
            : this(Input.Instance.MouseScroll, Input.Instance.MouseScreenPosition, Input.Instance.MouseWorldPosition, input)
        {

        }
    }
}
