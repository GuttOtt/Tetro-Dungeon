using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using EnumTypes;

public class UnitSkill : ScriptableObject {
    
}

public class ActiveSkill : UnitSkill {
    [SerializeField] protected float chance;

    public bool CheckChance(float chanceMultiplier) {
        float multipliedChance = chance * (1 + chanceMultiplier);
        Debug.Log($"Skill Chance : {multipliedChance}");
        return Random.Range(0, 1) <= multipliedChance;
    }

    public virtual void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target) {
        //자식 클래스에서 세부 구현
    }
}

