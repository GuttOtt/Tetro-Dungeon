using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour {
    [SerializeField] private BoxCollider2D _area;
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();
    private List<Equipment> _equipments = new List<Equipment>();

    public void Add(CharacterBlock characterBlock) {
        _characterBlocks.Add(characterBlock);
        //characterBlock.transform.SetParent(_area.transform);
        ArrangeTransform(characterBlock.transform);
    }

    public void Remove(CharacterBlock characterBlock) {
        _characterBlocks.Remove(characterBlock);
    }

    public void Add(Equipment equipment) {
        _equipments.Add(equipment);
        //equipment.transform.SetParent(_area.transform);
        ArrangeTransform(equipment.transform);
    }

    private void ArrangeTransform(Transform itemTransform) {
        Vector3 position = itemTransform.position;
        Bounds areaBounds = _area.bounds;
        float y = position.y;

        bool isYInside =  areaBounds.min.y <= y  && y <= areaBounds.max.y;

        if (!isYInside) {
            position.y = areaBounds.center.y;
            itemTransform.position = position;
        }
    }

    public void ArrangeAll() {
        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(_characterBlocks.Select(characterBlock => characterBlock.transform));
        transforms.AddRange(_equipments.Select(equipment => equipment.transform));

        float xGap = _area.bounds.size.x / transforms.Count;


        for (int i = 0; i < transforms.Count; i++) {
            transforms[i].position = new Vector2(_area.bounds.min.x + xGap * (i+0.5f), transforms[i].position.y);
        }
    }

    public void Remove(Equipment equipment) {
        _equipments.Remove(equipment);
    }

    public bool IsInsideArea(CharacterBlock characterBlock) {
        Vector3 blockPos = characterBlock.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = _area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool IsInsideArea(Equipment equipment) {
        Vector3 blockPos = equipment.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = _area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool ContainsItem(CharacterBlock characterBlock) {
        return _characterBlocks.Contains(characterBlock);
    }

    public bool ContainsItem(Equipment equipment) {
        return _equipments.Contains(equipment);
    }

    public List<CharacterBlockData> GetCharacterBlockDatas() {
        List<CharacterBlockData> datas = new List<CharacterBlockData>();

        foreach (CharacterBlock characterBlock in _characterBlocks) {
            CharacterBlockData data = characterBlock.GetData();
            datas.Add(data);
        }

        return datas.ToList();
    }

    public List<EquipmentData> GetEquipmentDatas() {
        List<EquipmentData> datas = new List<EquipmentData>();

        foreach (Equipment equipment in _equipments) {
            EquipmentData data = equipment.GetData();
            datas.Add(data);
        }

        return datas.ToList();
    }
}
