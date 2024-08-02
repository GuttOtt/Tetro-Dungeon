using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject {
    [SerializeField]
    public string Name;

    [SerializeField]
    public int MaxLife;

    [SerializeField]
    public List<BuffToken> BuffTokenPerDifficulty;

    /*
    [SerializeField]
    public EnemyEffect EnemyEffect;
    */
}
