using UnityEngine;

namespace Masot.Standard.Utility
{
    public interface IReceiver
    {
        void Notify(object sender);
    }

    public class NotifyOnDeath : MonoBehaviour
    {
        public IReceiver receiver;

        private void OnDestroy()
        {
            receiver?.Notify(this);
        }
    }
}
