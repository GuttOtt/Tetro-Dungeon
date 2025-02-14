using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Decorate Skill Awakening", menuName = "ScriptableObjects/Awakening/Decorate Skill", order = 0)]
public class DecorateSkillAwakening : Awakening {
    [SerializeField] private int _skillIndex;
    [SerializeField] private SkillConfig _decorator;
    private Dictionary<UnitSkill, bool> _activationDic= new Dictionary<UnitSkill, bool>();

    public override bool UpdateActivation(CharacterBlock characterBlock) {
        UnitSkill skill = characterBlock.Skills[_skillIndex];
        
        if (!_activationDic.ContainsKey(skill)) { 
            _activationDic.Add(skill, false);
        }

        bool isActivated = _activationDic[skill];

        if (!isActivated && _condition.IsSatisfied(characterBlock)) {
            Decorate(characterBlock.Skills[_skillIndex]);
            _activationDic[skill] = true;
            Debug.Log("Decorated");
            return true;
        }
        else if (isActivated && !_condition.IsSatisfied(characterBlock)) {
            Undecorate(characterBlock.Skills[_skillIndex]);
            _activationDic[skill] = false;
            Debug.Log("Undecorated");
            return false;
        }
        else {
            return _activationDic[skill];
        }
    }

    private void Decorate(UnitSkill unitSkill) {
        if (_decorator == null) {
            Debug.LogError("Decorator가 지정되어 있지 않습니다.");
        }
        if (unitSkill == null) {
            Debug.LogError("스킬 인덱스가 올바르지 않습니다.");
        }

        unitSkill.Decorate(_decorator);
    }

    private void Undecorate(UnitSkill unitSkill) {
        if (_decorator == null) {
            Debug.LogError("Decorator가 지정되어 있지 않습니다.");
        }
        if (unitSkill == null) {
            Debug.LogError("스킬 인덱스가 올바르지 않습니다.");
        }

        unitSkill.Undecorate(_decorator);
    }
}
