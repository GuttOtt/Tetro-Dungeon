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


    private StageManager _stageManager;

    private void Start() {
        _stageManager = StageManager.Instance;
        InitNodes();
    }

    private void InitNodes() {
        List<StageData> stages = _stageManager.Stages;

        for (int i = 0; i < stages.Count; i++) {
            _nodes[i].Init(stages[i]);

            _nodes[i].onPointerEnter += () => DrawStageInfoUI(stages[i]);
            _nodes[i].onPointerExit += () => CloseStageInfoUI();
        }
    }

    private void DrawStageInfoUI(StageData stageData) {
        _stageInfoUIPanel.gameObject.SetActive(true);
        Debug.Log("EnemyStageData");

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

    public void ToBattleScene() {
        _sceneChanger.LoadBattleScene();
    }

}