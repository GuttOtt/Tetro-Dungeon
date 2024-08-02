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
            this._unit = unit;
            (unit as BaseUnit).OnDestroy += () => UnitOut();

            //Transform 이동
            Transform unitTransform = (unit as BaseUnit).transform;
            unitTransform.parent = this.transform;
            unitTransform.DOMove(transform.position, unit.Speed * 0.8f).SetEase(Ease.OutBack, 2f);

            return true;
        }
    }

    public IUnit UnitOut() {
        if (_unit == null) return null;

        (_unit as BaseUnit).OnDestroy -= () => UnitOut();
        var temp = _unit;
        _unit = null;
        return temp;
    }
}
