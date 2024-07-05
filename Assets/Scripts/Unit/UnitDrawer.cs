using Assets.Scripts.Unit.UI;
using TMPro;
using UnityEngine;

public class UnitDrawer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _unitSprite;

    [SerializeField]
    private TMP_Text _hpText, _mpText, _attackText, _rangeText, _synergyText;

    [SerializeField]
    public GameObject _tooltip;

    [SerializeField]
    private UnitHealthText _healthText;

    public UnitHealthBar _healthBar;

    private TextMeshProUGUI _tooltip_name;
    private TextMeshProUGUI _tooltip_effect;

    public void Awake()
    {
        // 체력 바
        _healthBar = GetComponentInChildren<UnitHealthBar>();
        // Tooltip 내의 Name TextMeshPro 컴포넌트 찾기
        if (_tooltip != null)
        {
            _tooltip_name = _tooltip.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            _tooltip_effect = _tooltip.transform.Find("Effect")?.GetComponent<TextMeshProUGUI>();
        }
    }

    public void Draw(UnitConfig unitConfig, int attackBuff = 0, int hpBuff = 0) {
        //스프라이트
        if (_unitSprite != null) {
            _unitSprite.sprite = unitConfig.Sprite;
        }

        int attack = unitConfig.Attack + attackBuff;
        int hp = unitConfig.MaxHP + hpBuff;

        //텍스트
        _hpText?.SetText(hp.ToString());
        _mpText?.SetText(attack.ToString());
        _attackText?.SetText(unitConfig.Attack.ToString());
        _rangeText?.SetText(unitConfig.Range.ToString());
        _synergyText?.SetText(unitConfig.Synergies[0].ToString());

        _tooltip_name?.SetText(unitConfig.Name);
        _tooltip_effect?.SetText(unitConfig.EffectDescription);

        Debug.Log(unitConfig.EffectDescription);

        if (0 < attackBuff) {
            _attackText.color = new Color(0, 0.7f, 0);
        }
        if (0 < hpBuff) {
            _hpText.color = new Color(0, 0.7f, 0);
        }
    }

    public void UpdateHP(int hp, Color color) {
        _hpText?.SetText(hp.ToString());
        _healthBar.SetHealth(hp);
        _hpText.color = color;
    }


    public void UpdateAttack(int attack, Color color) {
        _attackText?.SetText(attack.ToString());
        _attackText.color = color;
    }

    public void DisplayDamageText(int number) {
        _healthText?.DisplayText(-number, Color.red);
    }

    public void DisplayHealText(int number) {
        _healthText.DisplayText(number, Color.white);
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
}
