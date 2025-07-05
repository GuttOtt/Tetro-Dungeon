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

    private void Update()
    {
        CharacterBlock charaterBlock = GetRightClockedBlock();
        ShopCharacterSlot slot = GetRightClickedSlot();
        BaseUnit unit = GetRightClickedUnit();


        if (charaterBlock != null)
            DrawInfo(charaterBlock);
        else if (slot != null)
            DrawInfo(slot);
        else if (unit != null)
            DrawInfo(unit);
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

    private ShopCharacterSlot GetRightClickedSlot()
    {
        if (!Input.GetMouseButtonDown(1))
            return null;

        ShopCharacterSlot slot = Utils.Pick<ShopCharacterSlot>();
        if (slot == null)
            return null;

        return slot;
    }

    private BaseUnit GetRightClickedUnit()
    {
        if (!Input.GetMouseButtonDown(1))
            return null;

        BaseUnit unit = Utils.Pick<BaseUnit>();
        if (unit == null)
            return null;

        return unit;
    }

    private void DrawInfo(BaseUnit unit)
    {
        _panel.SetActive(true);

        CharacterBlockConfig config = unit.Config;

        //Set Inputs Off
        _characterBlockSystem?.SetInputOff();
        equipmentSystem?.SetInputOff();

        //Name
        _nameText.text = config.Name;

        Stat stat = unit.Stat;
        DrawStat(stat);
        DrawLevel(unit.Level);
        
        _illustImage.sprite = config.Illust;

        DrawSpum(config.SPUM_Prefabs);

        //Skill
        foreach (SkillDescriptor descriptor in _skillDescriptors)
        {
            Destroy(descriptor.gameObject);
        }
        _skillDescriptors.Clear();

        List<UnitSkill> skills = unit.Skills;
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == null) continue;

            SkillDescriptor descriptor = Instantiate(_skillDescriptorPrefab, _panel.transform);
            descriptor.DescribeSkill(skills[i]);
            _skillDescriptors.Add(descriptor);

            Vector3 localPos = _skillDescriptorOrigin + i * _skillDescriptorGap;
            descriptor.transform.localPosition = localPos;
        }

        // Synergies
        string synergyText = "";
        foreach (var synergy in unit.SynergyDict)
        {
            if (synergy.Key != EnumTypes.SynergyTypes.None && synergy.Value > 0)
            {
                synergyText += $"{synergy.Key}: {synergy.Value} ";
            }
        }
        _synergyText.text = synergyText;

        /*
        // Awakenings
        string awakeningsText = "";
        foreach (var awakening in unit.Awakenings)
        {
            if (awakening != null)
            {
                // 만약 해당 awakening의 활성화 결과가 true라면 초록색 리치 텍스트로 표시
                bool isActive = false;

                if (characterBlock.AwakeningActivation.ContainsKey(awakening))
                {
                    isActive = characterBlock.AwakeningActivation[awakening];
                }

                if (isActive)
                {
                    awakeningsText += $"<color=green>{awakening.description}</color>\n";
                }
                else
                {
                    awakeningsText += $"{awakening.description}\n";
                }
            }
        }
        _awakeningsText.text = awakeningsText;
        */
    }

    private void DrawInfo(ShopCharacterSlot slot)
    {
        _panel.SetActive(true);
        CharacterBlockConfig config = slot.CharacterBlockConfig;
        _levelUpButton.gameObject.SetActive(false);
        _levelUpCostText.text = "";

        //Set Inputs Off
        _characterBlockSystem.SetInputOff();
        equipmentSystem.SetInputOff();

        //Name
        _nameText.text = config.Name;

        //Stats
        Stat stat = config.Stat;
        DrawStat(stat);

        //Level
        DrawLevel(1);

        //Illust
        _illustImage.sprite = config.Illust;

        //SPUM
        DrawSpum(config.SPUM_Prefabs);

        //Skill
        foreach (SkillDescriptor descriptor in _skillDescriptors)
        {
            Destroy(descriptor.gameObject);
        }
        _skillDescriptors.Clear();

        // Synergies
        string synergyText = "";
        foreach (var synergy in config.BaseSynergyDict)
        {
            if (synergy.Key != EnumTypes.SynergyTypes.None && synergy.Value > 0)
            {
                synergyText += $"{synergy.Key}: {synergy.Value} ";
            }
        }
        _synergyText.text = synergyText;

        // Awakenings
        string awakeningsText = "";
        foreach (var awakening in config.Awakenings)
        {
            if (awakening != null)
            {
                awakeningsText += $"{awakening.description}\n";
            }
        }
        _awakeningsText.text = awakeningsText;

        List<SkillConfig> skills = config.Skills;
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == null) continue;

            SkillDescriptor descriptor = Instantiate(_skillDescriptorPrefab, _panel.transform);
            descriptor.DescribeSkill(skills[i]);
            _skillDescriptors.Add(descriptor);

            Vector3 localPos = _skillDescriptorOrigin + i * _skillDescriptorGap;
            descriptor.transform.localPosition = localPos;
        }
    }

    public void DrawInfo(CharacterBlock characterBlock)
    {
        _panel.SetActive(true);
        currentCharacterBlock = characterBlock;

        //Set Inputs Off
        _characterBlockSystem?.SetInputOff();
        equipmentSystem?.SetInputOff();

        //Lelvel Up
        if (_levelUpButton != null && _levelUpCostText != null)
        {
            _levelUpButton.gameObject.SetActive(true);
            _levelUpCostText.gameObject.SetActive(true);
            _levelUpCostText.text = characterBlock.LevelUpCost.ToString();
            
        }

        _nameText.text = characterBlock.Config.Name;

        Stat stat = characterBlock.Stat;
        DrawStat(stat);
        DrawLevel(characterBlock.CurrentLevel);
        
        _illustImage.sprite = characterBlock.Config.Illust;

        DrawSpum(characterBlock.Config.SPUM_Prefabs);

        //Skill
        foreach (SkillDescriptor descriptor in _skillDescriptors)
        {
            Destroy(descriptor.gameObject);
        }
        _skillDescriptors.Clear();

        List<UnitSkill> skills = characterBlock.Skills;
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == null) continue;

            SkillDescriptor descriptor = Instantiate(_skillDescriptorPrefab, _panel.transform);
            descriptor.DescribeSkill(skills[i]);
            _skillDescriptors.Add(descriptor);

            Vector3 localPos = _skillDescriptorOrigin + i * _skillDescriptorGap;
            descriptor.transform.localPosition = localPos;
        }

        // Synergies
        string synergyText = "";
        foreach (var synergy in characterBlock.SynergyDict)
        {
            if (synergy.Key != EnumTypes.SynergyTypes.None && synergy.Value > 0)
            {
                synergyText += $"{synergy.Key}: {synergy.Value} ";
            }
        }
        _synergyText.text = synergyText;

        // Awakenings
        string awakeningsText = "";
        foreach (var awakening in characterBlock.Awakenings)
        {
            if (awakening != null)
            {
                // 만약 해당 awakening의 활성화 결과가 true라면 초록색 리치 텍스트로 표시
                bool isActive = false;

                if (characterBlock.AwakeningActivation.ContainsKey(awakening))
                {
                    isActive = characterBlock.AwakeningActivation[awakening];
                }

                if (isActive)
                {
                    awakeningsText += $"<color=green>{awakening.description}</color>\n";
                }
                else
                {
                    awakeningsText += $"{awakening.description}\n";
                }
            }
        }
        _awakeningsText.text = awakeningsText;

        
    }

    private void DrawStat(Stat stat)
    {
        //_hpText.text = characterBlock.MaxHP.ToString();
        _attackText.text = stat.Attack.ToString();
        _spellPowerText.text = stat.SpellPower.ToString();
        _defenceText.text = stat.Defence.ToString();
        _spellDefenceText.text = stat.SpellDefence.ToString();
        _speedText.text = stat.Speed.ToString();
        _rangeText.text = stat.Range.ToString();
    }

    private void DrawLevel(int level)
    {
        _levelText.text = "Lvl. " + level.ToString();
    }

    private void DrawSpum(SPUM_Prefabs spumPrefab)
    {
        if (_spum != null)
        {
            Destroy(_spum.gameObject);
        }

        if (spumPrefab == null)
            return;
        
        _spum = Instantiate(spumPrefab, _spumRoot.transform);

        _spum.transform.localPosition = Vector3.zero;

        int uiSortingLayer = SortingLayer.NameToID("UI");
        _spum.SetSortingLayer(uiSortingLayer);
    }


    public void ClosePanel()
    {
        _panel.SetActive(false);
        _characterBlockSystem?.SetInputOn();
        equipmentSystem?.SetInputOn();
        Debug.Log("Close Panel");
    }
}
