using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff Token", menuName = "Enemy/Buff Token")]
public class BuffToken : ScriptableObject {
    [SerializeField]
    public float Attack, MaxHP;
}
