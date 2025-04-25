using Masot.Standard.Utility;
using UnityEngine;

namespace Masot.Standard.Movement
{
    [CreateAssetMenu(fileName = "RotateWrapper2D", menuName = "Movement/Rotation2D")]
    public class RotateWrapper2DSettings : ScriptableObject
    {
        public float speedMultiplier = 1.0f;
        public bool instantRotation = false;
        public BindableProperty<float> maxDegreeDeltaEuler = new(360.0f);
        public BindableProperty<float> maxDegreeDeltaRadians = new(Mathf.PI * 2);
    }

    public static class RotateWrapper2DExtension
    {
        //---new
        public static void RotateBy(this Transform transform, RotateWrapper2DSettings settings, float deltaRadian)
        {
            if (settings.instantRotation)
            {
                transform._RotateBy(deltaRadian);
            }

            transform._RotateBy(settings, deltaRadian);
        }

        public static void RotateTo(this Transform transform, RotateWrapper2DSettings settings, float deltaRadian)
        {
            if (settings.instantRotation)
            {
                transform._RotateTo(deltaRadian);
            }

            var delta = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, deltaRadian);
            transform._RotateBy(settings, delta);
        }

        public static void RotateTo(this Transform transform, RotateWrapper2DSettings settings, Vector2 target)
        {
            var direction = MathMethods.Direction2D(transform.position, target);

            if (settings.instantRotation)
            {
                _RotateTo(transform, direction);
                return;
            }

            var delta = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, MathMethods.AngleEuler2D(direction));
            transform._RotateBy(settings, delta);
        }

        private static void _RotateBy(this Transform transform, RotateWrapper2DSettings settings, float deltaRadian)
        {
            var maxRotation = settings.maxDegreeDeltaRadians * settings.speedMultiplier;
            deltaRadian = Mathf.Min(Mathf.Max(deltaRadian, -maxRotation), maxRotation);
            transform._RotateBy(deltaRadian);
        }

        private static void _RotateBy(this Transform transform, float radianDelta)
        {
            // Create a quaternion for the rotation around the Z-axis using radians
            // Apply the rotation
            transform.rotation *= new Quaternion(0, 0, Mathf.Sin(radianDelta / 2), Mathf.Cos(radianDelta / 2));
        }


        private static void _RotateTo(this Transform transform, Vector2 direction)
        {
            transform.right = direction;
        }

        private static void _RotateTo(this Transform transform, float radian)
        {
            // Create a quaternion for the rotation around the Z-axis using radians
            // Set the rotation
            transform.rotation = new Quaternion(0, 0, Mathf.Sin(radian / 2), Mathf.Cos(radian / 2));
        }

    }
}
