using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BattleSystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;
    private Board _board;
    #endregion

    private void Awake() {
        _gameManager= transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
    }
    
    public async UniTask ComputeTick(CharacterTypes attackTurn) {

    }
}
