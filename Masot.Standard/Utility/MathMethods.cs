using UnityEngine;
using System;

namespace Masot.Standard.Utility
{
    public interface IMathFunction
    {
        float MultiplyBy { get; set; }
        Vector2 Offset { get; set; }
        float Evaluate(float x);
    }

    public abstract class MathFunctionBase : IMathFunction
    {
        public float MultiplyBy { get; set; } = 1;
        public Vector2 Offset { get; set; } = Vector2.zero;

        public float Evaluate(float x)
        {
            return MultiplyBy * _Evaluate(x + Offset.x) + Offset.y;
        }

        protected abstract float _Evaluate(float x);
    }

    public class LinearFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return x;
        }
    }

    public class SquareFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return x * x;
        }
    }

    public class CubeFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return x * x * x;
        }
    }

    public class SquareRootFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Sqrt(x);
        }
    }

    public class AbsoluteValueFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Abs(x);
        }
    }
    public class ReciprocalFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return 1 / x;
        }
    }
    public class LogarithmicFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Log(x);
        }
    }
    public class ExponentialFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Exp(x);
        }
    }
    public class FloorFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Floor(x);
        }
    }
    public class CeilingFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Ceil(x);
        }
    }
    public class SineFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Sin(x);
        }
    }
    public class CosineFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Cos(x);
        }
    }
    public class TangentFunction : MathFunctionBase
    {
        protected override float _Evaluate(float x)
        {
            return Mathf.Tan(x);
        }
    }

    public enum MathFunction
    {
        Linear,
        Square,
        Cube,
        Root,
        Absolute,
        Reciprocal,
        Logarithmic,
        Exponential,
        Floor,
        Ceil,
        Sine,
        Cosine,
        Tangent
    }

    public static class MathMethodFactory
    {
        public static IMathFunction CreateMathFunction(MathFunction function)
        {
            return function switch
            {
                MathFunction.Linear => new LinearFunction(),
                MathFunction.Square => new SquareFunction(),
                MathFunction.Cube => new CubeFunction(),
                MathFunction.Root => new SquareRootFunction(),
                MathFunction.Absolute => new AbsoluteValueFunction(),
                MathFunction.Reciprocal => new ReciprocalFunction(),
                MathFunction.Logarithmic => new LogarithmicFunction(),
                MathFunction.Exponential => new ExponentialFunction(),
                MathFunction.Floor => new FloorFunction(),
                MathFunction.Ceil => new CeilingFunction(),
                MathFunction.Sine => new SineFunction(),
                MathFunction.Cosine => new CosineFunction(),
                MathFunction.Tangent => new TangentFunction(),
                _ => null,
            };
        }
    }

    public static class MathMethods
    {
        // axis must be normalized
        public class Basis
        {
            public readonly Vector3 forward;
            public readonly Vector3 perp;
            public readonly Vector3 cross;

            public Basis(Vector3 forward, Vector3 perp, Vector3 cross)
            {
                this.forward = forward;
                this.perp = perp;
                this.cross = cross;
            }
        }

        public static Basis BasisFromVector(in Vector3 axis)
        {
            var perp = Vector3.Cross(axis, Vector3.right);
            if (perp.sqrMagnitude < 1E-9f) perp = Vector3.Cross(axis, Vector3.forward);
            perp.Normalize();
            return new Basis(axis, perp, Vector3.Cross(axis, perp));
        }

        public static bool FloatEqual(this float a, float b)
        {
            return Mathf.Abs((a - b) / b) < Epsilon;
        }

        public static bool NearlyEqual(double a, double b, double epsilon = double.Epsilon)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static bool NearlyEqual(float a, float b, float epsilon = float.Epsilon)
        {
            const float MinNormal = 1.401298E-40f;
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static float Epsilon = 0.000001f;
        public static float PI2 = Mathf.PI * 2;

        public static bool IsBetweenAngle(float a, float b, float x)
        {
            if (a > b)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            return a < x && x < b;
        }

        public static Vector2 ScaleToMagnitude2D(float magnitude, Vector2 direction)
        {
            float mag = direction.magnitude;
            return direction * (magnitude / mag);
        }
        public static Vector3[] CreateEllipsePoints(float radiusX, float radiusY, int n, bool loop = true)
        {
            return CreateEllipsePoints(radiusX, radiusY, n, Vector3.zero, loop);
        }

        public static Vector3[] CreateEllipsePoints(float radiusX, float radiusY, int n, Vector3 offset, bool loop = true)
        {
            if (loop)
            {
                n++;
            }

            Vector3[] points = new Vector3[n];

            if (loop)
            {
                n--;
            }

            float angle = 360f / n;

            for (int i = 0; i < n; i++)
            {
                points[i] = EllipsePointEuler(radiusX, radiusY, angle) + offset;
                angle += 360f / n;
            }

            if (loop)
            {
                points[n] = points[0];
            }

            return points;
        }

        public static Vector2 EllipsePoint2D(float radiusX, float radiusY, float radians)
        {
            float y = Mathf.Sin(radians) * radiusY;
            float x = Mathf.Cos(radians) * radiusX;

            return new Vector2(x, y);
        }

        public static Vector2 EllipsePoint2DEuler(float radiusX, float radiusY, float euler)
        {
            return EllipsePoint2D(radiusX, radiusY, euler * Mathf.Deg2Rad);
        }

        public static Vector3 EllipsePoint(float radiusX, float radiusY, float radians)
        {
            float y = Mathf.Sin(radians) * radiusY;
            float x = Mathf.Cos(radians) * radiusX;

            return new Vector3(x, y, 0);
        }

        public static Vector3 EllipsePointEuler(float radiusX, float radiusY, float euler)
        {
            return EllipsePoint(radiusX, radiusY, euler * Mathf.Deg2Rad);
        }

        // Determines if the lines AB and CD intersect.
        public static bool WillABIntersectCD(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out Vector2 intersectPoint)
        {
            intersectPoint = Vector2.zero;

            Vector2 P = new Vector2(B.x - A.x, B.y - A.y);
            Vector2 Q = new Vector2(D.x - C.x, D.y - C.y);
            Vector2 NNP = new Vector2(-P.y, P.x).normalized;

            float fp = Vector2.Dot(Q.normalized, NNP);

            if (fp == 0)
            {
                return false;
            }

            float h = Vector2.Dot(new Vector2(A.x - C.x, A.y - C.y).normalized, NNP) / fp;

            if (h > 1.0f || h < 0.0f)
            {
                return false;
            }

            intersectPoint = C + Q * h;
            return true;
        }

        public static Vector3[] DistributePointsBetweenTwoPoints(int numberOfPoints, Vector3 from, Vector3 to)
        {
            Vector3[] ret = new Vector3[numberOfPoints];

            Vector3 step = new Vector3(Mathf.Abs(from.x - to.x), Mathf.Abs(from.y - to.y), Mathf.Abs(from.z - to.z)) / numberOfPoints;
            Vector3 currPoint = from;
            for (int i = 0; i < numberOfPoints; i++)
            {
                currPoint += step;
                ret[i] = currPoint;
            }

            return ret;
        }

        public static Vector2 Rotate2D(float deltaEuler, Vector2 v)
        {
            deltaEuler *= Mathf.Deg2Rad;
            float sin = Mathf.Sin(deltaEuler);
            float cos = Mathf.Cos(deltaEuler);
            return Rotate2D(v, sin, cos);
        }

        public static Vector2[] Rotate2D(float deltaEuler, params Vector2[] vs)
        {
            deltaEuler *= Mathf.Deg2Rad;
            float sin = Mathf.Sin(deltaEuler);
            float cos = Mathf.Cos(deltaEuler);
            Vector2[] ret = new Vector2[vs.Length];

            for (int i = 0; i < vs.Length; i++)
            {
                ret[i] = Rotate2D(vs[i], sin, cos);
            }

            return ret;
        }

        private static Vector2 Rotate2D(Vector2 v, float sin, float cos)
        {
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public static Vector3[] DistributePointsAroundAPoint2D(int numberOfPoints)
        {
            Vector3[] ret = new Vector3[numberOfPoints];

            float step = (2 * Mathf.PI) / numberOfPoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float currentRadian = i * step;
                float y = Mathf.Sin(currentRadian);
                float x = Mathf.Cos(currentRadian);
                ret[i] = new Vector3(x, y, 0);
            }

            return ret;
        }

        public static Vector3[] DistributePointsAroundAPoint2D(int numberOfPoints, Vector3 offset)
        {
            Vector3[] ret = new Vector3[numberOfPoints];

            float step = (2 * Mathf.PI) / numberOfPoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float currentRadian = i * step;
                float y = Mathf.Sin(currentRadian);
                float x = Mathf.Cos(currentRadian);
                ret[i] = new Vector3(x, y, 0) + offset;
            }

            return ret;
        }

        public static Vector2 MiddlePoint(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + ((b.x - a.x) / 2), a.y + ((b.y - a.y) / 2));
        }

        public static Vector2 PointAtAngle(float radian, float distance)
        {
            return DirectionFromAngle2D(radian) * distance;
        }

        public static Vector2 PointAtDirection2D(Vector2 dir, float distance)
        {
            return dir.normalized * distance;
        }

        public static float Distance2D(Vector2 a, Vector2 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return Mathf.Sqrt(x * x + y * y);
        }

        public static float Distance2D(Vector2 x)
        {
            return Distance2D(Vector2.zero, x);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            float z = a.z - b.z;
            return Mathf.Sqrt(x * x + y * y + z * z);
        }

        public static float Distance(Vector3 x)
        {
            return Distance(Vector3.zero, x);
        }

        public static _T Min<_T>(_T a, _T b) where _T : IComparable
        {
            if (a.CompareTo(b) < 0)
            {
                return a;
            }
            return b;
        }

        public static _T Max<_T>(_T a, _T b) where _T : IComparable
        {
            if (a.CompareTo(b) > 0)
            {
                return a;
            }
            return b;
        }

        public static _T Clamp<_T>(_T min, _T max, _T val) where _T : IComparable
        {
            return Max(Min(val, max), min);
        }

        public static float AngleRadians2D(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x);
            if (angle < 0)
            {
                return (Mathf.PI * 2) + angle;
            }
            return angle;
        }

        public static float AngleRadians2D(Vector2 from, Vector2 to)
        {
            float x = to.x - from.x;
            float y = to.y - from.y;
            float angle = Mathf.Atan2(y, x);
            if (angle < 0)
            {
                return (Mathf.PI * 2) + angle;
            }
            return angle;
        }

        public static float AngleEuler2D(Vector2 from, Vector2 to)
        {
            var v = to - from;
            return AngleEuler2D(v.normalized);
        }

        public static float AngleEuler2D(Vector2 direction)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
            if (angle < 0)
            {
                return 360 + angle;
            }
            return angle;
        }

        public static void RotateObjectByAngles(GameObject gameObject, float angle, Vector3 axis)
        {
            gameObject.transform.rotation = Quaternion.AngleAxis(angle, axis);
        }

        public static void RotateObjectByAngles(GameObject gameObject, float angle, Vector3 axis, Vector2 point)
        {
            gameObject.transform.rotation = Quaternion.AngleAxis(angle, axis);
        }

        public static bool CastRayOnScreen(Camera camera, out RaycastHit hit, Vector2 position)
        {
            Ray ray = camera.ScreenPointToRay(position);
            return Physics.Raycast(ray, out hit);
        }

        public static Vector3 Direction(Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static Vector2 Direction2D(Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        public static bool CastRay2D(out RaycastHit2D hit, Vector2 origin, Vector2 direction, float distance = float.MaxValue)
        {
            hit = Physics2D.Raycast(origin, direction, distance);
            return hit.collider != null;
        }

        public static bool CastRay(out RaycastHit hit, Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            return Physics.Raycast(ray, out hit);
        }

        public static bool CastRayFromPointToPoint(out RaycastHit hit, Vector3 from, Vector3 to)
        {
            Ray ray = new Ray(from, Direction(from, to));
            return Physics.Raycast(ray, out hit, Distance(from, to) * 2);
        }

        public static Vector3 DirectionFromAngle(Vector3 radians)
        {
            return new Vector3(
               Mathf.Cos(radians.z) + Mathf.Cos(radians.y),
               Mathf.Sin(radians.z) + Mathf.Sin(radians.x),
               Mathf.Cos(radians.x) - Mathf.Sin(radians.y)
               ).normalized;
        }

        public static Vector2 DirectionFromAngleEuler2D(float euler)
        {
            return DirectionFromAngle2D(euler * Mathf.Deg2Rad);
        }

        public static Vector2 DirectionFromAngle2D(float radian)
        {
            return new Vector2(
                    Mathf.Cos(radian),
                    Mathf.Sin(radian)
                );
        }

        public static bool CastRayFromPointToPoint2D(out RaycastHit2D hit, Vector2 from, Vector2 to)
        {
            float distance = Distance2D(from, to);
            hit = Physics2D.Raycast(from, Direction(from, to), distance);
            return hit.distance <= distance;
        }

    }

}