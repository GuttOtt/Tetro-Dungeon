using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Sniper : BaseUnit
{
    public override void Init(UnitSystem unitSystem, UnitConfig config, CharacterTypes owner) {
        base.Init(unitSystem, config, owner);

        _projectilePrefab = Resources.Load<Projectile>("Prefabs/Unit/Basic Projectile");
    }

    public override void AttackAction(TurnContext turnContext) {
        IUnit farthest = turnContext.Board.GetFarthestUnit(CurrentCell, Owner.Opponent(), Range);

        if (farthest == null || farthest as BaseUnit == null)
            return;

        Projectile projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = transform.position;
        projectile.Init(farthest as BaseUnit, () => farthest.TakeDamage(turnContext, Attack));
    }
}
