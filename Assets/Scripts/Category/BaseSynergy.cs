using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BaseSynergy:ScriptableObject {
    [SerializeField]
    protected SynergyTypes _synergyType;
    [SerializeField]
    protected int _minSynergyCount; // minimum synergy value to be activated
    [SerializeField]
    protected float _synergyValue;

    public virtual SynergyTypes SynergyType { get => _synergyType; }
    public virtual int MinSynergyCount { get => _minSynergyCount; }

    public virtual void Init() {

    }

    public virtual void OnBattleBegin(TurnContext turnContext, int synergyValue) {

    }

    public virtual void OnTickBegin(TurnContext turnContext, int synergyValue) {

    }

}
