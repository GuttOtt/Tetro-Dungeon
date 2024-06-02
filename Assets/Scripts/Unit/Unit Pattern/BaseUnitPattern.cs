using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitPattern {
    public class BaseUnitPattern {
        #region Move
        public virtual bool IsMoveable(BaseUnit unit, TurnContext turnContext) {
            CharacterTypes moveTurn = turnContext.MoveTurn;

            if (unit.Owner != moveTurn) {
                return false;
            }

            //유닛의 앞쪽 셀을 가져옴
            Cell forwardCell = GetForwardCell(unit, turnContext.Board);

            //forwardCell에 이미 유닛이 있거나, forwardCell이 존재하지 않는다면 false
            if (forwardCell == null || forwardCell.Unit != null)
                return false;

            return true;
        }

        public virtual void Move(BaseUnit unit, TurnContext turnContext) {
            Cell forwardCell = GetForwardCell(unit, turnContext.Board);

            //유닛 이동
            unit.CurrentCell.UnitOut();
            forwardCell.UnitIn(unit);
            unit.CurrentCell = forwardCell;
        }

        protected Cell GetForwardCell(BaseUnit unit, Board board) {
            Cell currentCell = unit.CurrentCell;

            // 전방으로 한 칸의 위치 계산
            int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
            int targetColumn = currentCell.position.col + forwardOffset;

            //유닛의 앞쪽 셀을 가져옴
            Cell forwardCell = board.GetCell(targetColumn, currentCell.position.row);

            return forwardCell;
        }
        #region Move

        #region Attack
        public virtual bool IsAttackable(BaseUnit unit, TurnContext turnContext) {
            IUnit targetUnit = GetAttackTarget(unit, turnContext.Board);
            if (targetUnit != null)
                return true;
            else
                return false;
        }
        
        public virtual void Attack(BaseUnit unit, TurnContext turnContext) {
            IUnit attackTarget = GetAttackTarget(unit, turnContext.Board);

            attackTarget.TakeDamage(unit.Attack);
        }

        protected IUnit GetAttackTarget(IUnit unit, Board board) {
            int range = unit.Range;
            int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
            Cell currentCell = unit.CurrentCell;
            int originCol = currentCell.position.col;
            int originRow = currentCell.position.row;

            //가까운 유닛을 우선으로 공격
            for (int i = 1; i <= range; i++) {
                Cell targetCell = board.GetCell(originCol + forwardOffset * i, originRow);
                IUnit targetUnit = targetCell.Unit;

                if (targetUnit != null && unit.Owner != targetUnit.Owner) {
                    return targetUnit;
                }
            }

            return null;
        }
        #endregion

        #region Skill?
        public void Skill(BaseUnit unit, TurnContext turnContext) {

        }
        #endregion
    }
}
