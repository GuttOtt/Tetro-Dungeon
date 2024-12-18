using Assets.Scripts;
using Assets.Scripts.Reward;
using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

//전반적인 게임 플로우 제어 및
//System 들의 컴포지트 루트 담당
public class GameManager : MonoBehaviour, IGameManager
{
    #region private members
    #region Systems
    [SerializeField]
    private CardSystem _cardSystem;

    [SerializeField]
    private UnitSystem _unitSystem;

    private Player _player;

    [SerializeField]
    private Board _board;

    [SerializeField]
    private PhaseSystem _phaseSystem;

    [SerializeField]
    private BattleSystem _battleSystem;

    [SerializeField]
    private EnemySystem _enemySystem;

    [SerializeField]
    private UnitBlockSystem _unitBlockSystem;

    [SerializeField]
    private SynergySystem _synergySystem;

    [SerializeField]
    private TroopCardSystem _troopCardSystem;
    
    [SerializeField]
    private RewardSystem _rewardSystem;

    [SerializeField]
    private CharacterBlockSystem _characterBlockSystem;

    [SerializeField]
    private SceneChanger _sceneChanger;
    #endregion

    [SerializeField]
    private bool isWin = false; // Inspector에 노출될 변수

    #endregion


    private void Start() {
        _player = Player.Instance;
        if (_player == null)
        {
            Debug.LogError("Player instance is null!");
            return;
        }

        Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");

        if (SceneManager.GetActiveScene().name == "BattleScene")
            StartBattleScene();
    }

    private async void StartBattleScene() {
        PlaceEnemyUnits();
        PlacePlayerUnits().Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(3));

        _battleSystem.StartBattle().Forget();
    }

    private async UniTaskVoid PlacePlayerUnits() {
        List<CharacterBlock> blocks = new List<CharacterBlock>();

        foreach (CharacterBlockData data in _player.CharacterBlocksOnBoard) {
            CharacterBlock block = _characterBlockSystem.CreateCharacterBlock(data, true);
            blocks.Add(block);
        }

        await UniTask.WaitForSeconds(2f);

        foreach (CharacterBlock block in blocks) {
            BaseUnit unit = _unitSystem.CreateUnit(block.Config, CharacterTypes.Player);

            //Equipments
            List<Equipment> equipments = block.Equipments;
            foreach (Equipment equipment in equipments) {
                unit.Equip(equipment.Config);
            }

            //Place unit in the center of the characterBlock
            Vector2Int centerCellPos = block.CenterCellPos;
            Cell centerCell = _board.GetCell(centerCellPos.x, centerCellPos.y);

            _board.Place(centerCell, unit);

            Destroy(block.gameObject);
        }
    }

    private void PlaceEnemyUnits() {
        _enemySystem.DecideUnitList();
    }

    public void RestartBattleScene() {
        _phaseSystem.ToStandbyPhase();
    }

    public void InitializeUIElements()
    {
        _phaseSystem.InitializeUIElements();
    }

    public T GetSystem<T>() where T : class {
        if (typeof(T) == typeof(CardSystem)) {
            return _cardSystem as T;
        }
        else if (typeof(T) == typeof(UnitSystem)) {
            return _unitSystem as T;
        }
        else if (typeof(T) == typeof(Board)) {
            return _board as T;
        }
        else if (typeof(T) == typeof(PhaseSystem)) {
            return _phaseSystem as T;
        }
        else if(typeof(T) == typeof(BattleSystem)) {
            return _battleSystem as T;
        }
        else if (typeof(T) == typeof(EnemySystem)) {
            return _enemySystem as T;
        }
        else if (typeof(T) == typeof(UnitBlockSystem)) {
            return _unitBlockSystem as T;
        }
        else if (typeof(T) == typeof(SynergySystem)) {
            return _synergySystem as T;
        }
        else if (typeof(T) == typeof(TroopCardSystem)) {
            return _troopCardSystem as T;
        }
        else {
            Debug.LogError($"System of type {typeof(T)} is not supported.");
            return null;
        }
    }

    public async void PlayerWin() {
        Debug.Log("플레이어 승리! 3초 후 다음 스테이지로 넘어갑니다.");
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        _sceneChanger.LoadReadyScene();
    }

    public void PlayerLose() {
        Debug.Log("플레이어 패배... ");
    }

    public TurnContext CreateTurnContext() {
        Board board = _board;
        CharacterTypes moveTurn = _battleSystem.IsProcessing ? _battleSystem.AttackTurn : CharacterTypes.None;
        CardSystem cardSystem = _cardSystem;

        return new TurnContext(board, moveTurn, cardSystem);
    }
}
