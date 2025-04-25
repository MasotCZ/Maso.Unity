
using UnityEngine;

namespace Masot.Standard.Movement
{
    public static class Rigidbody2DExtensions
    {
        public static void RotateTo(this Rigidbody2D rigidbody2D, Vector2 target)
        {
            rigidbody2D.RotateTo(Mathf.Atan2(target.y, target.x));
        }

        public static void RotateTo(this Rigidbody2D rigidbody2D, Vector2 target, float limit)
        {
            rigidbody2D.RotateTo(Mathf.Atan2(target.y, target.x), limit);
        }

        public static void RotateTo(this Rigidbody2D rigidbody2D, float radians)
        {
            rigidbody2D.RotateBy(radians - rigidbody2D.rotation);
        }

        public static void RotateTo(this Rigidbody2D rigidbody2D, float radians, float limit)
        {
            rigidbody2D.RotateBy(radians - rigidbody2D.rotation, Mathf.Min(radians, limit));
        }

        public static void RotateBy(this Rigidbody2D rigidbody2D, float radians)
        {
            rigidbody2D.rotation += radians;
        }

        public static void RotateBy(this Rigidbody2D rigidbody2D, float radians, float limit)
        {
            rigidbody2D.RotateBy(Mathf.Min(radians, limit));
        }
    }
}
