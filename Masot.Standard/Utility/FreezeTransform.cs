using UnityEngine;

namespace Masot.Standard.Utility
{
    public class FreezeTransform : MonoBehaviour
    {
        public bool freezeLocalPosition;
        public bool freezeRotation;

        private Quaternion initRot;
        private Vector3 initLocalPos;

        private void OnEnable()
        {
            Debug.Assert(transform.parent != null, $"Has to have a parent to work");

            initRot = transform.rotation;
            initLocalPos = transform.localPosition;
        }

        private void LateUpdate()
        {
            if (freezeRotation)
            {
                transform.rotation = initRot;
            }

            if (freezeLocalPosition)
            {
                transform.position = transform.parent.position + initLocalPos;
            }
        }
    }
}