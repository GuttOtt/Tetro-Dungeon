using Array2DEditor;
using AYellowpaper.SerializedCollections;
using EnumTypes;
using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "ScriptableObjects/Character", order = 0)]
public class CharacterBlockConfig : ScriptableObject
{
    public string Name;
    public Sprite Illust;
    public string UnitTypeString;
    public LevelShapePair[] Shapes;
    [SerializeField] private int maxLevel = 5;
    public int MaxLevel { get => maxLevel; }

    public SPUM_Prefabs SPUM_Prefabs;
    public Animator Animator_Prefabs;


    #region Stats
    [SerializeField] private Stat _stat;
    [SerializeField] private Stat _statForLevelUp;
    public Stat Stat { get => _stat.DeepCopy(); }
    public Stat StatForLevelUp { get => _statForLevelUp.DeepCopy(); }
    #endregion


    #region Skills
    public SkillConfig DefaultSkill;
    public List<SkillConfig> ActiveSkills;
    public List<SkillConfig> PassiveSkills;
    public List<SkillConfig> Skills
    {
        get
        {
            List<SkillConfig> allSkills = new List<SkillConfig>();
            allSkills.AddRange(ActiveSkills);
            allSkills.AddRange(PassiveSkills);
            return allSkills;
        }
    }
    #endregion

    public List<SynergyPerLevel> SynergyPerLevels = new List<SynergyPerLevel>();
    public Dictionary<SynergyTypes, int> BaseSynergyDict
    {
        get
        {
            return SynergyPerLevels[0].SynergyDic;
        }
    }


    public List<Awakening> Awakenings;

    public Array2DBool GetShape(int lvl) {
        return Shapes[lvl-1].Shape;
    }

    public Vector2Int GetCenterIndex(int lvl) {
        return Shapes[0].CenterIndex;
    }
    public SerializedDictionary<SynergyTypes, int> GetSynergyDict(int lvl) {
        return SynergyPerLevels.Find(x => x.Level == lvl)?.SynergyDic;
    }
}

[Serializable]
public class LevelShapePair
{
    public int Level;
    public Array2DBool Shape;
    public Vector2Int CenterIndex;

    public LevelShapePair() {
        Level = 0;
        Shape = new Array2DBool();
    }
}

[Serializable]
public class SynergyPerLevel {
    public int Level;
    public SerializedDictionary<SynergyTypes, int> SynergyDic;
}