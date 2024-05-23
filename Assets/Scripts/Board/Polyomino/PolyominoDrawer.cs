using System.Collections.Generic;
using UnityEngine;

public class PolyominoDrawer : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;
    private List<GameObject> drawnBlocks = new List<GameObject>();
    private int[,] _shape;
    private Vector2 _blockSize;

    public List<GameObject> GetBlocks { get => drawnBlocks; }
    public int[,] GetShape { get => _shape; }
    public Vector2 GetBlockSize {
        get {
            if (_blockSize == Vector2.zero) _blockSize = blockPrefab.GetComponent<SpriteRenderer>().bounds.size;
            return _blockSize;
        }
    }


    public void Draw(int[,] shape) {
        //Top Left 계산
        Vector2 blockSize = GetBlockSize;
        int col = shape.GetLength(0);
        int row = shape.GetLength(1);
        Vector2 topLeft = new Vector2();

        //Top Left x
        if (col % 2 == 0) {
            topLeft.x = -blockSize.x * (col / 2 - 0.5f);
        }
        else {
            topLeft.x = -blockSize.x * (col / 2);
        }
        //Top Left y
        if (row % 2 == 0) {
            topLeft.y = blockSize.y * (row / 2 - 0.5f);
        }
        else {
            topLeft.y = blockSize.y * (row / 2);
        }

        //Clear Drawn Blocks
        ClearBlocks();

        //Draw
        for (int x = 0; x < col; x++) {
            for (int y = 0; y < row; y++) {
                if (shape[x, y] == 1) {
                    GameObject block = Instantiate(blockPrefab, transform);
                    block.transform.localPosition =
                        topLeft + new Vector2(blockSize.x * x, -blockSize.y * y);
                    drawnBlocks.Add(block);
                }
            }
        }

        //Shape 저장
        _shape = shape;
    }

    public void ClearBlocks() {
        foreach (GameObject block in drawnBlocks) {
            Destroy(block);
        }
        drawnBlocks.Clear();
    }
}
