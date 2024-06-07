using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnContext {
    public Board Board { get; set; }
    public CharacterTypes MoveTurn { get; set;}
    public BaseUnit CurrentActionUnit { get; set; }

    public TurnContext(Board board, CharacterTypes moveTurn, BaseUnit currentActionUnit) {
        Board = board;
        MoveTurn = moveTurn;
        CurrentActionUnit = currentActionUnit;
    }
}
