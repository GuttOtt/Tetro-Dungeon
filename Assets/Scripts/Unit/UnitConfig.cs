using EnumTypes;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitConfig", order = 0)]
public class UnitConfig : ScriptableObject
{
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

    public Sprite Sprite { get => _sprite; }
    public int MaxHP { get => _maxHp; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }
    public int Range { get => _range; }
    public float Speed { get => _speed; }
    public string UnitTypeString { get => _unitTypeString; }
    public int UnitTypeValue { get => _unitTypeValue; }
    public List<SynergyTypes> Synergies { get => _synergies; }
}