using Cysharp.Threading.Tasks.Triggers;
using EnumTypes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public IUnit CreateUnit(UnitConfig unitConfig, CharacterTypes owner) {
        GameObject unitGO = Instantiate(_unitPrefab);

        //유닛 패턴 생성
        string unitTypeString = unitConfig.UnitTypeString;
        if (unitTypeString == string.Empty || unitTypeString == null) {
            unitTypeString = "BaseUnit";
        }

        Type unitType = Type.GetType(unitTypeString);

        if (unitType == null) {
            Debug.Log(unitType);
            Debug.LogError("해당하는 unit type이 없습니다");
        }

        BaseUnit unit = unitGO.AddComponent(unitType) as BaseUnit;

        unit.Init(this, unitConfig, owner);
        _units.Add(unit);

        unit.OnDestroy += () => DestroyUnit(unit);

        return unit;
    }
    
    public void DestroyUnit(IUnit unit) {
        BaseUnit baseUnit = unit as BaseUnit;
        if (baseUnit == null) return;
        
        _units.Remove(baseUnit);
        Destroy(baseUnit.gameObject);
    }
}
