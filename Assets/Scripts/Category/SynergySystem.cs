using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using EnumTypes;
using TMPro;

public class SynergySystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;
    private Board _board;

    private Dictionary<SynergyTypes, int> _synergyDic = new Dictionary<SynergyTypes, int>();

    [SerializeField]
    private TMP_Text _synergyText;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();

        _board.onPlaceUnit += UpdateSynergy;
    }

    public void UpdateSynergy() {
        Dictionary<SynergyTypes, int> synergyDic = new Dictionary<SynergyTypes, int>();

        List<UnitBlock> unitBlocks = new List<UnitBlock>();

        for (int i = 0; i < _board.Column / 2; i++) {
            List<UnitBlock> blocksInRow = new List<UnitBlock>();
            
            if (_board.IsRowFull(i)) {
                blocksInRow = _board.GetBlocksInRow(i);
            }

            foreach (UnitBlock block in blocksInRow) {
                if (!unitBlocks.Contains(block)) {
                    unitBlocks.Add(block);
                }
            }
        }

        foreach (UnitBlock block in unitBlocks) {
            foreach (SynergyTypes synergy in block.Synergies) {
                if (synergyDic.ContainsKey(synergy)) {
                    synergyDic[synergy]++;
                }
                else {
                    synergyDic[synergy] = 1;
                }
            }

            block.Highlight();
        }

        _synergyDic = synergyDic;

        DisplaySynergies();
    }

    private void DisplaySynergies() {
        _synergyText.text = "Synergies: ";
        _synergyText.text += System.Environment.NewLine;

        foreach (SynergyTypes synergyTypes in _synergyDic.Keys) {
            _synergyText.text += $"{synergyTypes} : {_synergyDic[synergyTypes]}";
            _synergyText.text += System.Environment.NewLine;
        }
    }
}
