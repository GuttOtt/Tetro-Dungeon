using System;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    [Serializable]
    public struct Position
    {
        public int row;
        public int col;
    }

    [SerializeField]
    private BaseUnit _baseUnit;

    private IUnit _unit;
    [SerializeField]
    private Position _position;
    [SerializeField]
    private EnumTypes.CharacterTypes characterType;

    public Position position { get => _position; }
    public IUnit Unit { get => _unit; }
    public EnumTypes.CharacterTypes CharacterType { get => characterType; }

    public void Init(int x, int y, EnumTypes.CharacterTypes characterType) {
        _position.col = x;
        _position.row = y;
        this.characterType = characterType;
    }

    public bool UnitIn(IUnit unit) {
        if (_unit != null || unit == null) {//이미 유닛이 있다면, false를 반환하고 종료
            return false;
        }
        else {
            unit.CurrentCell?.UnitOut();
            _unit = unit;
            _baseUnit = unit as BaseUnit;
            (unit as BaseUnit).OnDestroy += () => UnitOut();
            (unit as BaseUnit).transform.parent = transform;

            return true;
        }
    }

    public IUnit UnitOut() {
        if (_unit == null) return null;

        (_unit as BaseUnit).OnDestroy -= () => UnitOut();
        IUnit temp = _unit;
        _unit = null;
        _baseUnit = null;
        return temp;
    }
}
