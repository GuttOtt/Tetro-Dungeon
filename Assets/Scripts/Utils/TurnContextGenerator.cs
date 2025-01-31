using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnContextGenerator: Singleton<TurnContextGenerator> {
    private Board board;

    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateBoard();
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        UpdateBoard();
    }

    private void UpdateBoard() {
        board = GameObject.FindObjectOfType<Board>();
    }

    public TurnContext GenerateTurnContext() {
        return new TurnContext(board);
    }
}
