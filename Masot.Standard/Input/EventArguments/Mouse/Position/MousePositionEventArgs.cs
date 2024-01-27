﻿using UnityEngine;

namespace Masot.Standard.Input.EventArguments.Mouse.Position
{
    public class MousePositionEventArgs : EventArgsBase
    {
        public Vector2 ScreenPosition { get; }
        public Vector2 WorldPosition { get; }

        public MousePositionEventArgs(Vector2 screenPosition, Vector2 worldPosition, IInputData input) : base(input)
        {
            ScreenPosition = screenPosition;
            WorldPosition = worldPosition;
        }

        public MousePositionEventArgs(IInputData input) : this(Input.Instance.MouseScreenPosition, Input.Instance.MouseWorldPosition, input)
        {
        }
    }
}
