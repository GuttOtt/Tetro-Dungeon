using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopCard {
    #region private members
    private Polyomino _polyomino;
    private StatDecorator _statDecorator;
    private TroopEffect _troopEffect;
    #endregion

    #region Properties
    public Polyomino Polyomino { get => _polyomino; }
    public StatDecorator StatDecorator { get => _statDecorator; }
    public TroopEffect TroopEffect { get => _troopEffect; }
    #endregion

    public TroopCard(Polyomino polyomino, TroopEffect troopEffect, StatDecorator statDecorator) {
        _polyomino= polyomino;
        _statDecorator= statDecorator;
        _troopEffect= troopEffect;
    }
}
