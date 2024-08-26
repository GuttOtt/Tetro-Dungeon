using Assets.Scripts.Unit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitConfigUIDrawer : MonoBehaviour
{
    [SerializeField]
    private Image _unitSprite;

    [SerializeField]
    private TMP_Text _hpText, _attackText, _synergyText;

    [SerializeField]
    public GameObject _tooltip;

    private UnitConfig _unitConfig;

    private TextMeshProUGUI _tooltip_name;
    private TextMeshProUGUI _tooltip_effect;

    public UnitConfig UnitConfig { get => _unitConfig; } 

    public void Draw(UnitConfig unitConfig, int attackBuff = 0, int hpBuff = 0) {
        //스프라이트
        if (_unitSprite != null) {
            _unitSprite.sprite = unitConfig.Sprite;
        }

        _unitConfig = unitConfig;
        int attack = unitConfig.Attack + attackBuff;
        int hp = unitConfig.MaxHP + hpBuff;

        //텍스트
        _hpText?.SetText(hp.ToString());
        _attackText?.SetText(unitConfig.Attack.ToString());
        _synergyText?.SetText(unitConfig.Synergies[0].ToString());

        _tooltip_name?.SetText(unitConfig.Name);
        _tooltip_effect?.SetText(unitConfig.EffectDescription);

        if (0 < attackBuff) {
            _attackText.color = new Color(0, 0.7f, 0);
        }
        if (0 < hpBuff) {
            _hpText.color = new Color(0, 0.7f, 0);
        }
    }

    public void UpdateHP(int hp, Color color) {
        _hpText?.SetText(hp.ToString());
        _hpText.color = color;
    }


    public void UpdateAttack(int attack, Color color) {
        _attackText?.SetText(attack.ToString());
        _attackText.color = color;
    }

    public void Highlight() {
        if (_unitSprite != null)
        {
            _unitSprite.color = Color.yellow;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is missing or destroyed.");
        }
    }

    public void Unhighlight() {
        if (_unitSprite != null)
        {
            _unitSprite.color = Color.white;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is missing or destroyed.");
        }
    }

    public void ChangeColor(Color color) {
        if (_unitSprite != null)
        {
            _unitSprite.color = color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is missing or destroyed.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hello2");
    }
}
