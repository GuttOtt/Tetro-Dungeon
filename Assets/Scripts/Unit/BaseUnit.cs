using System;
using UnityEngine;
using EnumTypes;
using System.Buffers;
using Unity.Mathematics;

public class BaseUnit : MonoBehaviour, IUnit
{
    private Cell _currentCell;
    private UnitConfig _config;
    private int _maxHP, _maxMP, _currentHP, _currentMP, _attack, _range;
    [SerializeField] private CharacterTypes _owner;

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }   
    public int Range { get => _range; }
    public Action OnDie { get; set; }
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get => _owner; set => _owner = value; }
    #endregion


    public void Init(UnitConfig config, CharacterTypes owner) {
        //Config
        _config = config;

        //Stats
        _maxHP = config.MaxHP;
        _currentHP = config.MaxHP;
        _maxMP = config.MaxMP;
        _currentMP = config.MaxMP;
        _attack = config.Attack;
        _range = config.Range;

        _owner = owner;

        //Draw
        GetComponent<UnitDrawer>().Draw(config);
    }

    public void TakeDamage(int damage) {
        _currentHP -= damage;
    }

    public void Die() {
        OnDie();
    }
}
