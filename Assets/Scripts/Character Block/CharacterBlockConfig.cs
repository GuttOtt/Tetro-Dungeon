using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlockConfig : ScriptableObject {
    public string Name;
    public UnitConfig UnitConfig;
    public List<TArray<bool>> Shapes;
}
