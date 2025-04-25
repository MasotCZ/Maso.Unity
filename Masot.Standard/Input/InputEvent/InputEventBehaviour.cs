using UnityEngine;

namespace Masot.Standard.Input.InputEvent
{
    public class InputEventBehaviour : MonoBehaviour
    {
        public AxisMode axisMode;

        private IInput? input = null;

        public E_UnityUpdateCycle updateType;

#if DEBUG
        [Header("Debug")]
        public bool LogKeyPresses = false;
#endif

        private void OnEnable()
        {
            input = Input.Instance;
            Debug.Log($"Axis mode: {axisMode}");
            Input.Instance.SetAxisMode(axisMode);
        }

        private void OnDisable()
        {
            input = null;
        }

        private void FixedUpdate()
        {
            if (updateType is not E_UnityUpdateCycle.FixedUpdate)
            {
                return;
            }

            ProcessEvents();
        }
        private void LateUpdate()
        {
            if (updateType is not E_UnityUpdateCycle.LateUpdate)
            {
                return;
            }

            ProcessEvents();
        }

        private void Update()
        {
            if (updateType is not E_UnityUpdateCycle.Update)
            {
                return;
            }

            ProcessEvents();
        }


        void OnGUI()
        {
            AddEvent(Event.current);

            if (updateType is not E_UnityUpdateCycle.OnGui)
            {
                return;
            }

            ProcessEvents();
        }

        //todo could be a problem with pressing multiple times every frame
        //exploit with scripts
        //clicking = faster than holding
        //if you click more than once every frame
        // Detects if the shift key was pressed
        private void AddEvent(Event @event)
        {
#if DEBUG
            if (LogKeyPresses && @event.keyCode != KeyCode.None)
            {
                Debug.Log($"event: {@event.type}|{@event.keyCode}");
            }
#endif
            input!.AddGuiEvent(@event);
        }

        private void ProcessEvents()
        {
            input!.ProcessEvents();
        }
    }

}
