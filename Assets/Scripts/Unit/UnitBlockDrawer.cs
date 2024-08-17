using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class UnitBlockDrawer : MonoBehaviour
{
    [SerializeField]
    private PolyominoDrawer _polyominoDrawer;
    [SerializeField]
    private UnitDrawer _unitDrawerPrefab;
    private List<UnitDrawer> _unitDrawers = new List<UnitDrawer>();

    public void Draw(Polyomino polyomino, UnitConfig unitConfig) {
        //폴리오미노를 먼저 그림
        _polyominoDrawer?.Draw(polyomino.Shape);

        //그 후 폴리오미노의 각 블럭 위에 UnitDrawer를 배치
        List<GameObject> blocks = _polyominoDrawer.GetBlocks;
        foreach (GameObject block in blocks) {
            UnitDrawer unitDrawer = Instantiate(_unitDrawerPrefab, block.transform);
            unitDrawer.Draw(unitConfig);
            _unitDrawers.Add(unitDrawer);
        }
    }
    
    public List<GameObject> DrawBlock(Polyomino polyomino, Transform tr)
    {
        return  _polyominoDrawer?.Draw(polyomino.Shape, tr);
    }

    public void Clear() {
        _polyominoDrawer.ClearBlocks();
        foreach (UnitDrawer unitDrawer in _unitDrawers) {
            if (unitDrawer != null && unitDrawer.gameObject != null) {
                Destroy(unitDrawer.gameObject);
            }
        }
        _unitDrawers.Clear();
    }

    public Vector3 GetTopLeftPosition() {
        //가장 좌상단의 포지션을 반환
        int[,] shape = _polyominoDrawer.GetShape;
        Vector2 blockSize = _polyominoDrawer.GetBlockSize;
        Vector3 topLeft = Vector2.zero;
        int col = shape.GetLength(0);
        int row = shape.GetLength(1);

        //Top Left x
        if (shape.GetLength(0) % 2 == 0) {
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

        return topLeft + transform.position;
    }
}
