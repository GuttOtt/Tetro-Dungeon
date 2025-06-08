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

        Select();
    }

    public void Select()
    {
        BlockPart_Equipment selectedBlockPart = Utils.Pick<BlockPart_Equipment>();
        if (selectedBlockPart == null) return;

        Equipment selectedEquipment = selectedBlockPart.Equipment;
        if (selectedEquipment == null) return;

        _selectedEquipment = selectedEquipment;

        //���� ���� ���� �ϱ� ���� SortingLayer ����
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        selectedEquipment.ChangeSortingLayer(draggingLayerID);

        //���� ��ġ ����
        _selectedPos = selectedEquipment.transform.position;

        //��Ŀ ���̰�
        _selectedEquipment.SetMarkersOn(true);

        //Unplace
        if (selectedEquipment.IsPlaced)
        {
            if (selectedEquipment.CharacterBlock.IsPlaced)
            {
                OnUnplaceFromBoard?.Invoke(selectedEquipment);
            }
            OnUnplace?.Invoke(selectedEquipment);
        }
        selectedEquipment.Unplace();
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

        //SortingLayer ������� �ǵ�����
        int equipmentLayerID = SortingLayer.NameToID("Equipment");
        _selectedEquipment.ChangeSortingLayer(equipmentLayerID);

        //Placing
        bool isPlaced = TryPlace();
        if (!isPlaced)
        {
            // �Ǹ� ó��
            if (_shopSystem.IsInsideShopArea(_selectedEquipment) && !_shopSystem.ContainsItem(_selectedEquipment))
            {
                _shopSystem.Sell(_selectedEquipment);
                Debug.Log("Sold equipment: " + _selectedEquipment.name);
                DestroyEquipment(_selectedEquipment);
            }

            //Inventory�� �־��� ���
            else if (_inventorySystem.ContainsItem(_selectedEquipment))
            {
                if (!_inventorySystem.IsInsideArea(_selectedEquipment))
                {
                    _selectedEquipment.transform.position = _selectedPos;
                }
            }
            //Shop�� �ִ� ���¿��� ���
            else if (_shopSystem.ContainsItem(_selectedEquipment))
            {
                //Shop -> Inventory
                if (_inventorySystem.IsInsideArea(_selectedEquipment)
                    && _shopSystem.IsAffordable(_selectedEquipment))
                {
                    _shopSystem.Buy(_selectedEquipment);
                    _inventorySystem.Add(_selectedEquipment);
                }
                else
                {
                    _selectedEquipment.transform.position = _selectedPos;
                    _selectedEquipment.gameObject.SetActive(false);
                }
            }
            //Place �Ǿ� �ִ� ������ ��
            else
            {
                if (_inventorySystem.IsInsideArea(_selectedEquipment))
                {
                    _selectedEquipment.Unplace();
                    _inventorySystem.Add(_selectedEquipment);
                }
                else
                {
                    _selectedEquipment.transform.position = _selectedPos;
                    Place(_selectedEquipment);
                }
            }
        }
        else
        {
            Debug.Log("Placed");
        }

        //��Ŀ �� ���̰�
        _selectedEquipment.SetMarkersOn(false);

        _selectedEquipment = null;
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
