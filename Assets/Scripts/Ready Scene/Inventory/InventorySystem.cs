using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        blockPos.z = 0;
        Bounds areaBounds = _area.bounds;

        Debug.Log(areaBounds.size);
        Debug.Log(areaBounds.Contains(blockPos));

        return areaBounds.Contains(blockPos);
    }

    public bool ContainsItem(CharacterBlock characterBlock) {
        return _characterBlocks.Contains(characterBlock);
    }

    public List<CharacterBlockData> GetCharacterBlockDatas() {
        List<CharacterBlockData> datas = new List<CharacterBlockData>();

        foreach (CharacterBlock characterBlock in _characterBlocks) {
            CharacterBlockData data = characterBlock.GetData();
            datas.Add(data);
        }

        return datas.ToList();
    }
}
