using Cysharp.Threading.Tasks.Triggers;
using EnumTypes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class UnitSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    [SerializeField] private GameObject _unitPrefab;
    private List<IUnit> _units = new List<IUnit>();
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

      

    public BaseUnit CreateUnit(CharacterBlockConfig config, CharacterTypes owner, int level = 1) {
        GameObject unitGO = Instantiate(_unitPrefab);

        //���� ���� ����
        string unitTypeString = config.UnitTypeString;
        if (unitTypeString == string.Empty || unitTypeString == null) {
            unitTypeString = "BaseUnit";
        }

        Type unitType = Type.GetType(unitTypeString);

        if (unitType == null) {
            Debug.Log(unitType);
            Debug.LogError("�ش��ϴ� unit type�� �����ϴ�");
        }

        BaseUnit unit = unitGO.AddComponent(unitType) as BaseUnit;

        //���� �ν��Ͻ� �ĺ��� ���Ǵ� ���� id
        int id = _units.Count;

        unit.Init(this, config, owner, id);
        _units.Add(unit);

        // ������ ���� ���� ����
        Stat statForLevelUp = config.StatForLevelUp;
        Stat statGain = statForLevelUp * (level - 1);
        unit.GainStat(statGain);

        unit.OnDestroy += () => DestroyUnit(unit);

        return unit;
    }

    public BaseUnit CreateUnit(CharacterBlock characterBlock, CharacterTypes owner) {
        GameObject unitGO = Instantiate(_unitPrefab);

        //���� ���� ����
        string unitTypeString = characterBlock.Config.UnitTypeString;
        if (unitTypeString == string.Empty || unitTypeString == null) {
            unitTypeString = "BaseUnit";
        }

        Type unitType = Type.GetType(unitTypeString);

        if (unitType == null) {
            Debug.Log(unitType);
            Debug.LogError("�ش��ϴ� unit type�� �����ϴ�");
        }

        BaseUnit unit = unitGO.AddComponent(unitType) as BaseUnit;

        //���� �ν��Ͻ� �ĺ��� ���Ǵ� ���� id
        int id = _units.Count;

        unit.Init(this, characterBlock, owner, id);
        _units.Add(unit);

        unit.OnDestroy += () => DestroyUnit(unit);

        return unit;
    }
    
    public void DestroyUnit(IUnit unit) {
        BaseUnit baseUnit = unit as BaseUnit;
        if (baseUnit == null) return;
        
        _units.Remove(baseUnit);
        DelayedDestroy(baseUnit);
    }

    private async void DelayedDestroy(BaseUnit baseUnit) {
        await Cysharp.Threading.Tasks.UniTask.Delay(1000);
        if (baseUnit == null|| baseUnit.gameObject == null)
            return;
        Destroy(baseUnit.gameObject);
    }
}
