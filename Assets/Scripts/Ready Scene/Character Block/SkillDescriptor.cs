using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SkillDescriptor : MonoBehaviour {
    static readonly Regex SkillDescriptionRegex = new Regex(@"\{(.+?)\}");

    [SerializeField] private TMP_Text _nameText, _descriptionText;

    public void DescribeSkill(UnitSkill skill) {
        string skillName = skill.SkillName;
        string description = RegexDescription(skill, skill.SkillDescription);

        _nameText?.SetText(skillName);
        _descriptionText?.SetText(description);
    }

    private string RegexDescription(UnitSkill skill, string template) {
        var type = skill.GetType();
        Debug.Log($"{type}");
        return SkillDescriptionRegex.Replace(template, match => {
            string fieldName = match.Groups[1].Value; // �߰�ȣ ���� �� ����
            Debug.Log($"{fieldName}");
            var property = type.GetProperty(fieldName);
            if (property != null) {
                return property.GetValue(skill)?.ToString() ?? match.Value;
            }
            return match.Value; // ��ġ�ϴ� ������Ƽ�� ������ ���� �� ��ȯ
        });
    }
}
