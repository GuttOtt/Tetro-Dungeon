using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSynergy: ScriptableObject {
    [SerializeField]
    protected SynergyTypes _synergyType;
    [SerializeField]
    protected int _minSynergyCount; // minimum synergy Count to be activated
    [SerializeField]
    protected float _synergyValue;
    [SerializeField]
    private float _coolTime = 0f;

    private float _coolDownCount = 0f;

    public virtual SynergyTypes SynergyType { get => _synergyType; }
    public virtual int MinSynergyCount { get => _minSynergyCount; }
    public float CoolTime { get => _coolTime;}
    public float CoolDownCount { get => _coolDownCount; set => _coolDownCount = value; }

    public virtual void Init() {

    }

    public virtual void OnBattleBegin(TurnContext turnContext, int synergyValue) {

    }

    public virtual void CoolTimeEffect(TurnContext turnContext, int synergyValue) {
        
    }
}
