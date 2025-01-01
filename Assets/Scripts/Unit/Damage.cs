using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class Damage {
    public Dictionary<DamageTypes, int> damageDic = new Dictionary<DamageTypes, int>() {
        { DamageTypes.Attack, 0 },
        { DamageTypes.Spell, 0 },
        { DamageTypes.True, 0 }
    };
    
    public Damage(int attackDamage = 0, int spellDamage = 0, int trueDamage = 0) {
        damageDic[DamageTypes.Attack] = attackDamage;
        damageDic[DamageTypes.Spell] = spellDamage;
        damageDic[DamageTypes.True] = trueDamage;
    }

    public Damage(DamageTypes damageType, int amount) {
        damageDic[damageType] = amount;
    }

    public int GetDamage(DamageTypes damageType) {
        return damageDic[damageType];
    }

    public void SetDamage(DamageTypes damageType, int damage) {
        damageDic[damageType] = damage;
    }

    public int GetSum() {
        int sum = 0;
        foreach (int damage in damageDic.Values) {
            sum += damage;
        }
        return sum;
    }

    public static Damage zero = new Damage();


    #region Operations
    // + 연산자 오버로딩
    public static Damage operator +(Damage a, Damage b)
    {
        return new Damage(
            a.damageDic[DamageTypes.Attack] + b.damageDic[DamageTypes.Attack],
            a.damageDic[DamageTypes.Spell] + b.damageDic[DamageTypes.Spell],
            a.damageDic[DamageTypes.True] + b.damageDic[DamageTypes.True]
        );
    }

    // - 연산자 오버로딩
    public static Damage operator -(Damage a, Damage b)
    {
         return new Damage(
            a.damageDic[DamageTypes.Attack] - b.damageDic[DamageTypes.Attack],
            a.damageDic[DamageTypes.Spell] - b.damageDic[DamageTypes.Spell],
            a.damageDic[DamageTypes.True] - b.damageDic[DamageTypes.True]
        );
    }

    // * 연산자 오버로딩 (스칼라 곱셈)
    public static Damage operator *(Damage a, int scalar)
    {
        return new Damage(
            a.damageDic[DamageTypes.Attack] * scalar,
            a.damageDic[DamageTypes.Spell] * scalar,
            a.damageDic[DamageTypes.True] * scalar
        );
    }

    public static Damage operator *(Damage a, float scalar)
    {
        return new Damage(
            (int) (a.damageDic[DamageTypes.Attack] * scalar),
            (int) (a.damageDic[DamageTypes.Spell] * scalar),
            (int) (a.damageDic[DamageTypes.True] * scalar)
        );
    }

    // * 연산자 오버로딩 (Damage 곱셈)
    public static Damage operator *(Damage a, Damage b)
    {
        return new Damage(
            a.damageDic[DamageTypes.Attack] * b.damageDic[DamageTypes.Attack],
            a.damageDic[DamageTypes.Spell] * b.damageDic[DamageTypes.Spell],
            a.damageDic[DamageTypes.True] * b.damageDic[DamageTypes.True]
        );
    }

     // / 연산자 오버로딩 (스칼라 나눗셈)
     public static Damage operator /(Damage a, int scalar)
    {
        if (scalar == 0)
        {
            throw new System.DivideByZeroException("Cannot divide by zero.");
        }
        return new Damage(
            a.damageDic[DamageTypes.Attack] / scalar,
            a.damageDic[DamageTypes.Spell] / scalar,
            a.damageDic[DamageTypes.True] / scalar
        );
    }

    public static Damage operator /(Damage a, float scalar)
    {
        return new Damage(
            (int) (a.damageDic[DamageTypes.Attack] / scalar),
            (int) (a.damageDic[DamageTypes.Spell] / scalar),
            (int) (a.damageDic[DamageTypes.True] / scalar)
        );
    }

    // / 연산자 오버로딩 (Damage 나눗셈)
    public static Damage operator /(Damage a, Damage b)
    {
        if (b.damageDic[DamageTypes.Attack] == 0 || b.damageDic[DamageTypes.Spell] == 0 || b.damageDic[DamageTypes.True] == 0)
        {
            throw new System.DivideByZeroException("Cannot divide by zero.");
        }

        return new Damage(
            a.damageDic[DamageTypes.Attack] / b.damageDic[DamageTypes.Attack],
            a.damageDic[DamageTypes.Spell] / b.damageDic[DamageTypes.Spell],
            a.damageDic[DamageTypes.True] / b.damageDic[DamageTypes.True]
        );
    }
    #endregion
}
