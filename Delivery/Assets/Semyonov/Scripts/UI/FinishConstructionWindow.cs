namespace ArcadeBridge
{
    public class FinishConstructionWindow: Window
    {

        private void Start()
        {
            ViewCunstructedCar view = GetComponent<ViewCunstructedCar>();

            view.PreviewCar();
            view.ShowCarPreview(SaveLoadService.instance.GetLastOpenedIndexCar());
            view.UpdateCarStatsUI();
        }

    }
}
