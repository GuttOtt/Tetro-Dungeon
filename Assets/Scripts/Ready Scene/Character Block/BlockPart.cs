using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart : MonoBehaviour {
    [SerializeField] private SpriteMask _spriteMask;
    private CharacterBlock _characterBlock;

    private Vector2Int _location = new Vector2Int();

    public Cell Cell;

    public Vector2 Size { get => _spriteMask.bounds.size; }
    public CharacterBlock CharacterBlock { get => _characterBlock; }
    public Vector2Int Location { get => _location; }

    public void Init(CharacterBlock characterBlock, int frontSortingOrder, Vector2Int location) {
        _location = location;
        _characterBlock = characterBlock;
        SetSortingOrder(frontSortingOrder);
    }


    public Cell PickCell() {
        return Utils.Pick<Cell>(transform.position);
    }

    public BlockPart PickBlockPart() {
        return Utils.Pick(transform.position, this);
    }

    public void SetSortingOrder(int frontSortingOrder) {
        _spriteMask.frontSortingOrder = frontSortingOrder;
    }

    public void SetSortingLayer(int sortingLayerID) {
        _spriteMask.frontSortingLayerID = sortingLayerID;
        _spriteMask.backSortingLayerID = sortingLayerID;
    }
}
