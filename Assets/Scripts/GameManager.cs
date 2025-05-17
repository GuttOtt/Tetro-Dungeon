using Assets.Scripts;
using Assets.Scripts.Reward;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
    private CancellationTokenSource _cancellationTokenSource;

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

    [SerializeField] private EquipmentSystem _equipmentSystem;

    [SerializeField]
    private SceneChanger _sceneChanger;
    #endregion

    [SerializeField] BaseUnitMarker _baseUnitMarkerPrefab;
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
            StartBattleScene().Forget();
    }

    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private async UniTask StartBattleScene() {
        _characterBlockSystem.SetInputOff();
        _equipmentSystem.SetInputOff();

        PlaceEnemyUnits();
        await PlacePlayerUnits(_cancellationTokenSource.Token);

        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: _cancellationTokenSource.Token);

        _battleSystem.StartBattle().Forget();
    }

    private async UniTask PlacePlayerUnits(CancellationToken cancellationToken) {
        List<CharacterBlock> blocks = new List<CharacterBlock>();

        foreach (CharacterBlockData data in _player.CharacterBlocksOnBoard) {
            CharacterBlock block = _characterBlockSystem.CreateCharacterBlock(data, true);
            blocks.Add(block);
        }

        await UniTask.WaitForSeconds(1f, cancellationToken: cancellationToken);

        foreach (CharacterBlock block in blocks) {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (BlockPart blockPart in block.BlockParts) {
                blockPart.Cell = blockPart.PickCell();
                Debug.Log($"BlockPart {blockPart.name} is placed at {blockPart.Cell}");
            }
        }

        // BaseUnitMarker를 생성하고 드래그 앤 드랍으로 유닛의 위치를 조절
        List<BaseUnitMarker> markers = await ManipulatePlayerUnitPos(blocks, cancellationToken);

        foreach (BaseUnitMarker marker in markers) {
            cancellationToken.ThrowIfCancellationRequested();
            CharacterBlock block = marker.characterBlock;
            
            BaseUnit unit = _unitSystem.CreateUnit(block, CharacterTypes.Player);

            //Place unit in the manipulated marker's cell
            Cell cellToPlace = marker.cell;
            _board.Place(cellToPlace, unit);

            Destroy(block.gameObject);
            Destroy(marker.gameObject);
        }

        Debug.Log($"현재 보드의 모든 플레이어 유닛: {_board.GetUnits(CharacterTypes.Player).Count}");
    }

    private async UniTask<List<BaseUnitMarker>> ManipulatePlayerUnitPos(List<CharacterBlock> characterBlocks, CancellationToken cancellationToken) {
        // BaseUnitMarker들을 생성
        List<BaseUnitMarker> markers = new List<BaseUnitMarker>();
        foreach (CharacterBlock block in characterBlocks) {
            cancellationToken.ThrowIfCancellationRequested();
            Vector2Int centerCellPos = block.CenterCellPos;
            Cell centerCell = _board.GetCell(centerCellPos.x, centerCellPos.y);
            BaseUnitMarker marker = Instantiate(_baseUnitMarkerPrefab, centerCell.transform);
            marker.Initialize(centerCell, block);
            markers.Add(marker);
        }

        // 엔터키 입력을 기다림
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return), cancellationToken: cancellationToken);

        return markers;
    }

    private void PlaceEnemyUnits() {
        //_enemySystem.DecideUnitList();
        _enemySystem.CreateEnemyUnits();
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
        Debug.Log("플레이어 승리! 10Gold 획득.");
        _player.CurrentMoney += 10;

        Debug.Log("3초 후 다음 화면으로 넘어갑니다.");

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        if (StageManager.Instance.IsLastStage) {
            Debug.Log("모든 스테이지를 클리어했습니다.");
            _sceneChanger.LoadEndingScene();
        }
        else {
            StageManager.Instance.MoveForward();
            _sceneChanger.LoadReadyScene();
        }
    }

    public void PlayerLose() {
        Debug.Log("플레이어가 패배했습니다..");
    }

    public TurnContext CreateTurnContext() {
        Board board = _board;
        CharacterTypes moveTurn = _battleSystem.IsProcessing ? _battleSystem.AttackTurn : CharacterTypes.None;
        CardSystem cardSystem = _cardSystem;

        return new TurnContext(board, moveTurn, cardSystem);
    }
}
