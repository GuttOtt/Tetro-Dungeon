using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
    private List<Equipment> _equipments = new List<Equipment>();
    [SerializeField] private Equipment _euipmentPrefab;
    private Equipment _selectedEquipment;

    [SerializeField] private InventorySystem _inventorySystem;

    private bool _isInputOn;

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public Equipment CreateEquipment(EquipmentConfig config) {
        Equipment newEquipment = Instantiate(_euipmentPrefab);
        newEquipment.Init(config);
        _equipments.Add(newEquipment);

        return newEquipment;
    }

    private void Select() {
        if (!_isInputOn || !Input.GetMouseButton(0) || _selectedEquipment != null) {
            return;
        }

        BlockPart selectedBlockPart = Utils.Pick<BlockPart>();
        if (selectedBlockPart == null) return;
    }
}
