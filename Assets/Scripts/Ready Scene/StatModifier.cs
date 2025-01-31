using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatFloat  {
    private float attack, defence, maxHP, spellPower, spellDefence, speed, range;

    public StatFloat(float attack, float defence, float maxHP, float spellPower, float spellDefence, float speed, float range) {
        this.attack = attack;
        this.defence = defence;
        this.maxHP = maxHP;
        this.spellPower = spellPower;
        this.spellDefence = spellDefence;
        this.speed = speed;
        this.range = range;
    }

    public float Attack { get => attack; }
    public float Defence { get => defence; }
    public float MaxHP { get => maxHP; }
    public float SpellPower { get => spellPower; }
    public float SpellDefence { get => spellDefence; }
    public float Speed { get => speed; }
    public float Range { get => range; }

    public static StatFloat operator *(StatFloat a, float multiplier) {
        return new StatFloat(
            a.attack * multiplier,
            a.defence * multiplier,
            a.maxHP * multiplier,
            a.spellPower * multiplier,
            a.spellDefence * multiplier,
            a.speed * multiplier,
            a.range * multiplier
        );
    }
}
