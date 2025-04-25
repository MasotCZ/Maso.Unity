using Masot.Standard.Utility;
using UnityEngine;

namespace Masot.Standard.Movement
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class RotateWrapper2D : MonoBehaviour { }

    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementWrapper2D : MonoBehaviour
    {
        private new Rigidbody2D rigidbody2D;

        public bool instantRotation = false;
        public float maximumVelocity = 10.0f;
        public BindableProperty<float> maxDegreeDeltaEuler = new(360.0f);
        public BindableProperty<float> maxDegreeDeltaRadians = new(Mathf.PI * 2);
        public float thrustLimit = 10.0f;
        public float speedMultiplier = 1.0f;

        public Vector3 Velocity
        {
            get => rigidbody2D.velocity;
            set => rigidbody2D.velocity = value;
        }

        //to validate
        [SerializeField]
        private float _linearDrag = 0;
        public float LinearDrag
        {
            get => _linearDrag; set
            {
                _linearDrag = value;
                rigidbody2D.drag = value;
            }
        }

        private void OnValidate()
        {
            if (rigidbody2D == null)
            {
                return;
            }

            rigidbody2D.drag = LinearDrag;
        }

        private void OnEnable()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody2D != null, "No assigned rigidBody");

            rigidbody2D.drag = LinearDrag;

            maxDegreeDeltaEuler.OnChange = OnMaxDegreeDeltaEuler;
            maxDegreeDeltaRadians.OnChange = OnMaxDegreeDeltaRadians;
        }

        private void OnMaxDegreeDeltaEuler(BindableProperty<float> newMaxDegreeDeltaEuler)
        {
            maxDegreeDeltaRadians.SetValue(newMaxDegreeDeltaEuler * Mathf.Deg2Rad);
        }

        private void OnMaxDegreeDeltaRadians(BindableProperty<float> newMaxDegreeDeltaRadians)
        {
            maxDegreeDeltaEuler.SetValue(newMaxDegreeDeltaRadians * Mathf.Rad2Deg);
        }

        public void SetContraints(RigidbodyConstraints2D constraints)
        {
            rigidbody2D.constraints = constraints;
        }

        public void Stop()
        {
            rigidbody2D.velocity = Vector3.zero;
        }

        public void ApplyBreakingForce()
        {
            rigidbody2D.drag = thrustLimit * speedMultiplier;
        }

        public void StopBreakingForce()
        {
            rigidbody2D.drag = 0;
        }

        //should multi be limited to 1?
        public void ApplyForceVector(Vector3 direction)
        {
            rigidbody2D.AddForce(direction.normalized * thrustLimit * speedMultiplier);
        }

        public void MoveByVelocityAndNullIt()
        {
            rigidbody2D.transform.position += transform.right * rigidbody2D.velocity.x + transform.up * rigidbody2D.velocity.y;
        }

        public void MoveInDirection(Vector2 direction)
        {
            rigidbody2D.velocity += direction * speedMultiplier;
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * thrustLimit * speedMultiplier;
        }

        public void MoveAndRotateTowards(Vector2 target)
        {
            RotateTo(target);
            ThrustForward();
        }

        public void ThrustForward()
        {
            ApplyForceVector(rigidbody2D.transform.right * thrustLimit * speedMultiplier);
        }

        public void RotateTo(Vector2 target)
        {
            rigidbody2D.RotateTo(target);
        }

        public void RotateTo(float radians)
        {
            if (instantRotation)
            {
                rigidbody2D.RotateTo(radians);
                return;
            }

            rigidbody2D.RotateTo(radians, maxDegreeDeltaRadians);
        }

        public void RotateBy(float radians)
        {
            if (instantRotation)
            {
                rigidbody2D.RotateBy(radians);
                return;
            }

            rigidbody2D.RotateBy(radians, maxDegreeDeltaRadians);
        }
    }
}
