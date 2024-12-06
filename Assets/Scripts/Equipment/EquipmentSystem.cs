using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
    private List<Equipment> _equipments = new List<Equipment>();
    [SerializeField] private Equipment _euipmentPrefab;
    private Equipment _selectedEquipment;
    private Vector3 _selectedPos;

    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private ShopSystem _shopSystem;

    private bool _isInputOn = true;

    void Update() {
        Select();
        UnSelect();
        MoveSelectedEquipment();
        SpinEquipment();
    }

    public Equipment CreateEquipment(EquipmentConfig config) {
        Equipment newEquipment = Instantiate(_euipmentPrefab);
        newEquipment.Init(config);
        _equipments.Add(newEquipment);

        return newEquipment;
    }

    public Equipment CreateEquipment(EquipmentData data) {
        Equipment newEquipment = CreateEquipment(data.Config);
        return newEquipment;
    }

    public Equipment CreateEquipment(EquipmentData data, CharacterBlock characterBlock) {
        Equipment newEquipment = CreateEquipment(data.Config);


        //Spin
        int spinDegree = data.SpinDegree;
        newEquipment.transform.Rotate(0, 0, -spinDegree);
        newEquipment.SpinDegree = spinDegree;

        //Locate in characterBlock
        Vector2Int location = data.Location;
        newEquipment.Place(characterBlock, location);


        return newEquipment;
    }

    private void Select() {
        if (!_isInputOn || !Input.GetMouseButtonDown(0) || _selectedEquipment != null) {
            return;
        }

        BlockPart_Equipment selectedBlockPart = Utils.Pick<BlockPart_Equipment>();
        if (selectedBlockPart == null) return;

        Equipment selectedEquipment = selectedBlockPart.Equipment;
        if (selectedEquipment == null) return;

        _selectedEquipment = selectedEquipment;

        //가장 위로 가게 하기 위해 SortingLayer 설정
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        selectedEquipment.ChangeSortingLayer(draggingLayerID);

        //원래 위치 저장
        _selectedPos = selectedEquipment.transform.position;

        //마커 보이게
        _selectedEquipment.SetMarkersOn(true);

        selectedEquipment.Unplace();
    }

    private void MoveSelectedEquipment() {
        if (_selectedEquipment == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _selectedEquipment.transform.position.z;
        _selectedEquipment.transform.position = mousePosition;
    }

    private void UnSelect() {
        if (!Input.GetMouseButtonUp(0) || _selectedEquipment == null) return;

        //SortingLayer 원래대로 되돌리기
        int equipmentLayerID = SortingLayer.NameToID("Equipment");
        _selectedEquipment.ChangeSortingLayer(equipmentLayerID);

        //Placing
        bool isPlaced = TryPlace();
        if (!isPlaced) {
            //Inventory에 있었던 경우
            if (_inventorySystem.ContainsItem(_selectedEquipment)) {
                if (!_inventorySystem.IsInsideArea(_selectedEquipment)) {
                    _selectedEquipment.transform.position = _selectedPos;
                }
            }
            //Shop에 있는 상태였을 경우
            else if (_shopSystem.ContainsItem(_selectedEquipment)) {
                //Shop -> Inventory
                if (_inventorySystem.IsInsideArea(_selectedEquipment)
                    && _shopSystem.IsAffordable(_selectedEquipment)) {
                    _shopSystem.Buy(_selectedEquipment);
                    _inventorySystem.Add(_selectedEquipment);
                }
                else {
                    _selectedEquipment.transform.position = _selectedPos;
                }
            }
            //Place 되어 있던 상태일 때
            else {
                if (_inventorySystem.IsInsideArea(_selectedEquipment)) {
                    _selectedEquipment.Unplace();
                    _inventorySystem.Add(_selectedEquipment);
                }
                else {
                    _selectedEquipment.transform.position = _selectedPos;
                    _selectedEquipment.Place();
                }
            }
        }
        else {
            Debug.Log("Placed");
        }

        //마커 안 보이게
        _selectedEquipment.SetMarkersOn(false);

        _selectedEquipment = null;
    }

    private bool TryPlace() {
        if (_selectedEquipment == null)
            return false;

        if (_selectedEquipment.IsPlacable()) {
            _selectedEquipment.Place();
            _inventorySystem.Remove(_selectedEquipment);
            return true;
        }
        else {
            return false;
        }
    }

    private void SpinEquipment() {
        if (_selectedEquipment == null) return;

        bool isClockwise;
        if (Input.GetKeyDown(KeyCode.Q))
            isClockwise = false;
        else if (Input.GetKeyDown(KeyCode.E))
            isClockwise = true;
        else
            return;

        _selectedEquipment.Spin(isClockwise);
    }
}
