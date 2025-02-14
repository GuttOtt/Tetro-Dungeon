using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Add Skill Awakening", menuName = "ScriptableObjects/Awakening/Add Skill", order = 0)]
public class AddSkillAwakening : Awakening {
    [SerializeField] private SkillConfig skillConfig;
    [SerializeField] private SkillTypes skillType;
    private Dictionary<CharacterBlock, UnitSkill> activationDic = new Dictionary<CharacterBlock, UnitSkill>();

    public override bool UpdateActivation(CharacterBlock characterBlock) {
        if (!activationDic.ContainsKey(characterBlock)) {
            activationDic.Add(characterBlock, null);
        }

        bool isActivated = activationDic[characterBlock] != null;

        if (!isActivated && _condition.IsSatisfied(characterBlock)) {
            activationDic[characterBlock] = AddSkill(characterBlock);
            Debug.Log("Added");
            return true;
        }
        else if (isActivated && !_condition.IsSatisfied(characterBlock)) {
            RemoveSkill(characterBlock);
            activationDic[characterBlock] = null;
            Debug.Log("Removed");
            return false;
        }
        else {
            return activationDic[characterBlock] != null;
        }
    }

    private UnitSkill AddSkill(CharacterBlock characterBlock) {
        if (characterBlock == null) {
            Debug.LogError("CharacterBlock이 지정되어 있지 않습니다.");
        }

        UnitSkill skill = SkillFactory.CreateSkill(skillConfig);

        if (skillType == SkillTypes.Active) {
            characterBlock.AddActiveSkill(skill);
        }
        else {
            characterBlock.AddPassiveSkill(skill);
        }

        return skill;
    }

    private void RemoveSkill(CharacterBlock characterBlock) {
        if (characterBlock == null) {
            Debug.LogError("CharacterBlock이 지정되어 있지 않습니다.");
        }
        
        UnitSkill skill = activationDic[characterBlock];
        if (skill == null) {
            Debug.LogError("스킬이 지정되어 있지 않습니다.");
        }
        else if (skillType == SkillTypes.Active) {
            characterBlock.RemoveActiveSkill(skill);
        }
        else {
            characterBlock.RemovePassiveSkill(skill);
        }
    }
}
