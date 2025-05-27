using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart_Equipment : MonoBehaviour {
    private Vector2Int _location;
    private Equipment _equipment;

    [SerializeField] private SpriteMask _spriteMask;
    [SerializeField] private SpriteRenderer _marker;

    public Vector2 Size { get => _spriteMask.bounds.size; }
    public Equipment Equipment { get => _equipment; }

    public void Init(Equipment equipment) {
        int equipmentLayerID = SortingLayer.NameToID("Equipment");
        _spriteMask.sortingLayerID = equipmentLayerID;
        _equipment = equipment;
    }


    public BlockPart PickBlockPart() {
        return Utils.Pick<BlockPart>(transform.position);
    }
    
    public BlockPart_Equipment PickBlockPartEquipment() {
        return Utils.Pick<BlockPart_Equipment>(transform.position, this);
    }

    public void SetMarkerOn(bool isOn)
    {
        _marker.enabled = isOn;
    }

}
