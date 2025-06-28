using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;
using TMPro;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks.Triggers;

public class ShopSystem : MonoBehaviour
{
    private List<CharacterBlockConfig> _characterBlockPool = new List<CharacterBlockConfig>();
    [SerializeField] private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();

    private List<EquipmentConfig> _equipmentPool = new List<EquipmentConfig>();
    private List<Equipment> _equipments = new List<Equipment>();

    private Dictionary<IItem, int> _itemCostPair = new Dictionary<IItem, int>();

    private Player _player;

    private ShopCharacterSlot selectedCharacterSlot;

    [SerializeField] private BoxCollider2D area;

    [Header("Shop Settings")]
    [SerializeField] private int rerollCost = 2;
    [SerializeField] private int characterBlockCost = 8;
    [SerializeField] private int equipmentCost = 5;

    [Header("Slots")]
    [SerializeField] private List<ShopCharacterSlot> characterSlots;
    [SerializeField] private List<ShopEquipmentSlot> equipmentSlots;


    [Header("GameObject References")]

    [SerializeField] private CharacterBlockSystem _characterBlockSystem;
    [SerializeField] private EquipmentSystem _equipmentSystem;

    [SerializeField] private TMP_Text _moneyText;

    [Header("Selling Price Settings")]
    [SerializeField] private int characterBlockSellPrice = 5;
    [SerializeField] private int characterBlockSellPricePerLevel = 1;
    [SerializeField] private int equipmentSellPrice = 3;

    private void Awake()
    {
        _player = Player.Instance;
        _characterBlockPool = Resources.LoadAll<CharacterBlockConfig>("Scriptable Objects/Character Block/General").ToList();
        _equipmentPool = Resources.LoadAll<EquipmentConfig>("Scriptable Objects/Equipment").ToList();
    }

    private void Start()
    {
        InitSlots();
        StartSelling();
        UpdateMoneyText();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            Player.Instance.CurrentMoney += 10;
            UpdateMoneyText();
        }
    }

    private void InitSlots()
    {
        for (int i = 0; i < characterSlots.Count; i++)
        {
            characterSlots[i].onClick += HandleCharacterSlotMouseDown;
        }

        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            equipmentSlots[i].onClick += HandleEquipmentSlotMouseDown;
        }
    
    }

    public void StartSelling()
    {
        for (int i = 0; i < characterSlots.Count; i++)
        {
            AddCharacterBlock(i);
        }
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            AddEquipment(i);
        }
    }

    #region Character Block
    private void AddCharacterBlock(int slotNumber)
    {
        CharacterBlockConfig config = _characterBlockPool[Random.Range(0, _characterBlockPool.Count)];
        characterSlots[slotNumber].SetCharacterBlock(config, characterBlockCost);
    }

    private void HandleCharacterSlotMouseDown(ShopCharacterSlot slot)
    {
        selectedCharacterSlot = slot;
        CharacterBlockConfig config = slot.CharacterBlockConfig;

        // 임시 블럭 생성
        CharacterBlock tempBlock = _characterBlockSystem.CreateCharacterBlock(config, 1);
        
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        tempBlock.ChangeSortingLayer(draggingLayerID);

        _characterBlockSystem.SetInputOff();
        DragCharacterSlot(slot, tempBlock).Forget();
    }

    private async UniTask DragCharacterSlot(ShopCharacterSlot slot, CharacterBlock tempBlock)
    {
        while (!Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempBlock.transform.position = mousePosition;

            _characterBlockSystem.SpinBlock(tempBlock);

            await UniTask.Yield();
        }

        if (_characterBlockSystem.TryPlace(tempBlock))
        {
            _player.CurrentMoney -= slot.Cost;
            UpdateMoneyText();
            EmptyCharacterSlot(slot);
        }
        else
        {
            _characterBlockSystem.DestroyBlock(tempBlock);
        }

        selectedCharacterSlot = null;
        _characterBlockSystem.SetInputOn();
        
        //SortingLayer 원래대로 되돌리기
        int illustLyaerID = SortingLayer.NameToID("Illust");
        tempBlock.ChangeSortingLayer(illustLyaerID);
    }

    private void EmptyCharacterSlot(ShopCharacterSlot slot)
    {
        slot.SetEmpty();
    }
    #endregion

    #region Equipment
    private void AddEquipment(int slotNumber)
    {
        EquipmentConfig config = _equipmentPool[Random.Range(0, _equipmentPool.Count)];
        Equipment equipment = _equipmentSystem.CreateEquipment(config);

        ShopEquipmentSlot slot = equipmentSlots[slotNumber];
        slot.SetEquipment(equipment, equipmentCost);
    }

    private void HandleEquipmentSlotMouseDown(ShopEquipmentSlot slot)
    {
        Equipment equipment = slot.Equipment;
        equipment.gameObject.SetActive(true);

        _equipmentSystem.Select();
    }

    public void RemoveEquipment(Equipment equipment)
    {
        _equipments.Remove(equipment);
    }

    private void EmptyEquipmentSlot(ShopEquipmentSlot slot)
    {
        slot.SetEmpty();
    }
    #endregion

    public bool ContainsItem(CharacterBlock characterBlock)
    {
        return _characterBlocks.Contains(characterBlock);
    }

    public bool ContainsItem(Equipment equipment)
    {
        foreach (ShopEquipmentSlot s in equipmentSlots)
        {
            if (s.Equipment == equipment)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAffordable(IItem item)
    {
        int cost = _itemCostPair[item];

        if (cost <= _player.CurrentMoney)
        {
            return true;
        }
        else
            return false;
    }

    public bool IsAffordable(Equipment equipment)
    {
        foreach (ShopEquipmentSlot s in equipmentSlots)
        {
            if (s.Equipment == equipment)
            {
                return s.Cost <= _player.CurrentMoney;
            }
        }

        Debug.LogError("해당 equipment가 shop에 존재하지 않습니다.");
        return false;
    }


    public void Buy(IItem item)
    {
        int cost = 0;
        if (item is CharacterBlock)
        {
            CharacterBlock characterBlock = item as CharacterBlock;
            EmptyCharacterSlot(selectedCharacterSlot);
        }
        else if (item is Equipment)
        {
            Equipment equipment = (Equipment)item;
            ShopEquipmentSlot slot = null;
            foreach (ShopEquipmentSlot s in equipmentSlots)
            {
                if (s.Equipment == equipment)
                {
                    slot = s;
                    cost = s.Cost;
                    break;
                }
            }
            EmptyEquipmentSlot(slot);
        }
        else
        {
            Debug.LogError("해당 equipment가 shop에 존재하지 않습니다.");
        }
        _player.CurrentMoney -= cost;
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        _moneyText.text = _player.CurrentMoney.ToString();
    }

    public void Reroll()
    {
        if (_player.CurrentMoney >= rerollCost)
        {
            _player.CurrentMoney -= rerollCost;
            UpdateMoneyText();

            for (int i = 0; i < _characterBlocks.Count; i++)
            {
                Destroy(_characterBlocks[i].gameObject);
            }

            for (int i = 0; i < _equipments.Count; i++)
            {
                Destroy(_equipments[i].gameObject);
            }

            foreach (ShopCharacterSlot s in characterSlots)
            {
                EmptyCharacterSlot(s);
            }

            foreach (ShopEquipmentSlot s in equipmentSlots)
            {
                EmptyEquipmentSlot(s);
            }

            StartSelling();
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }

    public bool IsInsideShopArea(CharacterBlock characterBlock)
    {
        Vector3 blockPos = characterBlock.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool IsInsideShopArea(Equipment equipment)
    {
        Vector3 blockPos = equipment.transform.position;
        blockPos.z = 0;
        Bounds areaBounds = area.bounds;

        return areaBounds.Contains(blockPos);
    }
    
    public void Sell(IItem item)
    {
        if (item is CharacterBlock)
        {
            CharacterBlock characterBlock = item as CharacterBlock;
            _player.CurrentMoney += characterBlockSellPrice + (characterBlock.CurrentLevel - 1) * characterBlockSellPricePerLevel;
        }
        else if (item is Equipment)
        {
            Equipment equipment = (Equipment)item;
            RemoveEquipment(equipment);
            _player.CurrentMoney += equipmentSellPrice;
        }
        
        UpdateMoneyText();
    }
}
