using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnContext {
    public Board Board { get; set; }
    public CharacterTypes MoveTurn { get; set;}

    public TurnContext(Board board, CharacterTypes moveTurn) {
        Board = board;
        MoveTurn = moveTurn;
    }
}
