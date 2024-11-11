using Array2DEditor;
using AYellowpaper.SerializedCollections;
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
    public UnitConfig UnitConfig;
    public LevelShapePair[] Shapes;
    public Array2DBool GetShape(int lvl) {
        return Shapes[lvl].Shape;
    }
}

[Serializable]
public class LevelShapePair
{
    public int Level;
    public Array2DBool Shape;

    public LevelShapePair() {
        Level = 0;
        Shape = new Array2DBool();
    }
}