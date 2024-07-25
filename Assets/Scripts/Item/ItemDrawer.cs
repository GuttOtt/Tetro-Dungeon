using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDrawer : MonoBehaviour {
    [SerializeField]
    private TMP_Text _nameText, _descriptionText;

    [SerializeField]
    private PolyominoDrawer _polyominoDrawer;

    [SerializeField]
    private Item ForTest;

    private void Start () {
        Draw(ForTest);
    }

    public void Draw(Item item) {
        _nameText?.SetText(item.Name);
        _descriptionText.SetText(item.Description);

        //_polyominoDrawer.Draw(item.Shape);
    }
}
