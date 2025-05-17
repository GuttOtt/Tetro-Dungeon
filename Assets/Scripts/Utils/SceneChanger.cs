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
    public void LoadDeckEditScene()
    {
        SceneManager.LoadScene("DeckEdit");
    }

    public void LoadItemEditScene()
    {
        SceneManager.LoadScene("ItemEdit");
    }
    public void LoadCombinationScene()
    {
        SceneManager.LoadScene("Combination");
    }

    public void LoadStageScene() {
        SceneManager.LoadScene("Stage");
    }

    public void LoadReadyScene() {
        SceneManager.LoadScene("ReadyScene");
    }
}
