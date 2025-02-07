using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Double Attack Status Config", menuName = "ScriptableObjects/Status/DoubleAttackStatusConfig")]
public class DoubleAttackStatusConfig : StatusConfig {
    [SerializeField] private float _doubleAttackChance;

    public float DoubleAttackChance { get => _doubleAttackChance; }
}

public class DoubleAttackStatus : Status {
    [SerializeField] private float _doubleAttackChance;

    public float DoubleAttackChance { get => _doubleAttackChance; }

    public DoubleAttackStatus(DoubleAttackStatusConfig config) : base(config) {
        _doubleAttackChance = config.DoubleAttackChance;
    }

    public override void ApplyTo(StatusApplicationContext context) {
        BaseUnit unit = context.TargetUnit;
        foreach(UnitEventTypes eventType in UnitEvents) {
            Register(unit, eventType);
        }
    }

    private void Register(BaseUnit unit, UnitEventTypes eventType) {
        switch (eventType) {
            case UnitEventTypes.OnAttacked:
                unit.onAttacked += ApplyDoubleAttack;
                break;
            case UnitEventTypes.OnAttacking:
                unit.onAttacking += ApplyDoubleAttack;
                break;
        }
    }

    public override void RemoveFrom(BaseUnit unit) {
        foreach(UnitEventTypes eventType in UnitEvents) {
            Unregister(unit, eventType);
        }
    }

    private void Unregister(BaseUnit unit, UnitEventTypes eventType) {
        switch (eventType) {
            case UnitEventTypes.OnAttacked:
                unit.onAttacked -= ApplyDoubleAttack;
                break;
            case UnitEventTypes.OnAttacking:
                unit.onAttacking -= ApplyDoubleAttack;
                break;
        }
    }

    private bool ApplyDoubleAttack(BaseUnit attacker, BaseUnit target, TurnContext turnContext) {
        Debug.Log("Apply Double Attack, doubleAttackChance: " + _doubleAttackChance);
        if(UnityEngine.Random.value <= _doubleAttackChance) {
            Debug.Log("Chance Success");
            DoubleAttack(attacker, target, turnContext).Forget();
        }

        return false;
    }

    private async UniTask<bool> DoubleAttack(BaseUnit attacker, BaseUnit target, TurnContext turnContext) {
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        attacker.AttackAction(turnContext, false);
        Debug.Log("Double Attack");
        return false;
    }

    public void SetDoubleAttackChance(int value) => _doubleAttackChance = value/100f;

}