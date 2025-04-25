using Masot.Standard.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Masot.Standard.Movement
{
    public class SmoothMovement : MonoBehaviour
    {
        private Vector3 localOffset;
        private Quaternion rotation;

        private Vector3 startOffset;
        private Quaternion startRotation;

        private IMathFunction function = null;

        public UnityEvent OnBegin = new UnityEvent();
        public UnityEvent OnUpdate = new UnityEvent();
        public UnityEvent OnEnd = new UnityEvent();

        public float smoothTime = 1f;
        public float x = 0;
        public float y = 0;
        public float m = 1;
        public MathFunction functionType;

        private void OnEnable()
        {
            function = MathMethodFactory.CreateMathFunction(functionType);
            function.Offset = new Vector2(x, y);
            function.MultiplyBy = m;
        }

        public void SetTarget(Vector3 localPositionTarget, Quaternion rotationTarget)
        {
            localOffset = localPositionTarget;
            rotation = rotationTarget;

            if (enabled)
            {
                return;
            }

            Begin();
        }

        private void Begin()
        {
            startOffset = transform.localPosition;
            startRotation = transform.rotation;

            OnBegin.Invoke();
            enabled = true;
        }

        private void Update()
        {
            smoothTime -= Time.deltaTime;
            var t = function.Evaluate(smoothTime);

            if (t > 1 || smoothTime < 0)
            {
                End();
                return;
            }

            transform.rotation = Quaternion.Lerp(startRotation, rotation, t);
            transform.localPosition = Vector3.Lerp(startOffset, localOffset, t);

            OnUpdate.Invoke();
        }

        private void End()
        {
            transform.localPosition = localOffset;
            enabled = false;

            OnEnd.Invoke();
        }
    }
}