using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fear Status Config", menuName = "ScriptableObjects/Status/FearStatusConfig")]
public class FearStatusConfig : StatusConfig {

    
}

public class FearStatus : Status 
{
    private BaseUnit _source;

    public FearStatus(StatusConfig config) : base(config) { }

    public override void ApplyTo(StatusApplicationContext context)
    {
        _source = context.ActivatorUnit;
        context.TargetUnit.OnActionStart += ApplyFear;
    }

    public override void RemoveFrom(BaseUnit unit)
    {
        unit.OnActionStart -= ApplyFear;
    }


    private bool ApplyFear(BaseUnit unit, TurnContext turnContext)
    {
        Debug.Log($"공포 효과 적용. {unit.Name}이(가) 1회동안 행동하지 않습니다.");
        unit.RemoveStatus(this);
        return true; // 공포 대상이 사라지면 공포 효과 종료
    }
}