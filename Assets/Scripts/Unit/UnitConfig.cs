using EnumTypes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[SerializeField]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitConfig", order = 0)]
public class UnitConfig : ScriptableObject
{
    [SerializeField]
    private string _name, _effectDescription;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private int _maxHp, _maxMP, _attack, _range, _unitTypeValue;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private string _unitTypeString;

    [SerializeField]
    private List<SynergyTypes> _synergies = new List<SynergyTypes>();

    [SerializeField]
    private Projectile _projectile;

    [SerializeField]
    public ActiveSkill DefaultSkill;
    [SerializeField]
    public List<ActiveSkill> ActiveSkills = new List<ActiveSkill>();

    public Sprite Sprite { get => _sprite; }
    public int MaxHP { get => _maxHp; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }
    public int Range { get => _range; }
    public float Speed { get => _speed; }
    public string UnitTypeString { get => _unitTypeString; }
    public int UnitTypeValue { get => _unitTypeValue; }
    public List<SynergyTypes> Synergies { get => _synergies; }
    public string Name { get => _name; }
    public string EffectDescription { get => _effectDescription; }
    public Projectile Projectile { get => _projectile; }

    public void Init(UnitConfig unit)
    {
        _name = unit.name;
        _effectDescription = unit.EffectDescription;
        _sprite = unit.Sprite;
        _maxHp = unit.MaxHP;
        _maxMP = unit.MaxMP;
        _attack = unit.Attack;
        _range = unit.Range;
        _speed = unit.Speed;
        _unitTypeString = unit.UnitTypeString;
        _unitTypeValue = unit.UnitTypeValue;
        _synergies = new List<SynergyTypes>(unit.Synergies);
    }
}