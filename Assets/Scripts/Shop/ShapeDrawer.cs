using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShapeDrawer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cellPrefab;
    [SerializeField] private Transform cellParent;
    [SerializeField] private Vector2 shapeSize;
    private Vector2 cellSize;
    private List<SpriteRenderer> cells = new List<SpriteRenderer>();
    private Vector3 cellParentOriginalScale;

    private void Awake()
    {
        cellParentOriginalScale = cellParent.localScale;
    }

    public void DrawShape(bool[,] shape)
    {
        if (cellSize == Vector2.zero)
        {
            cellSize = cellPrefab.bounds.size;
            Debug.Log($"cellSize: {cellSize}");
        }
        bool[,] trimmedShape = Trim(shape);

        int cols = trimmedShape.GetLength(0);
        int rows = trimmedShape.GetLength(1);
        int larger = Mathf.Max(cols, rows);
        float scale = shapeSize.x / larger;
        float xOrigin = cols % 2 == 0 ? -(cols / 2 - 0.5f) * cellSize.x : -(cols / 2) * cellSize.x;
        float yOrigin = rows % 2 == 0 ? (rows / 2 - 0.5f) * cellSize.y : (rows / 2) * cellSize.y;
        Vector3 origin = new Vector3(xOrigin, yOrigin, 0);

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (trimmedShape[x, y])
                {
                    SpriteRenderer cell = Instantiate(cellPrefab);
                    cell.transform.parent = cellParent;
                    cell.transform.localPosition = origin + new Vector3(x * cellSize.x, -y * cellSize.y, 0);
                    cells.Add(cell);
                }
            }
        }

        cellParent.localScale = new Vector3(scale, scale, 1);
    }

    public void Clear()
    {
        foreach (SpriteRenderer cell in cells)
        {
            Destroy(cell.gameObject);
        }
        cells.Clear();
        cellParent.localScale = cellParentOriginalScale;
    }

    private bool[,] Trim(bool[,] original)
    {
        int cols = original.GetLength(0);
        int rows = original.GetLength(1);

        int minRow = rows;
        int maxRow = -1;
        int minCol = cols;
        int maxCol = -1;

        // Find the bounds of the true values
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (original[i, j])
                {
                    minRow = Mathf.Min(minRow, j);
                    maxRow = Mathf.Max(maxRow, j);
                    minCol = Mathf.Min(minCol, i);
                    maxCol = Mathf.Max(maxCol, i);
                }
            }
        }

        // If no true values are found, return an empty array or handle as needed
        if (maxRow == -1)
        {
            return new bool[0, 0];
        }

        // Calculate the dimensions of the trimmed array
        int trimmedRows = maxRow - minRow + 1;
        int trimmedCols = maxCol - minCol + 1;

        // Create the new trimmed array
        bool[,] trimmed = new bool[trimmedCols, trimmedRows];

        // Copy the relevant portion of the original array
        for (int i = 0; i < trimmedCols; i++)
        {
            for (int j = 0; j < trimmedRows; j++)
            {
                trimmed[i, j] = original[minCol + i, minRow + j];
            }
        }

        return trimmed;
    }
}
