using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BaseSynergy:ScriptableObject {
    [SerializeField]
    protected SynergyTypes _synergyType;
    [SerializeField]
    protected int _minSynergyValue; // minimum synergy value to be activated

    public virtual SynergyTypes SynergyType { get => _synergyType; }
    public virtual int MinSynergyValue { get => _minSynergyValue; }

    public virtual void Init() {

    }

    public virtual void OnBattleBegin(TurnContext turnContext, int synergyValue) {

    }

    public virtual void OnTickBegin(TurnContext turnContext, int synergyValue) {

    }

}
