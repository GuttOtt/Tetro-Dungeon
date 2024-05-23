using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    [Serializable]
    public struct Position {
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
        if (this._unit != null) {//이미 유닛이 있다면, false를 반환하고 종료
            return false;
        }
        else {
            this._unit = unit;
            return true;
        }
    }

    public IUnit UnitOut() {
        if (_unit == null) return null;

        var temp = _unit;
        _unit = null;
        return temp;
    }
}
