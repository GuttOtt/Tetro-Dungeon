using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBlockInfoSystem : MonoBehaviour {
    [Header("Stats UI")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _spellPowerText;
    [SerializeField] private TMP_Text _defenceText;
    [SerializeField] private TMP_Text _spellDefenceText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _rangeText;
    
    [Header("Level Up UI")]
    [SerializeField] private TMP_Text _levelUpCostText;
    [SerializeField] private SimpleMonoButton _levelUpButton;
    
    [Header("Character Illustration")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private SpriteRenderer _illustImage;
    [SerializeField] private GameObject _spumRoot;
    [SerializeField] private SPUM_Prefabs _spum;
    
    [Header("Skill UI")]
    [SerializeField] private SkillDescriptor _skillDescriptorPrefab;
    [SerializeField] private Vector3 _skillDescriptorOrigin;
    [SerializeField] private Vector3 _skillDescriptorGap;
    private List<SkillDescriptor> _skillDescriptors = new List<SkillDescriptor>();

    [Header("Synergy UI")]
    [SerializeField] private TMP_Text _synergyText;

    [Header("Awakening UI")]
    [SerializeField] private TMP_Text _awakeningsText;

    [Header("References")]
    [SerializeField] private SimpleMonoButton _closeButton;
    [SerializeField] private CharacterBlockSystem _characterBlockSystem;
    [SerializeField] private EquipmentSystem equipmentSystem;

    public CharacterBlock CurrentCharacterBlock => currentCharacterBlock;

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
        
        //Set Inputs Off
        _characterBlockSystem.SetInputOff();
        equipmentSystem.SetInputOff();

        //Lelvel Up
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

         // Synergies
        string synergyText = "";
        foreach (var synergy in characterBlock.SynergyDict) {
            if (synergy.Key != EnumTypes.SynergyTypes.None && synergy.Value > 0) {
                synergyText += $"{synergy.Key}: {synergy.Value} ";
            }
        }
        _synergyText.text = synergyText;

        // Awakenings
        string awakeningsText = "";
        foreach (var awakening in characterBlock.Awakenings) {
            if (awakening != null) {
                // 만약 해당 awakening의 활성화 결과가 true라면 초록색 리치 텍스트로 표시
                bool isActive = false;

                if (characterBlock.AwakeningActivation.ContainsKey(awakening)) {
                    isActive = characterBlock.AwakeningActivation[awakening];
                }

                if (isActive) {
                    awakeningsText += $"<color=green>{awakening.description}</color>\n";
                }
                else {
                    awakeningsText += $"{awakening.description}\n";
                }
            }
        }
        _awakeningsText.text = awakeningsText;

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


    public void ClosePanel()
    {
        _panel.SetActive(false);
        _characterBlockSystem.SetInputOn();
        equipmentSystem.SetInputOn();
        Debug.Log("Close Panel");
    }
}
