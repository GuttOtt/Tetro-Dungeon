using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    private List<CharacterBlockConfig> _characterBlockPool = new List<CharacterBlockConfig>();
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();

    private List<EquipmentConfig> _equipmentPool = new List<EquipmentConfig>();
    private List<Equipment> _equipments = new List<Equipment>();

    private Dictionary<IItem, int> _itemCostPair = new Dictionary<IItem, int>();

    private Player _player;

    [SerializeField] private SpriteRenderer[] _characterBlockSlots;
    [SerializeField] private SpriteRenderer[] _equipmentBlockSlots;
    [SerializeField] private TMP_Text[] _characterCostTexts;
    [SerializeField] private TMP_Text[] _equipmentCostTexts;

    [SerializeField] private CharacterBlockSystem _characterBlockSystem;
    [SerializeField] private EquipmentSystem _equipmentSystem;

    [SerializeField] private TMP_Text _moneyText;

    private void Awake() {
        _player = Player.Instance;
        _characterBlockPool = Resources.LoadAll<CharacterBlockConfig>("Scriptable Objects/Character Block").ToList();
        _equipmentPool = Resources.LoadAll<EquipmentConfig>("Scriptable Objects/Equipment").ToList();
    }

    private void Start() {
        StartSelling();
        UpdateMoneyText();
    }

    public void StartSelling() {
        for (int i = 0; i < 3; i++) {
            AddCharacterBlock(i);
            AddEquipment(i);
        }
    }

    private void AddCharacterBlock(int slotNumber) {
        CharacterBlockConfig config = _characterBlockPool[Random.Range(0, _characterBlockPool.Count)];
        CharacterBlock characterBlock = _characterBlockSystem.CreateCharacterBlock(config, 0);

        characterBlock.transform.parent = _characterBlockSlots[slotNumber].transform;
        characterBlock.transform.localPosition = Vector3.back;

        _characterBlocks.Add(characterBlock);

        //Cost
        int cost = Random.Range(5, 6);
        _itemCostPair.Add(characterBlock, cost);
        //_characterCostTexts[slotNumber].text = cost.ToString();
    }

    public void RemoveCharacterBlock(CharacterBlock characterBlock) {
        _characterBlocks.Remove(characterBlock);
    }


    private void AddEquipment(int slotNumber) {
        EquipmentConfig config = _equipmentPool[Random.Range(0, _equipmentPool.Count)];
        Equipment equipment = _equipmentSystem.CreateEquipment(config);

        equipment.transform.parent = _equipmentBlockSlots[slotNumber].transform;
        equipment.transform.localPosition = Vector3.back;

        _equipments.Add(equipment);

        //Cost
        int cost = Random.Range(3, 4);
        _itemCostPair.Add(equipment, cost);
        //_equipmentCostTexts[slotNumber].text = cost.ToString();
    }

    public void RemoveEquipment(Equipment equipment) {
        _equipments.Remove(equipment);
    }
    public bool ContainsItem(CharacterBlock characterBlock) {
        return _characterBlocks.Contains(characterBlock);
    }

    public bool ContainsItem(Equipment equipment) {
        return _equipments.Contains(equipment);
    }

    public bool IsAffordable(IItem item) {
        int cost = _itemCostPair[item];

        if (cost <= _player.CurrentMoney) {
            return true;
        }
        else
            return false;
    }

    public void Buy(IItem item) {
        if (item is CharacterBlock) {
            CharacterBlock characterBlock = item as CharacterBlock;
            RemoveCharacterBlock(characterBlock);
        }
        else if (item is Equipment) {
            Equipment equipment = (Equipment)item;
            RemoveEquipment(equipment);
        }
        _player.CurrentMoney -= _itemCostPair[item];
        UpdateMoneyText();
    }

    private void UpdateMoneyText() {
        _moneyText.text = "Money: "+_player.CurrentMoney.ToString();
    }
}
