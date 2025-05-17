using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneController : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        mainMenuButton?.onClick.AddListener(OnMainMenuButtonClicked);
        quitButton?.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnMainMenuButtonClicked()
    {
        // Load the new game scene
        GameMenuManager.Instance.RestartGame();
    }


    private void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();
    }
}
