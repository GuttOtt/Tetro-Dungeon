using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class ReadySceneManager : MonoBehaviour {
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] CharacterBlockSystem _characterBlockSystem;
    [SerializeField] EquipmentSystem _equipmentSystem;
    [SerializeField] SceneChanger _sceneChanger;
    private Player _player;
    private Board _board;

    private void Start() {
        if (_player == null) 
            _player = Player.Instance;

        foreach (CharacterBlockData data in _player.CharacterBlocksInventory) {
            CharacterBlock block = _characterBlockSystem.CreateCharacterBlock(data);
            _inventorySystem.Add(block);
        }

        foreach (CharacterBlockData data in _player.CharacterBlocksOnBoard) {
            CharacterBlock block = _characterBlockSystem.CreateCharacterBlock(data, true);
        }

        foreach (EquipmentData data in _player.EquipmentsInventory) {
            Equipment equipment = _equipmentSystem.CreateEquipment(data);
            _inventorySystem.Add(equipment);
        }
    }

    public void ReloadReadyScene() {
        SaveBoardData();
        SaveInventoryData();
        _sceneChanger.LoadReadyScene();
    }

    public void ToBattleScene() {
        SaveBoardData();
        SaveInventoryData();
        _sceneChanger.LoadBattleScene();
    }

    private void SaveBoardData() {
        List<CharacterBlockData> characterBlockDatas = 
            _characterBlockSystem.GetCharacterBlockDatasOnBoard();

        _player.SaveCharacterBlockDatasOnBoard(characterBlockDatas);
    }

    private void SaveInventoryData() {
        //Save Character Blocks
        List<CharacterBlockData> characterBlockDatas =
            _inventorySystem.GetCharacterBlockDatas();

        _player.SaveCharacterBlockDataOnInventroy(characterBlockDatas);

        //Save Equipments
        List<EquipmentData> equipmentDatas =
            _inventorySystem.GetEquipmentDatas();

        _player.SaveEquipmentsInventory(equipmentDatas);
    }
}