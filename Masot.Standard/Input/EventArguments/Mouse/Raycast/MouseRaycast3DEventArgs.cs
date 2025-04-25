using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Raycast
{
    public class MouseRaycast3DEventArgs : MouseRaycastBase
    {
        private RaycastHit _hit;
        public RaycastHit Hit => _hit;

        public MouseRaycast3DEventArgs(Camera camera, Vector2 screenPosition, Vector2 worldPosition, IInputData input, float distance = Mathf.Infinity, int layer = -1)
            : base(screenPosition, worldPosition, input)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            if (layer != -1)
            {
                Physics.Raycast(ray.origin, ray.direction, out _hit, distance, layer);
            }
            else
            {
                Physics.Raycast(ray.origin, ray.direction, out _hit, distance);
            }
        }

        public MouseRaycast3DEventArgs(IInputData input, float distance = Mathf.Infinity, int layer = -1)
            : this(Input.Instance.MainCamera, Input.Instance.MouseScreenPosition, Input.Instance.MouseWorldPosition, input, distance, layer) { }

        public MouseRaycast3DEventArgs(IInputData input)
            : this(input, Mathf.Infinity, -1)
        {
        }

        protected override bool DidRayHit()
        {
            return Hit.collider != null;
        }
    }
}
