using Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("SampleScene");
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
}
