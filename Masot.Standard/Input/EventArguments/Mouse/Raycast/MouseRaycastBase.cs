using Masot.Standard.Input.EventArguments.Mouse.Position;
using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Raycast
{
    public abstract class MouseRaycastBase : MousePositionEventArgs
    {
        public bool DidHit => DidRayHit();

        public MouseRaycastBase(IInputData input) : base(input)
        {
        }

        public MouseRaycastBase(Vector2 screenPosition, Vector2 worldPosition, IInputData input) : base(screenPosition, worldPosition, input)
        {
        }

        protected abstract bool DidRayHit();
    }
}
