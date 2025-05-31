using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BlockPart : MonoBehaviour
{
    [SerializeField] private SpriteMask _spriteMask;
    private CharacterBlock _characterBlock;
    [SerializeField] private Vector2Int _location = new Vector2Int();

    public Cell Cell;

    public Vector2 Size { get => _spriteMask.bounds.size; }
    public CharacterBlock CharacterBlock { get => _characterBlock; }

    public Vector2Int Location { get => _location; }

    public void Init(CharacterBlock characterBlock, int frontSortingOrder, Vector2Int location)
    {
        _location = location;
        _characterBlock = characterBlock;
        SetSortingOrder(frontSortingOrder);
    }


    public Cell PickCell()
    {
        Vector3 position = transform.position;
        Cell cell = Utils.Pick<Cell>(position);
        Debug.Log(position);
        return cell;
    }

    public BlockPart PickBlockPart()
    {
        return Utils.Pick(transform.position, this);
    }

    public void SetSortingOrder(int frontSortingOrder)
    {
        _spriteMask.frontSortingOrder = frontSortingOrder;
    }

    public void SetSortingLayer(int sortingLayerID)
    {
        _spriteMask.frontSortingLayerID = sortingLayerID;
        _spriteMask.backSortingLayerID = sortingLayerID;
    }

    public void SetLocation(Vector2Int location)
    {
        _location = location;
    }
}
