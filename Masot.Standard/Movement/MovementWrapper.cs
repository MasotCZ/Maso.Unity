using Masot.Standard.Utility;
using UnityEngine;

namespace Masot.Standard.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class RotateWrapper : MonoBehaviour { }

    [RequireComponent(typeof(Rigidbody))]
    public class MovementWrapper : MonoBehaviour
    {
        private new Rigidbody rigidbody;

        public bool instantRotation = false;
        public float maximumVelocity = 10.0f;
        public float maximumEulerRotation = 360.0f;
        public float thrustLimit = 10.0f;
        public float speedMultiplier = 1.0f;

        public Vector3 Velocity
        {
            get => rigidbody.velocity;
            set => rigidbody.velocity = value;
        }

        //to validate
        [SerializeField]
        private float _linearDrag = 0;
        public float LinearDrag
        {
            get => _linearDrag; set
            {
                _linearDrag = value;
                rigidbody.drag = value;
            }
        }

        private void OnValidate()
        {
            if (rigidbody == null)
            {
                return;
            }

            rigidbody.drag = LinearDrag;
        }

        private void OnEnable()
        {
            rigidbody = GetComponent<Rigidbody>();
            Debug.Assert(rigidbody != null, "No assigned rigidBody");

            rigidbody.drag = LinearDrag;
        }

        public void SetContraints(RigidbodyConstraints constraints)
        {
            rigidbody.constraints = constraints;
        }

        public void Stop()
        {
            rigidbody.velocity = Vector3.zero;
        }

        public void ApplyBreakingForce()
        {
            rigidbody.drag = thrustLimit * speedMultiplier;
        }

        public void StopBreakingForce()
        {
            rigidbody.drag = 0;
        }

        //should multi be limited to 1?
        public void ApplyForceVector(Vector3 direction)
        {
            rigidbody.AddForce(direction.normalized * thrustLimit * speedMultiplier);
        }

        public void MoveByVelocityAndNullIt()
        {
            rigidbody.transform.position += transform.right * rigidbody.velocity.x + transform.up * rigidbody.velocity.y;
        }

        public void MoveInDirection(Vector3 direction)
        {
            rigidbody.velocity += direction * speedMultiplier;
            rigidbody.velocity = rigidbody.velocity.normalized * thrustLimit * speedMultiplier;
        }

        public void MoveAndRotateTowards(Vector3 target)
        {
            RotateTo(target);
            ThrustForward();
        }

        public void ThrustForward()
        {
            ApplyForceVector(rigidbody.transform.right * thrustLimit * speedMultiplier);
        }

        public void RotateTo(Vector3 target)
        {
            var basis = MathMethods.BasisFromVector(target);
            var rotation = Quaternion.LookRotation(basis.forward, basis.cross);
            RotateTo(rotation);
        }

        public void RotateTo(Quaternion rotation)
        {
            if (instantRotation)
            {
                rigidbody.rotation = rotation;
                return;
            }
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, rotation, maximumEulerRotation);
        }

        public void RotateBy(Quaternion rotation)
        {
            if (instantRotation)
            {
                rigidbody.rotation *= rotation;
                return;
            }
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, rigidbody.rotation * rotation, maximumEulerRotation);
        }
    }
}
