using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunnerSynergy : BaseSynergy {
    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        Debug.Log("총잡이 시너지 효과 발동");

        Board board = turnContext.Board;

        List<IUnit> playerUnits = board.GetUnits(CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            int originCol = unit.CurrentCell.position.col;
            int originRow = unit.CurrentCell.position.row;

            IUnit targetUnit = null;

            for (int i = originCol + 1; i < board.Column; i++) {
                Cell targetCell = board.GetCell(i, originRow);

                if (targetCell != null && targetCell.Unit != null && targetCell.Unit.Owner == CharacterTypes.Enemy) {
                    targetUnit = targetCell.Unit;
                    break;
                }
            }

            if (targetUnit != null) {
                targetUnit.TakeDamage(turnContext, synergyCount * (int)_synergyValue);
            }
        }
    }
}
