using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;

namespace ArcadeBridge
{
    public class WorkBenchSpawner: Spawner
    {
        public override void CreateObject()
        {
            base.CreateObject();

            int stage = SaveLoadService.instance.GetStage();

            TransformerDefinition def = StaticDataService.instance.GetTransformDefinitionForCar(stage);

            ObjectForInteraction.GetComponent<Transformer>().SetDefinition(def);
        }
    }
}
