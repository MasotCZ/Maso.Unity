using System.Collections.Generic;
using UnityEngine;

namespace Masot.Standard.Utility
{

    public class UpdateToken
    {
        public readonly IUpdate updateAble;
        public float timeBetweenUpdates;
        public float lastUpdate;
        public bool paused;

        public UpdateToken(IUpdate updateAble, float timeBetweenUpdates = 0, bool paused = false, float lastUpdate = 0)
        {
            this.updateAble = updateAble;
            this.timeBetweenUpdates = timeBetweenUpdates;
            this.lastUpdate = lastUpdate;
            this.paused = paused;
        }
    }

    public abstract class UpdaterBase : MonoBehaviour
    {
        public Dictionary<IUpdate, UpdateToken> updateList = new Dictionary<IUpdate, UpdateToken>();
        private CommandBuffer commandBuffer = new CommandBuffer();

        private static GameObject _updaterObject;
        protected static GameObject updaterObject
        {
            get
            {
                if (_updaterObject is null)
                {
                    _updaterObject = new GameObject("Updater");
                }

                return _updaterObject;
            }
        }

        public bool Register(UpdateToken toAdd)
        {
            commandBuffer.Add(new DictionaryAddCommand<IUpdate, UpdateToken>(updateList, toAdd.updateAble, toAdd));
            return true;
        }

        public bool Remove(IUpdate toRemove)
        {
            commandBuffer.Add(new DictionaryRemoveCommand<IUpdate, UpdateToken>(updateList, toRemove));
            return true;
        }

        protected virtual void StartUpdate()
        {
            foreach (var obj in updateList.Values)
            {
                if (obj.paused)
                {
                    continue;
                }

                if (obj.lastUpdate + obj.timeBetweenUpdates > Time.realtimeSinceStartup)
                {
                    continue;
                }

                obj.lastUpdate = Time.realtimeSinceStartup;
                obj.updateAble.Update(this);
            }
        }

        protected virtual void FinishUpdate()
        {
            commandBuffer.Process();
        }

        protected virtual void ProcessUpdate()
        {
            StartUpdate();
            FinishUpdate();
        }
    }

    public class Updater : UpdaterBase
    {

        private static Updater _instance;
        public static Updater Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = updaterObject.AddComponent<Updater>();
                }

                return _instance;
            }
        }

        private void Update()
        {
            ProcessUpdate();
        }
    }

    public class FixedUpdater : UpdaterBase
    {
        private static FixedUpdater _instance;
        public static FixedUpdater Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = updaterObject.AddComponent<FixedUpdater>();
                }

                return _instance;
            }
        }

        private void FixedUpdate()
        {
            ProcessUpdate();
        }
    }

    public class LateUpdater : UpdaterBase
    {
        private static LateUpdater _instance;
        public static LateUpdater Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = updaterObject.AddComponent<LateUpdater>();
                }

                return _instance;
            }
        }

        private void LateUpdate()
        {
            ProcessUpdate();
        }
    }

    public interface IUpdate
    {
        void StartUpdate();
        void StopUpdate();
        void Update(UpdaterBase updater);
    }

    public abstract class UpdateBaseBase : IUpdate
    {
        protected UpdateToken updateToken;

        protected UpdateBaseBase(float timeBetweenUpdates, bool paused, float lastUpdate)
        {
            updateToken = new UpdateToken(this, timeBetweenUpdates, paused, lastUpdate);
        }

        public virtual void StartUpdate()
        {
            updateToken.paused = false;
        }

        public virtual void StopUpdate()
        {
            updateToken.paused = true;
        }

        public abstract void Update(UpdaterBase updater);
    }

    public abstract class UpdateBase : UpdateBaseBase
    {
        protected UpdateBase(float timeBetweenUpdates = 0, bool paused = false, float lastUpdate = 0) : base(timeBetweenUpdates, paused, lastUpdate)
        {
            Updater.Instance.Register(updateToken);
        }

        ~UpdateBase()
        {
            Updater.Instance.Remove(this);
        }
    }

    public abstract class FixedUpdateBase : UpdateBaseBase
    {
        public FixedUpdateBase(float timeBetweenUpdates = 0, bool paused = false, float lastUpdate = 0) : base(timeBetweenUpdates, paused, lastUpdate)
        {
            FixedUpdater.Instance.Register(updateToken);
        }

        ~FixedUpdateBase()
        {
            FixedUpdater.Instance.Remove(this);
        }
    }

    public abstract class LateUpdateBase : UpdateBaseBase
    {
        public LateUpdateBase(float timeBetweenUpdates = 0, bool paused = false, float lastUpdate = 0) : base(timeBetweenUpdates, paused, lastUpdate)
        {
            LateUpdater.Instance.Register(updateToken);
        }

        ~LateUpdateBase()
        {
            LateUpdater.Instance.Remove(this);
        }
    }
}