using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySlider : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private EnemySystem _enemySystem;

    [SerializeField]
    private TMP_Text _text;

    private void Awake() {
        _slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    private void OnValueChanged() {
        int sliderValue = (int) _slider.value;

        _enemySystem.SetDifficulty(sliderValue);
        _text.text = "Difficulty: " + sliderValue.ToString(); 
    }
}
