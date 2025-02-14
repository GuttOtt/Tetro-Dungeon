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
            Debug.LogError("Decorator�� �����Ǿ� ���� �ʽ��ϴ�.");
        }
        if (unitSkill == null) {
            Debug.LogError("��ų �ε����� �ùٸ��� �ʽ��ϴ�.");
        }

        unitSkill.Decorate(_decorator);
    }

    private void Undecorate(UnitSkill unitSkill) {
        if (_decorator == null) {
            Debug.LogError("Decorator�� �����Ǿ� ���� �ʽ��ϴ�.");
        }
        if (unitSkill == null) {
            Debug.LogError("��ų �ε����� �ùٸ��� �ʽ��ϴ�.");
        }

        unitSkill.Undecorate(_decorator);
    }
}
