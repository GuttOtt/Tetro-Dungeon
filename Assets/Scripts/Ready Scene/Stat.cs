using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat {
    [SerializeField]
    private int _maxHP, _attack, _spellPower, _defence, _spellDefence, _speed, _range;

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int Attack { get => _attack; }
    public int SpellPower { get => _spellPower;}
    public int Defence { get => _defence;}
    public int SpellDefence { get => _spellDefence;}
    public int Speed { get => _speed;}
    public int Range { get => _range;}
    #endregion

    public Stat(int maxHP, int attack, int spellPower, int defence, int spellDefence, int speed, int range) {
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

    public Stat DeepCopy() {
        return new Stat(_maxHP, _attack, _spellPower, _defence, _spellDefence, _speed, _range);
    }
}
