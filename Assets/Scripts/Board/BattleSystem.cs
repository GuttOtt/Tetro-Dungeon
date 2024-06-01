using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private Board _board;
    
    //배틀 진행 시의 딜레이
    [SerializeField]
    private float delayPerUnit = 0.5f;
    [SerializeField]
    private float delayPerTick = 1f;

    //배틀이 진행중인가?
    private bool _isProcessing = false;

    //라이프 (이후 다른 클래스로 옮겨야 할 수도 있음)
    private Dictionary<CharacterTypes, int> _lifeDic;
    [SerializeField] private TMP_Text _playerLifeText;
    [SerializeField] private TMP_Text _enemyLifeText;
    private Dictionary<CharacterTypes, TMP_Text> _lifeTextDic = new Dictionary<CharacterTypes, TMP_Text>();
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();

        //Set Life
        _lifeDic = new Dictionary<CharacterTypes, int>() { 
            { CharacterTypes.Player, 10 },
            { CharacterTypes.Enemy, 10 }
        };

        _lifeTextDic.Add(CharacterTypes.Player, _playerLifeText);
        _lifeTextDic.Add(CharacterTypes.Enemy, _enemyLifeText);
        UpdateLifeText(CharacterTypes.Player);
        UpdateLifeText(CharacterTypes.Enemy);
    }

    public async void StartBattle(CharacterTypes attackTurn) {
        if (_isProcessing)
            return;

        Debug.Log("Battle Started");
        _isProcessing = true;

        //Attack Turn의 유닛이 전부 사라질 때까지 전투
        while (true) {
            if (_board.GetUnits(attackTurn).Count == 0) break;

            await ComputeTick(attackTurn);
        }

        //배틀 종료
        _isProcessing= false;
        _gameManager.GetSystem<PhaseSystem>().ToEndPhase();
    }

    public async UniTask ComputeTick(CharacterTypes attackTurn) {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        //플레이어 유닛 액션
        foreach (var unit in playerUnits) {
            //하이라이트하고 딜레이
            (unit as BaseUnit).Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //이동할 수 있다면, 이동시키고 다음 유닛으로
            if (CheckMovable(unit, attackTurn)) {
                MoveUnit(unit);
            }
            //이동할 수 없다면, 공격을 시도
            else {
                UnitAttack(unit);
            }

            //하이라이트 끄기
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }

        //적 유닛 액션
        foreach (var unit in enemyUnits) {
            //하이라이트하고 딜레이
            (unit as BaseUnit).Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //이동할 수 있다면, 이동시키고 다음 유닛으로
            if (CheckMovable(unit, attackTurn)) {
                MoveUnit(unit);
            }
            //이동할 수 없다면, 공격을 시도
            else {
                UnitAttack(unit);
            }

            //하이라이트 끄기
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }


        //유닛의 죽음 처리
        ProcessDeath(playerUnits);
        ProcessDeath(enemyUnits);

        await UniTask.Delay(TimeSpan.FromSeconds(delayPerTick));
    }

    private bool CheckMovable(IUnit unit, CharacterTypes attackTurn) {
        if (unit.Owner != attackTurn) {
            return false;
        }
        Cell currentCell = unit.CurrentCell;

        // 전방으로 한 칸의 위치 계산
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //forwardCell에 이미 유닛이 있거나, forwardCell이 존재하지 않는다면 false
        if (forwardCell == null || forwardCell.Unit != null)
            return false;

        return true;
    }

    private void MoveUnit(IUnit unit) {
        Cell currentCell = unit.CurrentCell;

        // 전방으로 한 칸의 위치 계산
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //유닛 이동
        currentCell.UnitOut();
        forwardCell.UnitIn(unit);
        unit.CurrentCell = forwardCell;

        //끝 열에 도달했다면 유닛을 죽이고 라이프 데미지
        int endCol = unit.Owner == CharacterTypes.Player ? _board.Column - 1 : 0;
        if (forwardCell.position.col == endCol) {
            LifeDamage(unit.Owner.Opponent(), unit.Attack);
            unit.Die();
        }
    }

    private void UnitAttack(IUnit unit) {
        int range = unit.Range;
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = unit.CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //가까운 유닛을 우선으로 공격
        for (int i = 1; i <= range; i++) {
            Cell targetCell = _board.GetCell(originCol + forwardOffset * i, originRow);
            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && unit.Owner != targetUnit.Owner) {
                //사정거리 내에 유닛이 있으면 공격하고 루프 종료
                targetUnit.TakeDamage(unit.Attack);
                break;
            }
        }
    }

    private void ProcessDeath(List<IUnit> units) {
        foreach (IUnit unit in units) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit.CurrentHP <= 0) {
                baseUnit.Die();
            }
        }
    }

    #region Life Damage
    private void LifeDamage(CharacterTypes characterType, int damage) {
        _lifeDic[characterType] -= damage;
        UpdateLifeText(characterType);
    }

    private void UpdateLifeText(CharacterTypes charactertype) {
        _lifeTextDic[charactertype].text = "Life: " + _lifeDic[charactertype].ToString();
    }
    #endregion
}
