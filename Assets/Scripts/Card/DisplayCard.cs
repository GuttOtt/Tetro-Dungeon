using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Card
{
    public class DisplayCard : MonoBehaviour, ICard, IPointerClickHandler
    {
        #region private members
        [SerializeField]
        private PolyominoDrawer _polyominoDrawer;
        [SerializeField]
        private UnitDrawer _unitInCardDrawer;
        [SerializeField]
        private TMP_Text _troopDescription;
        [SerializeField]
        private UnitConfig _unitConfig;
        private TroopCard _troopCard;
        private UnityEngine.Events.UnityAction _clickAction;
        #endregion

        #region properties
        public TroopCard TroopCard { get => _troopCard; }
        public Polyomino Polyomino { get => _troopCard.Polyomino; }
        public UnitConfig UnitConfig { get; private set; }
        #endregion

        public void Init(UnitConfig unitConfig, TroopCard troopCard) {
            UnitConfig = unitConfig;
            _troopCard = troopCard;
            DrawPolyomino();
            DrawUnit();
            DrawTroopDescription();
        }

        public void Init(CardData card)
        {
            UnitConfig = card.UnitConfig;
            _troopCard = card.TroopCard;
            DrawPolyomino();
            DrawUnit();
            DrawTroopDescription();
        }

        // 카드 클릭 이벤트 핸들러 설정
        public void SetOnClickListener(UnityEngine.Events.UnityAction action)
        {
            _clickAction = action;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _clickAction?.Invoke();
        }

        #region Presentation
        private void DrawPolyomino() {
            if (Polyomino == null) return;

            _polyominoDrawer.Draw(Polyomino.Shape);
        }

        private void DrawUnit() {
            _unitInCardDrawer.Draw(UnitConfig, TroopCard.StatDecorator.Attack, TroopCard.StatDecorator.HP);
        }

        private void DrawTroopDescription() {
            _troopDescription.text = "";
            _troopDescription.text += TroopCard.TroopEffect.Description;
        }

        public ICard DeepCopy()
        {
            DisplayCard newCard = new GameObject().AddComponent<DisplayCard>();
            newCard._polyominoDrawer = _polyominoDrawer;
            newCard._unitInCardDrawer = _unitInCardDrawer;
            newCard._troopDescription = _troopDescription;
            newCard.Init(this.UnitConfig, this.TroopCard);
            return newCard;
        }

        #endregion

    }
}