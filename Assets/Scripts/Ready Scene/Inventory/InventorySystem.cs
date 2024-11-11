using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour {
    [SerializeField] private BoxCollider2D _area;
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();

    public void Add(CharacterBlock characterBlock) {
        _characterBlocks.Add(characterBlock);
        characterBlock.transform.position = _area.transform.position;
    }

    public void Remove(CharacterBlock characterBlock) {
        _characterBlocks.Remove(characterBlock);
    }

    public bool IsInsideArea(CharacterBlock characterBlock) {
        Vector3 blockPos = characterBlock.transform.position;
        Bounds areaBounds = _area.bounds;

        return areaBounds.Contains(blockPos);
    }

    public bool ContainsItem(CharacterBlock characterBlock) {
        return _characterBlocks.Contains(characterBlock);
    }
}
