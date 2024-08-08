using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopCardSystem : MonoBehaviour
{
    #region private members
    [SerializeField]
    private List<TroopEffect> _allTroopEffect;
    private List<StatDecorator> _allStatDecorator;
    #endregion

    private void Awake() {
        _allTroopEffect = Resources.LoadAll<TroopEffect>("Scriptable Objects/Troop Effect").ToList();
        _allStatDecorator = Resources.LoadAll<StatDecorator>("Scriptable Objects/Stat Decorator").ToList();
    }

    public BlockCard CreateTroopCard(Polyomino polyomino, TroopEffect troopEffect, StatDecorator statDecorator) {
        return new BlockCard(polyomino, troopEffect, statDecorator);
    }

    public BlockCard CreateRandomTroopCard() {
        Polyomino polyomino = Polyomino.GetRandomPolyomino();
        TroopEffect troopEffect = _allTroopEffect[Random.Range(0, _allTroopEffect.Count)];
        StatDecorator statDecorator = _allStatDecorator[Random.Range(0, _allStatDecorator.Count)];

        return new BlockCard(polyomino, troopEffect, statDecorator);
    }
}
