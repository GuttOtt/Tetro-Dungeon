using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageNodeSystem : MonoBehaviour {
    [SerializeField]
    private List<StageNode> _nodes = new List<StageNode>();

    [SerializeField]
    private TMP_Text _nodeNameText, _nodeDescriptionText;

    [SerializeField]
    private Image _nodeImage;

    [SerializeField]
    private SceneChanger _sceneChanger;

    [SerializeField]
    private GameObject _stageInfoUIPanel;

    [SerializeField]
    private Button _moveButton;

    [SerializeField]
    private GameObject _playerMarker;


    private StageManager _stageManager;

    private void Start() {
        _stageManager = StageManager.Instance;
        InitNodes();
        MovePlayerMarker(false);
    }

    private void InitNodes() {
        List<StageData> stages = _stageManager.Stages;

        for (int i = 0; i < stages.Count; i++) {
            StageData data = stages[i];
            _nodes[i].Init(data);

            _nodes[i].onPointerClick += () => DrawStageInfoUI(data);
        }

        int currentStageIndex = _stageManager.CurrentStageIndex;
        _nodes[currentStageIndex + 1].GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    private void DrawStageInfoUI(StageData stageData) {
        _stageInfoUIPanel.gameObject.SetActive(true);

        if (stageData == null) {
            Debug.LogError("stageData가 없습니다.");
            return;
        }

        if (stageData as EnemyStageData != null) {
            EnemyStageData enemyStageData = (EnemyStageData)stageData;
            EnemyData enemyData = enemyStageData.enemyData;

            _nodeNameText.text = enemyData.Name;
            _nodeDescriptionText.text = enemyData.Description;
            _nodeImage.sprite = enemyData.Sprite;
        }

        if (_stageManager.CurrentStageIndex == stageData.stageIndex - 1) {
            _moveButton.gameObject.SetActive(true);
        }
        else {
            _moveButton.gameObject.SetActive(false);
        }
    }

    public void CloseStageInfoUI() {
        _stageInfoUIPanel?.gameObject.SetActive(false);
    }

    public async void ToNextStage() {
        _stageManager.MoveForward();

        StageData nextStage = _stageManager.CurrentStage;

        CloseStageInfoUI();

        MovePlayerMarker(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        if (nextStage is EnemyStageData) {
            _sceneChanger.LoadBattleScene();
        }
    }

    private void MovePlayerMarker(bool isDoTween) {
        StageNode currentStageNode = _nodes[_stageManager.CurrentStageIndex];

        Transform markerTransform = _playerMarker.transform;
        Vector3 moveVector = new Vector3(currentStageNode.transform.position.x, markerTransform.position.y, -1);

        if (isDoTween) {
            markerTransform.DOMove(moveVector, 1f).SetEase(Ease.OutBack, 2f);
        }
        else {
            markerTransform.position = moveVector;
        }
    }
}