using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Raycast
{
    //public class MouseRaycast2DEventArgs : MouseRaycastBase
    //{
    //    public RaycastHit2D Hit { get; }

    //    public MouseRaycast2DEventArgs(Camera camera, Vector2 screenPosition, Vector2 worldPosition, IInputDefine input, float distance = Mathf.Infinity, int layer = -1)
    //        : base(screenPosition, worldPosition, input)
    //    {
    //        var ray = camera.ScreenPointToRay(screenPosition);
    //        if (layer != -1)
    //        {
    //            Hit = Physics2D.GetRayIntersection(ray, distance, layer);
    //        }
    //        else
    //        {
    //            Hit = Physics2D.GetRayIntersection(ray, distance);
    //        }
    //    }

    //    public MouseRaycast2DEventArgs(IInputDefine input, float distance = Mathf.Infinity, int layer = -1)
    //        : this(InputData.Instance.MainCamera, InputData.Instance.MouseScreenPosition, InputData.Instance.MouseScreenPosition, input, distance, layer) { }

    //    public MouseRaycast2DEventArgs(IInputDefine input)
    //        : this(input, Mathf.Infinity, -1)
    //    {
    //    }

    //    protected override bool DidRayHit()
    //    {
    //        return Hit.collider != null;
    //    }
    //}
}
