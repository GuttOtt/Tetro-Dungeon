using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterBlockData  {
    public CharacterBlockConfig Config;
    public int Level;

    public CharacterBlockData(CharacterBlockConfig config, int level) {
        Config = config;
        Level = level;

    }
}
