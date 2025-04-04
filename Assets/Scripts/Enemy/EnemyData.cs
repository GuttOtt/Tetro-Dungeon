using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
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
    public EnemyEffect EnemyEffect;

    public List<CharacterBlockConfig> CharacterBlockConfigs = new List<CharacterBlockConfig>();
    public List<UnitPlacementConfig> UnitPlacementConfigs = new List<UnitPlacementConfig>();

}

[Serializable]
public class UnitPlacementConfig {
    [SerializeField] public CharacterBlockConfig chracterBlockConfig;
    [SerializeField] public Array2DBool placableArea;
    [SerializeField] public int placeAmount;
    [SerializeField] public float placeAmountPerDifficerty;
}
