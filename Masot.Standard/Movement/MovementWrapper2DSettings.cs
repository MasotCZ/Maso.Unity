using UnityEngine;

namespace Masot.Standard.Movement
{

    [CreateAssetMenu(fileName = "MovementWrapper2D", menuName = "Movement/Movement2D")]
    public class MovementWrapper2DSettings : ScriptableObject
    {
        public float maximumVelocity = 10.0f;
        public float thrustLimit = 10.0f;
        public float speedMultiplier = 1.0f;
    }

    public static class MovementWrapper2DExtension
    {
        public static void SetContraints(this Rigidbody2D rigidbody2D, RigidbodyConstraints2D constraints)
        {
            rigidbody2D.constraints = constraints;
        }

        public static void Stop(this Rigidbody2D rigidbody2D)
        {
            rigidbody2D.velocity = Vector2.zero;
        }

        public static void ApplyBreakingForce(this Rigidbody2D rigidbody2D, MovementWrapper2DSettings settings)
        {
            rigidbody2D.drag = settings.thrustLimit * settings.speedMultiplier;
        }

        public static void StopBreakingForce(this Rigidbody2D rigidbody2D)
        {
            rigidbody2D.drag = 0;
        }

        //should multi be limited to 1?
        public static void ApplyForceVector(this Rigidbody2D rigidbody2D, MovementWrapper2DSettings settings, Vector2 direction)
        {
            rigidbody2D.AddForce(direction.normalized * settings.thrustLimit * settings.speedMultiplier);
        }

        public static void MoveByVelocityAndNullIt(this Rigidbody2D rigidbody2D)
        {
            rigidbody2D.transform.position += rigidbody2D.transform.right * rigidbody2D.velocity.x + rigidbody2D.transform.up * rigidbody2D.velocity.y;
        }

        public static void MoveInDirection(this Rigidbody2D rigidbody2D, MovementWrapper2DSettings settings, Vector2 direction)
        {
            rigidbody2D.velocity += direction * settings.speedMultiplier;
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * settings.thrustLimit * settings.speedMultiplier;
        }

        public static void ThrustForward(this Rigidbody2D rigidbody2D, MovementWrapper2DSettings settings)
        {
            rigidbody2D.ApplyForceVector(settings, rigidbody2D.transform.right * settings.thrustLimit * settings.speedMultiplier);
        }
    }
}
