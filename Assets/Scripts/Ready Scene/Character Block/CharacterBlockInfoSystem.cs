using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBlockInfoSystem : MonoBehaviour {
    [SerializeField] private TMP_Text _nameText, _levelText, _hpText, _attackText, _spellPowerText,
        _defenceText, _spellDefenceText, _speedText, _rangeText, _levelUpCostText;
    
    [SerializeField] private GameObject _panel;
    
    [SerializeField] private SpriteRenderer _illustImage;

    [SerializeField] private GameObject _spumRoot;
    [SerializeField] private SPUM_Prefabs _spum;
    [SerializeField] private SimpleMonoButton _closeButton;
    [SerializeField] private SkillDescriptor _skillDescriptorPrefab;
    [SerializeField] private Vector3 _skillDescriptorOrigin, _skillDescriptorGap;
    [SerializeField] private SimpleMonoButton _levelUpButton;
    [SerializeField] private CharacterBlockSystem _characterBlockSystem;

    private List<SkillDescriptor> _skillDescriptors = new List<SkillDescriptor>();
    private CharacterBlock currentCharacterBlock;

    private void Start() {
        _panel.SetActive(false);
        _closeButton.onClick += ClosePanel;
    }

    private void Update() {
        CharacterBlock charaterBlock = GetRightClockedBlock();

        if (charaterBlock != null) 
            DrawInfo(charaterBlock);
    }

    private CharacterBlock GetRightClockedBlock() {
        if (!Input.GetMouseButtonDown(1))
            return null;

        CharacterBlock characterBlock = Utils.PickCharacterBlock();

        if (characterBlock == null ||
            (Utils.Pick<BlockPart_Equipment>() && !Input.GetKey(KeyCode.LeftShift)))
            return null;

        return characterBlock;
    }

    public void DrawInfo(CharacterBlock characterBlock) {
        _panel.SetActive(true);
        currentCharacterBlock = characterBlock;

        //Lelvel Up
        _levelUpButton.onClick = null;
        _levelUpButton.onClick += () => HandleLevelUpButton();
        _levelUpCostText.text = characterBlock.LevelUpCost.ToString();

        //Name
        _nameText.text = characterBlock.Config.Name;

        //Stats
        Stat stat = characterBlock.Stat;
        //_hpText.text = characterBlock.MaxHP.ToString();
        _attackText.text = stat.Attack.ToString();
        _spellPowerText.text = stat.SpellPower.ToString();
        _defenceText.text = stat.Defence.ToString();
        _spellDefenceText.text = stat.SpellDefence.ToString();
        _speedText.text = stat.Speed.ToString();
        _rangeText.text = stat.Range.ToString();
        _levelText.text = "Lvl. " + characterBlock.CurrentLevel.ToString();


        //Illust
        _illustImage.sprite = characterBlock.Config.Illust;

        //SPUM
        if (_spum != null) {
            Destroy(_spum.gameObject);
        }
        SPUM_Prefabs spumPrefab = characterBlock.Config.SPUM_Prefabs;
        _spum = Instantiate(spumPrefab, _spumRoot.transform);
        
        _spum.transform.localPosition = Vector3.zero;

        int uiSortingLayer = SortingLayer.NameToID("UI");
        _spum.SetSortingLayer(uiSortingLayer);

        //Skill
        foreach (SkillDescriptor descriptor in _skillDescriptors) {
            Destroy(descriptor.gameObject);
        }
        _skillDescriptors.Clear();

        List<UnitSkill> skills = characterBlock.Skills;
        for (int i = 0; i < skills.Count; i++) {
            if (skills[i] == null) continue;

            SkillDescriptor descriptor = Instantiate(_skillDescriptorPrefab, _panel.transform);
            descriptor.DescribeSkill(skills[i]);
            _skillDescriptors.Add(descriptor);

            Vector3 localPos = _skillDescriptorOrigin + i * _skillDescriptorGap;
            descriptor.transform.localPosition = localPos;
        }
    }

    private void HandleLevelUpButton() {
        CharacterBlock levelUpBlock = _characterBlockSystem.LevelUp(currentCharacterBlock);

        //Level Up에 성공했을 경우.
        if (levelUpBlock != null) {
            DrawInfo(levelUpBlock);
            currentCharacterBlock = levelUpBlock;
        }
    }

    public void ClosePanel() {
        _panel.SetActive(false);
    }
}
