using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using EnumTypes;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;
using AYellowpaper.SerializedCollections;

public class SynergySystem : MonoBehaviour {
    #region private members
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private CharacterBlockSystem characterBlockSystem;
    [SerializeField] private EquipmentSystem equipmentSystem;

    [SerializeField] private SerializedDictionary<SynergyTypes, int> _synergyDic = 
        new SerializedDictionary<SynergyTypes, int>();

    [SerializeField]
    private List<BaseSynergy> _allSynergyList = new List<BaseSynergy>();

    [SerializeField]
    private TMP_Text _synergyText;
    

    #endregion

    private void Awake() {

        // Subscribe to events
        characterBlockSystem.OnPlace += HandleCharacterBlockPlace;
        characterBlockSystem.OnUnplace += HandleCharacterBlockUnplace;
        equipmentSystem.OnPlaceOnBoard += HandleEquipmentPlace;
        equipmentSystem.OnUnplaceFromBoard += HandleEquipmentRemove;

        if (battleSystem != null) {
            battleSystem.OnBattleBegin += OnBattleBeginEffects;
            battleSystem.OnTimePass += OnTimePass;
        }


        _allSynergyList = Resources.LoadAll<BaseSynergy>("Scriptable Objects/Synergy").ToList();
    }

    private void Start() {
        _synergyText.richText = true;  // Rich Text Ȱ��ȭ
        // ...existing code...
    }
    
    private void HandleEquipmentPlace(Equipment equipment) {
        if (!equipment.CharacterBlock.IsPlaced) {
            return;
        }

        foreach (SynergyTypes synergyType in equipment.SynergyDict.Keys) {
            if (_synergyDic.ContainsKey(synergyType)) {
                _synergyDic[synergyType] += equipment.SynergyDict[synergyType];
            }
            else if (synergyType != SynergyTypes.None) {
                _synergyDic.Add(synergyType, equipment.SynergyDict[synergyType]);
            }
        }
        DisplaySynergies();
    }

    private void HandleEquipmentRemove(Equipment equipment) {
        if (!equipment.CharacterBlock.IsPlaced) {
            return;
        }

        foreach (SynergyTypes synergyType in equipment.SynergyDict.Keys) {
            if (_synergyDic.ContainsKey(synergyType)) {
                _synergyDic[synergyType] -= equipment.SynergyDict[synergyType];
            }
        }
        DisplaySynergies();
    }

    private void HandleCharacterBlockPlace(CharacterBlock characterBlock) {
        foreach (SynergyTypes synergyType in characterBlock.SynergyDict.Keys) {
            if (_synergyDic.ContainsKey(synergyType)) {
                _synergyDic[synergyType] += characterBlock.SynergyDict[synergyType];
            }
            else if (synergyType != SynergyTypes.None) {
                _synergyDic.Add(synergyType, characterBlock.SynergyDict[synergyType]);
            }
        }
        DisplaySynergies();
    }

    private void HandleCharacterBlockUnplace(CharacterBlock characterBlock) {
        foreach (SynergyTypes synergyType in characterBlock.SynergyDict.Keys) {
            if (_synergyDic.ContainsKey(synergyType)) {
                _synergyDic[synergyType] -= characterBlock.SynergyDict[synergyType];
            }
        }
        DisplaySynergies();
    }

    [Header("Synergy Display")]
    [SerializeField] private GameObject _synergyDisplayPrefab;
    [SerializeField] private Transform _displayContainer;
    private Dictionary<SynergyTypes, SynergyDisplay> _synergyDisplays = new();
    public void DisplaySynergies() {
        // ���ο� �ó��� ���÷��� ���� �Ǵ� ������Ʈ
        foreach (var synergy in _synergyDic) {
            if (_synergyDisplays.TryGetValue(synergy.Key, out SynergyDisplay display)) {
                display.UpdateDisplay(synergy.Value);
            }
            else {
                SynergyDisplay newDisplay = CreateSynergyDisplay(FindSynergy(synergy.Key));
                newDisplay.UpdateDisplay(synergy.Value);
            }
        }

        // ������ �ó������� ����Ʈ�� ����
        List<SynergyTypes> synergiesToRemove = new List<SynergyTypes>();
        foreach (var synergy in _synergyDisplays) {
            if (!_synergyDic.ContainsKey(synergy.Key) || _synergyDic[synergy.Key] <= 0) {
                synergiesToRemove.Add(synergy.Key);
            }
        }

        // ����� ����Ʈ�� ��ȸ�ϸ� �ó��� ����
        foreach (var synergyType in synergiesToRemove) {
            if (_synergyDisplays.TryGetValue(synergyType, out SynergyDisplay display)) {
                Destroy(display.gameObject);
                _synergyDisplays.Remove(synergyType);
            }
        }

        ArrangeSynergyDisplays();
    }

    private void ArrangeSynergyDisplays() {
        float x = 0;
        float xOffset = _synergyDisplayPrefab.GetComponent<RectTransform>().rect.width + 5;
        foreach (var synergy in _synergyDisplays) {
            synergy.Value.transform.localPosition = new Vector3(x, 0, 0);
            x += xOffset;
        }
    }

    private SynergyDisplay CreateSynergyDisplay(BaseSynergy synergy) {
        GameObject displayObj = Instantiate(_synergyDisplayPrefab, _displayContainer);
        SynergyDisplay display = displayObj.GetComponent<SynergyDisplay>();
        display.Init(synergy.SynergyType, synergy);
        _synergyDisplays[synergy.SynergyType] = display;

        return display;
    }

    public void OnBattleBeginEffects() {
        List<BaseSynergy> synergies = FindActivatedSynergies();
        TurnContext turnContext = TurnContextGenerator.Instance.GenerateTurnContext();

        foreach (BaseSynergy synergy in synergies) {
            synergy.OnBattleBegin(turnContext, _synergyDic[synergy.SynergyType]);
        }
    }

    public void OnTimePass() {
        List<BaseSynergy> synergies = FindActivatedSynergies();
        TurnContext turnContext = TurnContextGenerator.Instance.GenerateTurnContext();

        foreach (BaseSynergy synergy in synergies) {
            synergy.CoolDownCount += Time.deltaTime;

            if (synergy.CoolTime <= synergy.CoolDownCount) {
                synergy.CoolTimeEffect(turnContext, _synergyDic[synergy.SynergyType]);
                synergy.CoolDownCount = 0;
            }
        }
    }

    private BaseSynergy FindSynergy(SynergyTypes synergyType) {
        foreach (BaseSynergy synergy in _allSynergyList) {
            if (synergy.SynergyType == synergyType) {
                return synergy;
            }
        }

        return null;
    }

    private List<BaseSynergy> FindSynergies(List<SynergyTypes> synergyTypes) {
        List<BaseSynergy> synergies = new List<BaseSynergy>();

        foreach (SynergyTypes synergyType in _synergyDic.Keys) {
            BaseSynergy synergy = FindSynergy(synergyType);

            if (synergy != null) {
                synergies.Add(synergy);
            }
            else {
                Debug.LogError("synergyType�� �ش��ϴ� synergy�� �������� �ʽ��ϴ�. synergy Type enum Ȥ�� synergy Scriptable Object�� Ȯ�����ּ���.");
            }
        }

        return synergies;
    }

    private List<BaseSynergy> FindActivatedSynergies() {
        List<BaseSynergy> synergies = new List<BaseSynergy>();

        foreach (SynergyTypes synergyType in _synergyDic.Keys) {
            BaseSynergy synergy = FindSynergy(synergyType);


            if (synergy == null) {
                Debug.LogError($"synergyType {synergyType}�� �ش��ϴ� synergy�� �������� �ʽ��ϴ�. synergy Type enum Ȥ�� synergy Scriptable Object�� Ȯ�����ּ���.");
            }
            else if (synergy.MinSynergyCount <= _synergyDic[synergyType]) {
                synergies.Add(synergy);
            }
        }

        return synergies;
    }
}
