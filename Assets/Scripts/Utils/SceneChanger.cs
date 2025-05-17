using Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void LoadStageScene() {
        SceneManager.LoadScene("Stage");
    }

    public void LoadReadyScene() {
        SceneManager.LoadScene("ReadyScene");
    }

    public void LoadEndingScene() {
        SceneManager.LoadScene("EndingScene");
    }

    public void LoadGameOverScene() {
        SceneManager.LoadScene("GameOverScene");
    }

    public void LoadUpscaylScene() {
        SceneManager.LoadScene("UpscaylScene");
    }
}
