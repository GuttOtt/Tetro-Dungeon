using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart : MonoBehaviour {
    [SerializeField] private SpriteMask _spriteMask;

    public Vector2 Size { get => _spriteMask.bounds.size; }

    public Cell GetCellUnder() {
        return Utils.Pick<Cell>(transform.position);
    }

    public void SetSortingOrder(int frontSortingOrder) {
        _spriteMask.frontSortingOrder = frontSortingOrder;
    }
}
