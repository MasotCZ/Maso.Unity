using Masot.Standard.Input.EventArguments.Mouse.Position;
using System.Collections.Generic;
using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Raycast
{
    //public abstract class MouseGraphicsRaycastEventArgsBase : MousePositionEventArgs
    //{
    //    protected MouseGraphicsRaycastEventArgsBase(Vector2 screenPosition, Vector2 worldPosition, IInputData input, EventSystem eventSystem, GraphicRaycaster raycaster)
    //        : base(screenPosition, worldPosition, input)
    //    {
    //        Hits = new List<RaycastResult>();
    //        PointerEventData = new PointerEventData(eventSystem);
    //        PointerEventData.position = screenPosition;
    //        raycaster.Raycast(PointerEventData, Hits);

    //        if (Hits.Count == 0)
    //        {
    //            DidHit = false;
    //        }
    //    }

    //    public PointerEventData PointerEventData { get; }
    //    public List<RaycastResult> Hits { get; }
    //    public bool DidHit { get; } = true;
    //}
}
