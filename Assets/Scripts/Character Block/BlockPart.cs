using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart : MonoBehaviour {
    [SerializeField] private SpriteMask _spriteMask;
    private CharacterBlock _characterBlock;
    public Cell Cell;

    public Vector2 Size { get => _spriteMask.bounds.size; }
    public CharacterBlock CharacterBlock { get => _characterBlock; }

    public void Init(CharacterBlock characterBlock, int frontSortingOrder) {
        _characterBlock = characterBlock;
        SetSortingOrder(frontSortingOrder);
    }

    public Cell PickCell() {
        return Utils.Pick<Cell>(transform.position);
    }

    public void SetSortingOrder(int frontSortingOrder) {
        _spriteMask.frontSortingOrder = frontSortingOrder;
    }

    public void SetSortingLayer(int sortingLayerID) {
        _spriteMask.frontSortingLayerID = sortingLayerID;
        _spriteMask.backSortingLayerID = sortingLayerID;
    }
}
