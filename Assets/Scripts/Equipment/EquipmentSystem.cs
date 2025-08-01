using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private List<Equipment> _equipments = new List<Equipment>();
    [SerializeField] private Equipment _euipmentPrefab;
    private Equipment _selectedEquipment;
    private Vector3 _selectedPos;
    private ShopEquipmentSlot selectedSlot;

    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private EquipmentInfoSystem _equipmentInfoSystem;
    [SerializeField] private ShopSystem _shopSystem;

    private bool _isInputOn = true;

    public Action<Equipment> OnPlace;
    public Action<Equipment> OnUnplace;
    public Action<Equipment> OnPlaceOnBoard;
    public Action<Equipment> OnUnplaceFromBoard;

    void Update()
    {
        HandleMouseButtonDown();
        UnSelect();
        MoveSelectedEquipment();
        SpinEquipment();
    }


    public Equipment CreateEquipment(EquipmentConfig config)
    {
        Equipment newEquipment = Instantiate(_euipmentPrefab);
        newEquipment.Init(config);
        _equipments.Add(newEquipment);

        return newEquipment;
    }

    public Equipment CreateEquipment(EquipmentData data)
    {
        Equipment newEquipment = CreateEquipment(data.Config);
        return newEquipment;
    }

    public Equipment CreateEquipment(EquipmentData data, CharacterBlock characterBlock)
    {
        Equipment newEquipment = CreateEquipment(data.Config);


        //Spin
        int spinDegree = data.SpinDegree;
        newEquipment.transform.Rotate(0, 0, -spinDegree);
        newEquipment.SpinDegree = spinDegree;

        //Locate in characterBlock
        Vector2Int location = data.Location;
        newEquipment.Place(characterBlock, location);
        OnPlace?.Invoke(newEquipment);
        if (characterBlock.IsPlaced)
        {
            OnPlaceOnBoard?.Invoke(newEquipment);
        }


        return newEquipment;
    }

    private void HandleMouseButtonDown()
    {
        if (!_isInputOn || !Input.GetMouseButtonDown(0) || _selectedEquipment != null)
        {
            return;
        }

        if (!SelectSlot())
            SelectOnMouse();
    }

    private bool SelectSlot()
    {
        ShopEquipmentSlot slot = Utils.Pick<ShopEquipmentSlot>();
        if (slot == null) return false;

        Equipment equipment = slot.Equipment;
        if (equipment == null) return false;

        equipment.gameObject.SetActive(true);

        Select(equipment);
        selectedSlot = slot;

        return true;
    }

    public void SelectOnMouse()
    {
        BlockPart_Equipment selectedBlockPart = Utils.Pick<BlockPart_Equipment>();
        if (selectedBlockPart == null) return;

        Equipment equipmentOnMouse = selectedBlockPart.Equipment;
        if (equipmentOnMouse == null) return;

        Select(equipmentOnMouse);
    }

    private void Select(Equipment equipment)
    {
        _selectedEquipment = equipment;

        //가장 위로 가게 하기 위해 SortingLayer 설정
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        equipment.ChangeSortingLayer(draggingLayerID);

        //원래 위치 저장
        _selectedPos = equipment.transform.position;

        //마커 보이게
        _selectedEquipment.SetMarkersOn(true);

        //Unplace
        if (equipment.IsPlaced)
        {
            if (equipment.CharacterBlock.IsPlaced)
            {
                OnUnplaceFromBoard?.Invoke(equipment);
            }
            OnUnplace?.Invoke(equipment);
        }
        equipment.Unplace();
    }
    

    private void MoveSelectedEquipment()
    {
        if (_selectedEquipment == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _selectedEquipment.transform.position.z;
        _selectedEquipment.transform.position = mousePosition;
    }

    private void UnSelect()
    {
        if (!Input.GetMouseButtonUp(0) || _selectedEquipment == null) return;

        //SortingLayer 원래대로 되돌리기
        int equipmentLayerID = SortingLayer.NameToID("Equipment");
        _selectedEquipment.ChangeSortingLayer(equipmentLayerID);

        //Placing
        bool isPlaced = TryPlace();
        if (!isPlaced)
        {
            // 판매 처리
            if (_shopSystem.IsInsideShopArea(_selectedEquipment) && !_shopSystem.ContainsItem(_selectedEquipment))
            {
                _shopSystem.Sell(_selectedEquipment);
                Debug.Log("Sold equipment: " + _selectedEquipment.name);
                selectedSlot?.SetEmpty();
                DestroyEquipment(_selectedEquipment);
            }

            //Inventory에 있었던 경우
            else if (selectedSlot is InventoryEquipmentSlot)
            {
                InventoryEquipmentSlot hoveredSlot = _inventorySystem.GetHoveredSlot();
                if (hoveredSlot != null)
                {
                    if (_inventorySystem.TryAddOnHoveredSlot(_selectedEquipment))
                    {
                        selectedSlot.SetEmpty();
                    }
                    else //Change
                    {
                        Equipment hoveredSlotEquipment = hoveredSlot.Equipment;
                        hoveredSlot.SetEquipment(_selectedEquipment);
                        selectedSlot.SetEquipment(hoveredSlotEquipment);
                    }
                }
                else
                {
                    selectedSlot.SetEquipment(_selectedEquipment);
                }

            }
            //Shop에 있는 상태였을 경우
            else if (selectedSlot != null && selectedSlot is not InventoryEquipmentSlot)
            {
                //Shop -> Inventory
                if (_shopSystem.IsAffordable(_selectedEquipment)
                && _inventorySystem.TryAddOnHoveredSlot(_selectedEquipment))
                {
                    _shopSystem.Buy(_selectedEquipment);
                }
                else
                {
                    _selectedEquipment.transform.position = _selectedPos;
                    _selectedEquipment.gameObject.SetActive(false);
                }
            }
            //Place 되어 있던 상태일 때
            else
            {
                if (_inventorySystem.IsInsideArea(_selectedEquipment))
                {
                    _selectedEquipment.Unplace();
                    _inventorySystem.Add(_selectedEquipment);
                }
                else // 원래대로 되돌림
                {
                    _selectedEquipment.transform.position = _selectedPos;
                    Place(_selectedEquipment);
                }
            }
        }

        //마커 안 보이게
        _selectedEquipment.SetMarkersOn(false);

        _selectedEquipment = null;
        selectedSlot = null;
    }

    private bool TryPlace()
    {
        if (_selectedEquipment == null || !_selectedEquipment.IsPlacable())
            return false;

        if (_shopSystem.ContainsItem(_selectedEquipment))
        {
            if (!_shopSystem.IsAffordable(_selectedEquipment))
            {
                Debug.Log("Not enough money to buy this equipment.");
                return false;
            }
            _shopSystem.Buy(_selectedEquipment);
        }

        _inventorySystem.Remove(_selectedEquipment);

        Place(_selectedEquipment);

        return true;
    }

    private void Place(Equipment equipment)
    {
        equipment.Place();
        _inventorySystem.Remove(equipment);
        OnPlace?.Invoke(equipment);

        if (equipment.CharacterBlock.IsPlaced)
        {
            OnPlaceOnBoard?.Invoke(equipment);
        }
    }

    private void SpinEquipment()
    {
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

    public void PlaceOnBoard(Equipment equipment)
    {
        OnPlaceOnBoard?.Invoke(equipment);
    }

    public void UnplaceFromBoard(Equipment equipment)
    {
        OnUnplaceFromBoard?.Invoke(equipment);
    }

    public void SetInputOn()
    {
        _isInputOn = true;
    }

    public void SetInputOff()
    {
        _isInputOn = false;
    }
    
    public void DestroyEquipment(Equipment equipment)
    {
        if (_equipments.Contains(equipment))
        {
            _equipments.Remove(equipment);
            Destroy(equipment.gameObject);
        }
    }
}
