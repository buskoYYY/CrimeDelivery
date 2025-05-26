using ArcadeBridge;
using UnityEngine;

public class RenderButtonSelectionManager : MonoBehaviour
{
    private RenderButtonItem selectedItem;
    [SerializeField] private Car3DPreviewRenderer previewRenderer;

    public void OnButtonClicked(RenderButtonItem item, GameObject carPrefab, int carIndex)
    {
        if (selectedItem != null && selectedItem != item)
        {
            selectedItem.SetSelected(false);
        }

        selectedItem = item;
        selectedItem.SetSelected(true);

        // Вызываем отображение 3D превью
        previewRenderer.PreviewCar();
        previewRenderer.ShowCarPreview(carIndex);
        previewRenderer.UpdateCarStatsUI();
    }
}

