namespace ArcadeBridge
{
    public class FinishConstructionWindow: Window
    {

        private void Start()
        {
            GetComponent<ViewCunstructedCar>().PreviewCar();
            GetComponent<ViewCunstructedCar>().ShowCarPreview(SaveLoadService.instance.GetLastOpenedIndexCar());
        }

    }
}
