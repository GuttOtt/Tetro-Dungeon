using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        // Load the new game scene
        SceneChanger.Instance.LoadReadyScene();
    }

    private void OnLoadGameButtonClicked()
    {
        // To do
    }

    private void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();
    }
}
