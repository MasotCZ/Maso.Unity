using UnityEngine;

namespace Masot.Standard.Movement
{
    public class DirectInputMovement : InputMovementBase
    {
        protected override void OnLeft(MovementWrapper2DSettings settings)
        {
            rigidbody2D.ApplyForceVector(settings, Time.deltaTime * new Vector2(-1, 0));
        }

        protected override void OnRight(MovementWrapper2DSettings settings)
        {
            rigidbody2D.ApplyForceVector(settings, Time.deltaTime * new Vector2(1, 0));
        }

        protected override void OnUp(MovementWrapper2DSettings settings)
        {
            rigidbody2D.ApplyForceVector(settings, Time.deltaTime * new Vector2(0, 1));
        }

        protected override void OnDown(MovementWrapper2DSettings settings)
        {
            rigidbody2D.ApplyForceVector(settings, Time.deltaTime * new Vector2(0, -1));
        }
    }
}