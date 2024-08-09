using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoE Damage", menuName = "Enemy/Enemy Effect/AoE Damage")]
public class EnemyEffect_AoEDamage : EnemyEffect {
    [SerializeField]
    private TArray<bool> aoe = new TArray<bool>();

    [SerializeField]
    private int damage;

    private bool[,] AoE { get => aoe.GetArray<bool>(); }

    public override void OnBattleBeginEffect(TurnContext turnContext) {
        base.OnBattleBeginEffect(turnContext);

        ApplyDamage(turnContext);
    }

    public override void CoolTimeEffect(TurnContext turnContext) {
        base.CoolTimeEffect(turnContext);

        ApplyDamage(turnContext);
    }

    private void ApplyDamage(TurnContext turnContext) {
        List<IUnit> units = turnContext.Board.GetUnitsInArea(AoE, EnumTypes.CharacterTypes.Player);

        foreach(IUnit unit in units) {
            unit.TakeDamage(turnContext, damage);
        }
    }
}
