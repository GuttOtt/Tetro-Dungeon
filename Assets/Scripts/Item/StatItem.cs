using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Item", menuName = "Item/Stat Item")]
public class StatItem : Item {
    [SerializeField]
    private bool _isForAllAlly = false;

    [SerializeField]
    private int _attack, _maxHP;

    [SerializeField]
    private float _speed;

    public override void OnSatisfiedEffect(TurnContext turnContext) {
        base.OnBattleStartEffect(turnContext);

        List<IUnit> units;

        if (_isForAllAlly) {
            units = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
            Debug.Log($"{_name}의 효과로 아군 전체에게 공격력 {_attack}, 체력 {_maxHP}, 속도 {_speed} 만큼 버프!");
        }
        else {
            units = GetUnitsOnShape(turnContext.Board);
            Debug.Log($"{_name}의 효과로 공격력 {_attack}, 체력 {_maxHP}, 속도 {_speed} 만큼 버프!");
        }

        BuffUnits(units);
    }

    private void BuffUnits(List<IUnit> units) {
        foreach (BaseUnit unit in units) {
            unit.ChangeAttack(_attack);
            unit.ChangeMaxHP(_maxHP);
            unit.ChangeSpeed(_speed);
        }
    }
}
