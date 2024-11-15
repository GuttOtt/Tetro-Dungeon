using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
    [SerializeField]
    public string Name;

    [SerializeField]
    public Sprite Sprite;

    [SerializeField]
    public string Description;

    [SerializeField]
    public int MaxLife;

    [SerializeField]
    public List<BuffToken> BuffTokenPerDifficulty;
    
    [SerializeField]
    public EnemyEffect EnemyEffect;

    [SerializeField]
    public List<UnitConfig> UnitConfigs = new List<UnitConfig>();

    public List<CharacterBlockConfig> CharacterBlockConfigs = new List<CharacterBlockConfig>();
}
