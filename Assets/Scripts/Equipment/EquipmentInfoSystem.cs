using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using EnumTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _statText;
    [SerializeField] private TMP_Text _synergiesText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private GameObject _panel;
    [SerializeField] private SimpleMonoButton _closeButton;
    [SerializeField] private SpriteRenderer _equipmentSprite;

    private void Start()
    {
        _panel.SetActive(false);
        _closeButton.onClick += ClosePanel;
    }

    private void Update()
    {
        HandleRightClick();
    }

    
    private void HandleRightClick() {
        if (!Input.GetMouseButtonDown(1)) return;

        BlockPart_Equipment equipmentBlockPart = Utils.Pick<BlockPart_Equipment>();
        if (equipmentBlockPart == null) return;

        Equipment equipment = equipmentBlockPart.Equipment;
        if (equipment == null) return;

        DrawInfo(equipment);
    }

    public void DrawInfo(Equipment equipment)
    {
        _panel.SetActive(true);

        // Name
        _nameText.text = equipment.Config.Name;

        // Description
        _descriptionText.text = equipment.Config.Description;

        // Sprite
        _equipmentSprite.sprite = equipment.Sprite;

        // Stats
        Stat stat = equipment.Stat;
        List<string> statTexts = new List<string>();
        if (stat.Attack != 0) statTexts.Add($"공격력: {stat.Attack}");
        if (stat.SpellPower != 0) statTexts.Add($"주문력: {stat.SpellPower}");
        if (stat.Defence != 0) statTexts.Add($"방어력: {stat.Defence}");
        if (stat.SpellDefence != 0) statTexts.Add($"마법 방어력: {stat.SpellDefence}");
        if (stat.Speed != 0) statTexts.Add($"속도: {stat.Speed}");
        if (stat.Range != 0) statTexts.Add($"사거리: {stat.Range}");
        _statText.text = string.Join("\n", statTexts);

        // Synergies
        SerializedDictionary<SynergyTypes, int> synergyDict = equipment.SynergyDict;
        List<string> synergyTexts = new List<string>();
        foreach (var synergy in synergyDict)
        {
            synergyTexts.Add($"{synergy.Key}: {synergy.Value}");
        }
        _synergiesText.text = string.Join("\n", synergyTexts);
    }

    private void ClosePanel()
    {
        _panel.SetActive(false);
    }
}
