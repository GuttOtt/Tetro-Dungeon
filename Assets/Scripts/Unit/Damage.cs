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
}
