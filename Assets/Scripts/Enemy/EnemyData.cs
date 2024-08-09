using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
    [SerializeField]
    public string Name;

    [SerializeField]
    public int MaxLife;

    [SerializeField]
    public List<BuffToken> BuffTokenPerDifficulty;

    
    [SerializeField]
    public EnemyEffect EnemyEffect;
    
}
