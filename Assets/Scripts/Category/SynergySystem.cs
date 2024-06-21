using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using EnumTypes;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;

public class SynergySystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;
    private Board _board;

    private Dictionary<SynergyTypes, int> _synergyDic = new Dictionary<SynergyTypes, int>();

    [SerializeField]
    private List<BaseSynergy> _allSynergyList = new List<BaseSynergy>();

    [SerializeField]
    private TMP_Text _synergyText;

    [SerializeField]
    private float _delayPerSynergy = 1f;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();

        _board.onPlaceUnit += UpdateSynergy;

        _allSynergyList = Resources.LoadAll<BaseSynergy>("Scriptable Objects/Synergy").ToList();
    }

    public void UpdateSynergy() {
        Dictionary<SynergyTypes, int> synergyDic = new Dictionary<SynergyTypes, int>();

        List<UnitBlock> unitBlocks = new List<UnitBlock>();
        unitBlocks = _gameManager.GetSystem<UnitBlockSystem>().UnitBlocks.ToList();

        foreach (UnitBlock unitBlock in unitBlocks) {
            foreach (SynergyTypes synergyType in unitBlock.Synergies) {
                if (synergyDic.ContainsKey(synergyType)) {
                    synergyDic[synergyType]++;
                }
                else {
                    synergyDic.Add(synergyType, 1);
                }
            }
        }

        _synergyDic = synergyDic;

        DisplaySynergies();
    }

    private void DisplaySynergies() {
        _synergyText.text = "Synergies: ";
        _synergyText.text += System.Environment.NewLine;

        foreach (SynergyTypes synergyTypes in _synergyDic.Keys) {
            _synergyText.text += $"{synergyTypes} : {_synergyDic[synergyTypes]}";
            _synergyText.text += System.Environment.NewLine;
        }
    }

    public async UniTask OnBattleBeginEffects(TurnContext turnContext) {
        List<BaseSynergy> synergies = FindActivatedSynergies(_synergyDic.Keys.ToList());

        foreach (BaseSynergy synergy in synergies) {
            await UniTask.WaitForSeconds(_delayPerSynergy);

            synergy.OnBattleBegin(turnContext, _synergyDic[synergy.SynergyType]);
        }
    }

    public async UniTask OnTickBegin(TurnContext turnContext) {
        List<BaseSynergy> synergies = FindActivatedSynergies(_synergyDic.Keys.ToList());

        foreach (BaseSynergy synergy in synergies) {
            await UniTask.WaitForSeconds(_delayPerSynergy);

            synergy.OnTickBegin(turnContext, _synergyDic[synergy.SynergyType]);
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

    private List<BaseSynergy> FindActivatedSynergies(List<SynergyTypes> synergyTypes) {
        List<BaseSynergy> synergies = new List<BaseSynergy>();

        foreach (SynergyTypes synergyType in _synergyDic.Keys) {
            BaseSynergy synergy = FindSynergy(synergyType);


            if (synergy == null) {
                Debug.LogError($"synergyType {synergyType}�� �ش��ϴ� synergy�� �������� �ʽ��ϴ�. synergy Type enum Ȥ�� synergy Scriptable Object�� Ȯ�����ּ���.");
            }
            else if (synergy.MinSynergyValue <= _synergyDic[synergyType]) {
                synergies.Add(synergy);
            }
        }

        return synergies;
    }
}
