using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Raycast
{
    public class MouseRaycast2DEventArgs : MouseRaycastBase
    {
        public RaycastHit2D Hit { get; }

        public MouseRaycast2DEventArgs(Camera camera, Vector2 screenPosition, Vector2 worldPosition, IInputData input, float distance = Mathf.Infinity, int layer = -1)
            : base(screenPosition, worldPosition, input)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            if (layer != -1)
            {
                Hit = Physics2D.GetRayIntersection(ray, distance, layer);
            }
            else
            {
                Hit = Physics2D.GetRayIntersection(ray, distance);
            }
        }

        public MouseRaycast2DEventArgs(IInputData input, float distance = Mathf.Infinity, int layer = -1)
            : this(Input.Instance.MainCamera, Input.Instance.MouseScreenPosition, Input.Instance.MouseWorldPosition, input, distance, layer) { }

        public MouseRaycast2DEventArgs(IInputData input)
            : this(input, Mathf.Infinity, -1)
        {
        }

        protected override bool DidRayHit()
        {
            return Hit.collider != null;
        }
    }
}
