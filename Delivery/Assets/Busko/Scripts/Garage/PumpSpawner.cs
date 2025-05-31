using System;

namespace ArcadeBridge
{
    public class PumpSpawner: Spawner {

        public event Action OnWheelsPumped;

        public override void CreateObject()
        {
            base.CreateObject();

            (ObjectForInteraction as Pump).OnWheelsPumped += WheelsPumpedInvoke;
        }

        private void WheelsPumpedInvoke()
        {
            OnWheelsPumped?.Invoke();
        }

        private void OnDestroy()
        {
            if(ObjectForInteraction)
                (ObjectForInteraction as Pump).OnWheelsPumped -= WheelsPumpedInvoke;
        }
    }
}
