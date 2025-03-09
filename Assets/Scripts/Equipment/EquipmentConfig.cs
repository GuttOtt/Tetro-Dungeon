using Array2DEditor;
using AYellowpaper.SerializedCollections;
using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Equipment", order = 0)]
public class EquipmentConfig : ScriptableObject {
    public string Name;
    public Sprite Sprite;
    public Array2DBool Shape;
    [TextArea(3, 10)]
    public string _description;
    [SerializeField] private SerializedDictionary<SynergyTypes, int> _synergyDict;
    [SerializeField] private List<SkillConfig> skills;
    

    #region Stats
    [SerializeField] private Stat _stat;
    public Stat Stat { get => _stat.DeepCopy(); }
    public SerializedDictionary<SynergyTypes, int> SynergyDict { get => _synergyDict; }
    public List<SkillConfig> Skills { get => skills; }
    public string Description { get => _description; }
    #endregion


}
