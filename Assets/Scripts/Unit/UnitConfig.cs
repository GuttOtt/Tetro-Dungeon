using EnumTypes;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitConfig", order = 0)]
public class UnitConfig : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private int _maxHp, _maxMP, _attack, _range, _unitTypeValue;

    [SerializeField]
    private string _unitTypeString;

    [SerializeField]
    private string _effectDescription;

    [SerializeField]
    private List<SynergyTypes> _synergies = new List<SynergyTypes>();

    public string Name { get => base.name; }
    public Sprite Sprite { get => _sprite; }
    public int MaxHP { get => _maxHp; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }
    public int Range { get => _range; }
    public int UnitTypeValue { get => _unitTypeValue; }
    public string UnitTypeString => _unitTypeString;
    public string EffectDescription => _effectDescription;
    public List<SynergyTypes> Synergies { get => _synergies; }
}