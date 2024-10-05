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
        return Random.Range(0, 1) <= chance * chanceMultiplier;
    }

    public virtual void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target) {
        //자식 클래스에서 세부 구현
    }
}

