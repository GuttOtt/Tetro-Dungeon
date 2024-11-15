using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    private List<CharacterBlockConfig> _characterBlockPool = new List<CharacterBlockConfig>();
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();


    [SerializeField] private CharacterBlockSystem _characterBlockSystem;

    private void Awake() {
        _characterBlockPool = Resources.LoadAll<CharacterBlockConfig>("Scriptable Objects/Character Block").ToList();
    }

    public void StartSelling() {
        for (int i = 0; i < 3; i++) {
            CharacterBlock newCharacter = AddCharacterBlock();
            newCharacter.transform.parent = transform;
            newCharacter.transform.localPosition = new Vector3(i * 2, 0, 0);
        }
    }

    private CharacterBlock AddCharacterBlock() {
        CharacterBlockConfig config = _characterBlockPool[Random.Range(0, _characterBlockPool.Count)];
        CharacterBlock characterBlock = _characterBlockSystem.CreateCharacterBlock(config, 1);
        _characterBlocks.Add(characterBlock);

        return characterBlock;
    }
}
