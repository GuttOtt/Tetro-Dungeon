using System;
using UnityEngine;
using EnumTypes;
using System.Buffers;
using Unity.Mathematics;

public class BaseUnit : MonoBehaviour, IUnit
{
    #region private members
    private UnitSystem _unitSystem;
    private Cell _currentCell;
    private UnitConfig _config;
    private int _maxHP, _maxMP, _currentHP, _currentMP, _attack, _range;
    [SerializeField] private CharacterTypes _owner;
    [SerializeField] private UnitDrawer _unitDrawer;
    #endregion

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }   
    public int Range { get => _range; }
    public Action OnDie { get; set; }
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get => _owner; set => _owner = value; }
    public int CurrentHP { 
        get => _currentHP;
        private set {
            _currentHP = value;
            if (_currentHP < _maxHP) {
                _unitDrawer.UpdateHP(_currentHP, Color.red);
            }
            else if (_maxHP < _currentHP) {
                _unitDrawer.UpdateHP(_currentHP, Color.green);
            }
            else {
                _unitDrawer.UpdateHP(_currentHP, Color.black);
            }
        }
    }
    #endregion


    public void Init(UnitSystem unitSystem, UnitConfig config, CharacterTypes owner) {
        //System
        _unitSystem = unitSystem;

        //Config
        _config = config;

        //Draw
        _unitDrawer = GetComponent<UnitDrawer>();
        _unitDrawer.Draw(config);

        //Stats
        _maxHP = config.MaxHP;
        _currentHP = config.MaxHP;
        _maxMP = config.MaxMP;
        _currentMP = config.MaxMP;
        _attack = config.Attack;
        _range = config.Range;

        _owner = owner;

    }

    public void TakeDamage(int damage) {
        CurrentHP -= damage;
    }

    public void Die() {
        OnDie();
        _unitSystem.DestroyUnit(this);
    }

    public void Highlight() {
        _unitDrawer.Highlight();
    }

    public void Unhighlight() {
        _unitDrawer.Unhighlight();
    }
}
