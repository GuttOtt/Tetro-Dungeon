using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterBlockData  {
    public CharacterBlockConfig Config;
    public int Level;
    public Vector2Int CenterCellIndex;
    public int SpinDegree;
    public List<EquipmentData> Equipments;

    public CharacterBlockData(CharacterBlockConfig config, int level, 
        Vector2Int centerCellPos = default(Vector2Int), int spinDegree = 0,
        List<EquipmentData> equipments = null) {
        Config = config;
        Level = level;
        CenterCellIndex = centerCellPos;
        SpinDegree = spinDegree;
        Equipments = equipments;
    }
}
