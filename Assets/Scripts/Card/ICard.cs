using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard {
    public Polyomino Polyomino { get; }
    public UnitConfig UnitConfig { get; }
}
