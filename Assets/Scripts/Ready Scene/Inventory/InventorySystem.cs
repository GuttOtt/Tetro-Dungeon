using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _area;
    [SerializeField] private List<InventoryEquipmentSlot> equipmentSlots = new List<InventoryEquipmentSlot>();
    [SerializeField] private EquipmentSystem equipmentSystem;
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();     
    private List<Equipment> Equipments
    {
        get
        {
            List<Equipment> equipments = new List<Equipment>();
            foreach (InventoryEquipmentSlot slot in equipmentSlots)
            {
                if (slot.Equipment != null)
                {
                    equipments.Add(slot.Equipment);
                }
            }
            return equipments.ToList();
        }
    }

    private void Awake()
    {
        foreach (InventoryEquipmentSlot slot in equipmentSlots)
        {
            slot.SetEmpty();
        }

    }

    public void Add(CharacterBlock characterBlock)
    {
    }

    public void Remove(CharacterBlock characterBlock)
    {
        _characterBlocks.Remove(characterBlock);
    }

    public void Add(Equipment equipment)
    {

        foreach (InventoryEquipmentSlot s in equipmentSlots)
        {
            if (s.Equipment == null)
            {
                s.SetEquipment(equipment);
                break;
            }
        }
    }

    public bool TryAddOnHoveredSlot(Equipment equipment)
    {
        InventoryEquipmentSlot hoveredSlot = GetHoveredSlot();
        if (hoveredSlot == null || hoveredSlot.Equipment != null) return false;

        hoveredSlot.SetEquipment(equipment);

        return true;
    }

  private void ArrangeTransform(Transform itemTransform)
    {
        Vector3 position = itemTransform.position;
        Bounds areaBounds = _area.bounds;
        float y = position.y;

        bool isYInside = areaBounds.min.y <= y && y <= areaBounds.max.y;

        if (!isYInside)
        {
            position.y = areaBounds.center.y;
            itemTransform.position = position;
        }
    }

    public void ArrangeAll()
    {
        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(_characterBlocks.Select(characterBlock => characterBlock.transform));
        transforms.AddRange(Equipments.Select(equipment => equipment.transform));

        float xGap = _area.bounds.size.x / transforms.Count;


        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].position = new Vector2(_area.bounds.min.x + xGap * (i + 0.5f), transforms[i].position.y);
        }
    }

    public void Remove(Equipment equipment)
    {
        foreach (InventoryEquipmentSlot s in equipmentSlots)
        {
            if (s.Equipment == equipment)
            {
                s.SetEmpty();
                break;
            }
        }
    }

    public bool IsInsideArea(CharacterBlock characterBlock)
    {
        Vector3 blockPos = characterBlock.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = _area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool IsInsideArea(Equipment equipment)
    {
        Vector3 blockPos = equipment.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = _area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool ContainsItem(Equipment equipment)
    {
        foreach (InventoryEquipmentSlot s in equipmentSlots)
        {
            if (s.Equipment == equipment)
            {
                return true;
            }
        }

        return false;
    }

    public List<CharacterBlockData> GetCharacterBlockDatas()
    {
        List<CharacterBlockData> datas = new List<CharacterBlockData>();

        foreach (CharacterBlock characterBlock in _characterBlocks)
        {
            CharacterBlockData data = characterBlock.GetData();
            datas.Add(data);
        }

        return datas.ToList();
    }

    public List<EquipmentData> GetEquipmentDatas()
    {
        List<EquipmentData> datas = new List<EquipmentData>();

        foreach (Equipment equipment in Equipments)
        {
            EquipmentData data = equipment.GetData();
            datas.Add(data);
        }

        return datas.ToList();
    }

    public InventoryEquipmentSlot GetHoveredSlot()
    {
        InventoryEquipmentSlot slot = Utils.Pick<InventoryEquipmentSlot>();

        return slot;
    }
}
