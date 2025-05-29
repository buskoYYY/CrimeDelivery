using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 3;
    [SerializeField] private RectOffset padding;
    [SerializeField] private Vector2 spacing = new Vector2(10, 10);
    [SerializeField] private float minCellSize = 100f;

    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateGridLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateGridLayout();
    }

    public void UpdateGridLayout()
    {
        if (gridLayout == null || rectTransform == null)
            return;

        // Рассчитываем доступную ширину (ширина контейнера минус отступы)
        float availableWidth = rectTransform.rect.width - padding.left - padding.right;

        // Рассчитываем ширину ячейки с учетом промежутков между ними
        float cellWidth = (availableWidth - (columns - 1) * spacing.x) / columns;

        // Ограничиваем минимальный размер ячейки
        cellWidth = Mathf.Max(cellWidth, minCellSize);

        // Устанавливаем размеры ячеек (квадратные)
        gridLayout.cellSize = new Vector2(cellWidth, cellWidth);

        // Устанавливаем заданные параметры
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.spacing = spacing;
        gridLayout.padding = padding;
    }

    // Методы для изменения параметров во время выполнения
    public void SetColumns(int newColumns)
    {
        columns = Mathf.Max(1, newColumns);
        UpdateGridLayout();
    }

    public void SetPadding(RectOffset newPadding)
    {
        padding = newPadding;
        UpdateGridLayout();
    }

    public void SetSpacing(Vector2 newSpacing)
    {
        spacing = newSpacing;
        UpdateGridLayout();
    }

    public void SetMinCellSize(float newSize)
    {
        minCellSize = Mathf.Max(10f, newSize);
        UpdateGridLayout();
    }
}
