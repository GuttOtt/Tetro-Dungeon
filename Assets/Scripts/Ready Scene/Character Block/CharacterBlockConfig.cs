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
    public int MaxLevel { get => Shapes.Length; }

    public SPUM_Prefabs SPUM_Prefabs;

    #region Stats
    public int MaxHP;
    public int Attack;
    public int SpellPower;
    public int Defence;
    public int SpellDefence;
    public int Range;
    public int Speed;
    #endregion

    #region Skills
    public ActiveSkill DefaultSkill;
    public List<ActiveSkill> ActiveSkills;
    #endregion

    public List<SynergyValuePair> Synergies;

    public Array2DBool GetShape(int lvl) {
        return Shapes[lvl].Shape;
    }

    public Vector2Int GetCenterIndex(int lvl) {
        return Shapes[lvl].CenterIndex;
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
public class SynergyValuePair {
    public SynergyTypes SynergyType;
    public int Value;
}