using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Skill", menuName = "ScriptableObjects/Skill/DamageSkill", order = 0)]
public class DamageSkill : ActiveSkill
{
    [SerializeField] protected DamageTypes damageType;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected float attackRatio;
    [SerializeField] protected float spellPowerRatio;
    [SerializeField] protected TArray<bool> aoe = new TArray<bool>();

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit mainTarget) {
        Board board = turnContext.Board;

        List<IUnit> targets = GetUnitsInAoE(board, activator, mainTarget, mainTarget.Owner);

        int damage = (int)(baseDamage + activator.Attack * attackRatio + activator.SpellPower * spellPowerRatio);
        
        foreach (IUnit target in targets) {
            target.TakeDamage(turnContext, damage, damageType);
        }
        
    }

    private List<IUnit> GetUnitsInAoE(Board board, BaseUnit activator, BaseUnit target, CharacterTypes opponentType) {
        //위치에 따른 회전을 적용해야 함
        Cell targetCell = target.CurrentCell;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] adjustedAoE = AdjustAoE(activator, target);

        int top = targetRow - adjustedAoE.GetLength(1) / 2;
        int left = targetCol - adjustedAoE.GetLength(0) / 2;


        List<IUnit> unitsInAoE = board.GetUnitsInArea(adjustedAoE, opponentType, top, left);

        return unitsInAoE;
    }

    private bool[,] AdjustAoE(BaseUnit activator, BaseUnit mainTarget) {
        //스킬 시전 유닛과 타겟 유닛의 상대적 위치에 따라 AoE를 조정함

        int xActivator = activator.CurrentCell.position.col;
        int yActivator = activator.CurrentCell.position.row;
        int xTarget = mainTarget.CurrentCell.position.col;
        int yTarget = mainTarget.CurrentCell.position.row;

        int xOffset = xTarget - xActivator;
        int yOffset = yTarget - yActivator;

        bool[,] adjusted;

        
        //적이 우측에 있는 경우
        if (0 < xOffset) {
            adjusted = aoe.GetArray<bool>();
        }
        //적이 좌측에 있는 경우
        else if (xOffset < 0) {
            adjusted = Utils.HorizontalFlip<bool>(aoe);
        }
        //적이 아래에 있는 경우
        else if (yOffset < 0) {
            adjusted = Utils.RotateRight<bool>(aoe);
        }
        //적이 위에 있는 경우
        else {
            adjusted = Utils.RotateLeft<bool>(aoe);
        }

        return adjusted;
    }
}