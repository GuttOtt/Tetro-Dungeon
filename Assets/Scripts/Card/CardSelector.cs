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

        //카드를 선택했다면 카드에 맞는 유닛 블럭 마커를 생성
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
            //Unit Block Marker를 Clear하고 비활성화
            _unitBlockMarker.Clear();
            _unitBlockMarker.gameObject.SetActive(false);
        }
    }

    private void MoveUnitBlockMarker() {
        //UnitBlockMarker가 활성화 되어 있다면, 마우스를 따라 움직이도록 함
        if (_unitBlockMarker != null && _unitBlockMarker.gameObject.activeSelf) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = _unitBlockMarker.transform.position.z;
            _unitBlockMarker.transform.position = mousePosition;
        }
    }
}
