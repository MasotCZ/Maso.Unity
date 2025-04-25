using UnityEngine;

namespace Masot.Standard.Utility
{
    //todo
    //make speed scale with object size
    //make speed scale with how far the object is also
    //scale zoom by size too ?

    //todo on snap move the camera so that the snapped object is in the middle of the screen ?
    //so look at the size and scale the distnace from object proportionaly
    public class TransformInfo
    {
        public readonly Transform parent;
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public TransformInfo(Transform parent, Vector3 position, Quaternion rotation)
        {
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
        }
    }
}