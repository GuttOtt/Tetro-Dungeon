using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Equipment", order = 0)]
public class EquipmentConfig : ScriptableObject {
    public string Name;
    public Sprite Sprite;
    public Array2DBool Shape;

    #region Stats
    public int MaxHP;
    public int Attack;
    public int SpellPower;
    public int Defence;
    public int SpellDefence;
    public int Range;
    public int Speed;
    #endregion


}
