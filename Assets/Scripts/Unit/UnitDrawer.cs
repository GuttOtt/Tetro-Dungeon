using TMPro;
using UnityEngine;

public class UnitDrawer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _unitSprite;

    [SerializeField]
    private TMP_Text _hpText, _mpText, _attackText, _rangeText;

    public void Draw(UnitConfig unitConfig) {
        //스프라이트
        if (_unitSprite != null) {
            _unitSprite.sprite = unitConfig.Sprite;
        }

        //텍스트
        _hpText?.SetText(unitConfig.MaxHP.ToString());
        _mpText?.SetText(unitConfig.MaxMP.ToString());
        _attackText?.SetText(unitConfig.Attack.ToString());
        _rangeText?.SetText(unitConfig.Range.ToString());
    }

    public void UpdateHP(int hp, Color color) {
        _hpText?.SetText(hp.ToString());
        _hpText.color = color;
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
