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
    [SerializeField] private Stat _stat;
    public Stat Stat { get => _stat.DeepCopy(); }
    #endregion


}
