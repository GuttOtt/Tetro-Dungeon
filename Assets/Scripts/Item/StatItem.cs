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

    public override void OnBattleStartEffect(TurnContext turnContext) {
        base.OnBattleStartEffect(turnContext);

        List<IUnit> units;

        if (_isForAllAlly) {
            units = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
        }
        else {
            units = GetUnitsOnShape(turnContext.Board);
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
