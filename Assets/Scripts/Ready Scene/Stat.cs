using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Stat {
    [SerializeField]
    private int _maxHP, _attack, _spellPower, _defence, _spellDefence, _range;
    [SerializeField] private float _speed;

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int Attack { get => _attack; }
    public int SpellPower { get => _spellPower;}
    public int Defence { get => _defence;}
    public int SpellDefence { get => _spellDefence;}
    public float Speed { get => _speed;}
    public int Range { get => _range;}
    #endregion

    public Stat(int maxHP, int attack, int spellPower, int defence, int spellDefence, float speed, int range) {
        _maxHP = maxHP;
        _attack = attack;
        _spellPower = spellPower;
        _defence = defence;
        _spellDefence = spellDefence;
        _speed = speed;
        _range = range;
    }

    public static Stat operator +(Stat a, Stat b) {
        return new Stat(
            a._maxHP + b._maxHP,
            a._attack + b._attack,
            a._spellPower + b._spellPower,
            a._defence + b._defence,
            a._spellDefence + b._spellDefence,
            a._speed + b._speed,
            a._range + b._range
        );
    }
    public static Stat operator -(Stat a, Stat b) {
        return new Stat(
            a._maxHP - b._maxHP,
            a._attack - b._attack,
            a._spellPower - b._spellPower,
            a._defence - b._defence,
            a._spellDefence - b._spellDefence,
            a._speed - b._speed,
            a._range - b._range
        );
    }

    public static Stat operator -(Stat a) {
        return new Stat(-a._maxHP, -a._attack, -a._spellPower, -a._defence, -a._spellDefence, -a._speed, -a._range);
    }

    public static Stat operator *(Stat a, int multiplier) {
        return new Stat(
            a._maxHP * multiplier,
            a._attack * multiplier,
            a._spellPower * multiplier,
            a._defence * multiplier,
            a._spellDefence * multiplier,
            a._speed * multiplier,
            a._range * multiplier
        );
    }

    public Stat PercentageMultiply(Stat percentage) {
        return new Stat(
            (int) (_maxHP * percentage._maxHP/100f),
            (int) (_attack * percentage._attack/100f),
            (int) (_spellPower * percentage._spellPower/100f),
            (int) (_defence * percentage._defence/100f),
            (int) (_spellDefence * percentage._spellDefence/100f),
            _speed * percentage._speed/100f,
            (int) (_range * percentage._range/100f)
        );
    }

    public Stat DeepCopy() {
        return new Stat(_maxHP, _attack, _spellPower, _defence, _spellDefence, _speed, _range);
    }
}
