using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

[CreateAssetMenu]
public class Item : ScriptableObject {
    [SerializeField]
    private TArray<bool> shape = new TArray<bool>(5, 5);

}
