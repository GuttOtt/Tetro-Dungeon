using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class GameMenuManager : Singleton<GameMenuManager>
{
    [SerializeField] private GameObject gameMenuUI;

    private void Start()
    {
        gameMenuUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleGameMenu();
        }
    }

    public void ToggleGameMenu()
    {
        gameMenuUI.SetActive(!gameMenuUI.activeSelf);
        Time.timeScale = gameMenuUI.activeSelf ? 0 : 1; // Pause the game when menu is open
    }

    public void RestartGame()
    {
        SceneChanger.Instance.LoadMainMenu();
        Destroy(Player.Instance.gameObject);
        ResumeGame();
    }

    public void ResumeGame()
    {
        gameMenuUI.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }
}
