using Masot.Standard.Input.EventArguments.Mouse.Position;
using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Drag
{
    public class MouseAxisDragEventArgs : MousePositionEventArgs
    {
        public Vector2 Drag { get; }

        public MouseAxisDragEventArgs(Vector2 drag, Vector2 screenPosition, Vector3 worldPosition, IInputData input) : base(screenPosition, worldPosition, input)
        {
            Drag = drag;
        }

        public MouseAxisDragEventArgs(IInputData input)
            : this(Input.Instance.MouseDrag, Input.Instance.MouseScreenPosition, Input.Instance.MouseWorldPosition, input)
        {
        }
    }
}
