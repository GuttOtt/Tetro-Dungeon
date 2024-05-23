using System;
using UnityEngine;
using EnumTypes;

public class BaseUnit : MonoBehaviour, IUnit
{
    private Cell _currentCell;
    private UnitConfig _config;
    private int _maxHP, _maxMP, _currentHP, _currentMP, _attack, _range;

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }   
    public int Range { get => _range; }
    public Action OnDie { get; set; }
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get; }
    #endregion

    public void Init(int maxHP, int maxMP, int attack, int range) {
        _maxHP = maxHP;
        _currentHP = maxHP;
        _maxMP = maxMP;
        _currentMP = maxMP;
        _attack = attack;
        _range = range;
    }

    public void Init(UnitConfig config) {
        _config = config;
        Init(config.MaxHP, config.MaxMP, config.Attack, config.Range);
        GetComponent<UnitDrawer>().Draw(config);
    }

    public void TakeDamage(int damage) {
        _currentHP -= damage;
        if (_currentHP <= 0) {
            Die();
        }
    }

    public void Die() {
        OnDie();
    }
}
