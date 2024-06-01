using Cysharp.Threading.Tasks.Triggers;
using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    [SerializeField] private BaseUnit _unitPrefab;
    private List<IUnit> _units = new List<IUnit>();
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    public IUnit CreateUnit(UnitConfig unitConfig, CharacterTypes owner) {
        BaseUnit unit = Instantiate(_unitPrefab);
        unit.Init(this, unitConfig, owner);
        _units.Add(unit);

        return unit;
    }
    
    public void DestroyUnit(IUnit unit) {
        BaseUnit baseUnit = unit as BaseUnit;

        _units.Remove(baseUnit);
        Destroy(baseUnit.gameObject);
    }
}
