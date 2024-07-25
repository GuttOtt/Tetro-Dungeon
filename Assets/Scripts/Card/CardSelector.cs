using Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour {
    [SerializeField]
    private UnitBlockDrawer _unitBlockMarker;

    private ICard _selectedCard;

    public UnitBlockDrawer UnitblockMarker { get => _unitBlockMarker; }
    public ICard SelectedCard { get => _selectedCard; }

    private void Update() {
        MoveUnitBlockMarker();
    }

    public ICard Select() {
        ICard selectedCard = Utils.Pick<BaseCard>();

        //ī�带 �����ߴٸ� ī�忡 �´� ���� �� ��Ŀ�� ����
        if (_unitBlockMarker != null && selectedCard != null) {
            Debug.Log("Selected");
            _unitBlockMarker.gameObject.SetActive(true);
            _unitBlockMarker.Draw(selectedCard.Polyomino, selectedCard.UnitConfig);
        }

        _selectedCard = selectedCard;

        return selectedCard;
    }


    public void Unselect() {
        if (_unitBlockMarker != null) {
            //Unit Block Marker�� Clear�ϰ� ��Ȱ��ȭ
            _unitBlockMarker.Clear();
            _unitBlockMarker.gameObject.SetActive(false);
        }
    }

    private void MoveUnitBlockMarker() {
        //UnitBlockMarker�� Ȱ��ȭ �Ǿ� �ִٸ�, ���콺�� ���� �����̵��� ��
        if (_unitBlockMarker != null && _unitBlockMarker.gameObject.activeSelf) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = _unitBlockMarker.transform.position.z;
            _unitBlockMarker.transform.position = mousePosition;
        }
    }
}
