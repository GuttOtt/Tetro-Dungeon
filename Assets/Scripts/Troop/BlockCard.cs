using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockCard : MonoBehaviour
{
    #region private members
    private Polyomino _polyomino;
    private StatDecorator _statDecorator;
    private TroopEffect _troopEffect;
    private GameObject _tooltipPrefab;
    [SerializeField] private TextMeshProUGUI _tooltip_name;
    [SerializeField] private TextMeshProUGUI _tooltip_effect;
    #endregion

    #region Properties
    public Polyomino Polyomino { get => _polyomino; }
    public StatDecorator StatDecorator { get => _statDecorator; }
    public TroopEffect TroopEffect { get => _troopEffect; }
    public GameObject TooltipPrefab
    {
        get => _tooltipPrefab; 
        set
        {
            _tooltipPrefab = value;

            if (_tooltipPrefab != null)
            {
                // Panel 객체를 Canvas의 자식으로부터 바로 찾습니다.
                Transform panelTransform = _tooltipPrefab.transform.GetChild(0);

                // Panel 하위의 TextMeshProUGUI 컴포넌트들을 배열로 가져옵니다.
                TextMeshProUGUI[] textMeshPros = panelTransform.GetComponentsInChildren<TextMeshProUGUI>();

                if (textMeshPros.Length >= 2)
                {
                    // 두 개의 TextMeshProUGUI 컴포넌트를 사용합니다.
                    //Debug.Log("TextMeshPro 1 Text: " + textMeshPros[0].text);
                    //Debug.Log("TextMeshPro 2 Text: " + textMeshPros[1].text);
                    textMeshPros[1]?.SetText(_troopEffect.Description);
                }
                else
                {
                    Debug.LogError("TextMeshPro components not found or insufficient number!");
                }

                //_tooltip_name = _tooltipPrefab.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                //_tooltip_effect = _tooltipPrefab.transform.Find("Effect")?.GetComponent<TextMeshProUGUI>();
            }
        }
    }
    #endregion

    public BlockCard(Polyomino polyomino, TroopEffect troopEffect, StatDecorator statDecorator)
    {
        _polyomino = polyomino;
        _statDecorator = statDecorator;
        _troopEffect = troopEffect;
    }

    public void Init(BlockCard troop)
    {
        _polyomino = troop.Polyomino;
        _statDecorator = troop.StatDecorator;
        _troopEffect = troop.TroopEffect;
    }
}
