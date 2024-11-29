using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBlockInfoSystem : MonoBehaviour {
    [SerializeField] private TMP_Text _nameText, _levelText, _hpText, _attackText, _spellPowerText,
        _defenceText, _spellDefenceText, _speedText, _rangeText;
    
    [SerializeField] private GameObject _panel;
    
    [SerializeField] private SpriteRenderer _illustImage;

    [SerializeField] private GameObject _spumRoot;
    [SerializeField] private SPUM_Prefabs _spum;
    [SerializeField] private SimpleMonoButton _closeButton;

    private void Start() {
        _panel.SetActive(false);
        _closeButton.onClick += ClosePanel;
    }

    private void Update() {
        CharacterBlock charaterBlock = GetRightClockedBlock();

        if (charaterBlock != null) 
            DrawInfo(charaterBlock);
    }

    private CharacterBlock GetRightClockedBlock() {
        if (!Input.GetMouseButtonDown(1))
            return null;

        CharacterBlock characterBlock = Utils.PickCharacterBlock();

        if (characterBlock == null ||
            (Utils.Pick<BlockPart_Equipment>() && !Input.GetKey(KeyCode.LeftShift)))
            return null;

        return characterBlock;
    }

    public void DrawInfo(CharacterBlock characterBlock) {
        _panel.SetActive(true);

        //Name
        _nameText.text = characterBlock.Config.Name;
        
        //Stats
        //_hpText.text = characterBlock.MaxHP.ToString();
        _attackText.text = characterBlock.Attack.ToString();
        _spellPowerText.text = characterBlock.SpellPower.ToString();
        _defenceText.text = characterBlock.Defence.ToString();
        _spellDefenceText.text = characterBlock.SpellDefence.ToString();
        _speedText.text = characterBlock.Speed.ToString();
        _rangeText.text = characterBlock.Range.ToString();

        //Illust
        _illustImage.sprite = characterBlock.Config.Illust;

        //SPUM
        if (_spum != null) {
            Destroy(_spum.gameObject);
        }
        SPUM_Prefabs spumPrefab = characterBlock.Config.SPUM_Prefabs;
        _spum = Instantiate(spumPrefab, _spumRoot.transform);
        
        _spum.transform.localPosition = Vector3.zero;

        int uiSortingLayer = SortingLayer.NameToID("UI");
        _spum.SetSortingLayer(uiSortingLayer);
    }

    public void ClosePanel() {
        _panel.SetActive(false);
    }
}
