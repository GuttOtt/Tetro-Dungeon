using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awakening {
    protected AwakeningCondition _condition;

    public void UpdateActivation(BaseUnit baseUnit) {

    }
}

[Serializable]
public class AwakeningCondition {
    public bool IsSatisfied(BaseUnit unit) {
        return false;
    }
}