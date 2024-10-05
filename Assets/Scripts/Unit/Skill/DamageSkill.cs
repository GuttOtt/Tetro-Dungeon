using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
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

        List<IUnit> targets = GetUnitsInAoE(board, mainTarget, mainTarget.Owner);

        int damage = (int)(baseDamage + activator.Attack * attackRatio + activator.SpellPower * spellPowerRatio);

        foreach (IUnit target in targets) {
            target.TakeDamage(turnContext, damage, damageType);
        }
    }

    private List<IUnit> GetUnitsInAoE(Board board, BaseUnit target, CharacterTypes opponentType) {
        Cell targetCell = target.CurrentCell;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        int top = targetRow - aoe.Size.x / 2;
        int left = targetCol - aoe.Size.y / 2;

        List<IUnit> unitsInAoE = board.GetUnitsInArea(aoe, opponentType, top, left);

        return unitsInAoE;
    }
}