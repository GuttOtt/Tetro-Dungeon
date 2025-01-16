using UnityEngine;

public class TurnContextGenerator: Singleton<TurnContextGenerator> {
  private Board board;
  private void Start() {
    board = GameObject.FindObjectOfType<Board>();
  }

  public TurnContext GenerateTurnContext() {
    return new TurnContext(board);
  }
}
