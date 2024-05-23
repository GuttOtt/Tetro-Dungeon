using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    public int MaxHP { get; }
    public int MaxMP { get; }
    public int Attack { get; }
    public int Range { get; }

    
}
