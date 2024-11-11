using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class ReadySceneManager : MonoBehaviour {
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] CharacterBlockSystem _characterBlockSystem;
    private Player _player;

    private void Start() {
        _player = Player.Instance;

        foreach (CharacterBlockData data in _player.CharacterBlocksInventory) {
            CharacterBlock block = _characterBlockSystem.CreateCharacterBlock(data);
            _inventorySystem.Add(block);
        }
    }
}
