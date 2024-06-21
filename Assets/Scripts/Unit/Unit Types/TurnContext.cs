using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnContext {
    public Board Board { get; set; }
    public CharacterTypes MoveTurn { get; set;}
    public CardSystem CardSystem { get; set; }

    public TurnContext(Board board, CharacterTypes moveTurn, CardSystem cardSystem) {
        Board = board;
        MoveTurn = moveTurn;
        CardSystem = cardSystem;
    }
}
