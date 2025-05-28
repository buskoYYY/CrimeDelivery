using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 3;
    [SerializeField] private float horizontalPadding = 20f;
    [SerializeField] private float verticalPadding = 20f; // Новый параметр для вертикальных отступов
    [SerializeField] private float spacing = 15f;
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        CalculateGridLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        CalculateGridLayout();
    }

    public void CalculateGridLayout()
    {
        if (scrollRect == null || scrollRect.viewport == null)
            return;

        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        RectTransform viewport = scrollRect.viewport;

        // Рассчитываем доступную ширину с учетом отступов и промежутков
        float availableWidth = viewport.rect.width - (2 * horizontalPadding) - ((columns - 1) * spacing);
        float buttonSize = availableWidth / columns;

        // Настраиваем GridLayoutGroup
        grid.cellSize = new Vector2(buttonSize, buttonSize); // Квадратные кнопки
        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset(
            Mathf.RoundToInt(horizontalPadding),
            Mathf.RoundToInt(horizontalPadding),
            Mathf.RoundToInt(verticalPadding), // Верхний отступ
            Mathf.RoundToInt(verticalPadding)  // Нижний отступ
        );
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }
}
