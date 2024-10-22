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
        float r = Random.Range(0, 1f);
        Debug.Log($"Skill Chance : {multipliedChance}, r : {r}");
        return r <= multipliedChance;
    }

    public virtual void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target) {
        //�ڽ� Ŭ�������� ���� ����
    }
}

