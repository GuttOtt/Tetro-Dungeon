using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Card
{
    public class BaseCard : MonoBehaviour, ICard
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
        private BlockCard _blockCard;
        #endregion

        #region properties
        public BlockCard BlockCard { get => _blockCard; }
        public Polyomino Polyomino { get => _blockCard.Polyomino; }
        public UnitConfig UnitConfig { get; private set; }

        public int Cost {
            get {
                return Polyomino.Size;
            }
        }
        #endregion

        public void Init(UnitConfig unitConfig, BlockCard troopCard) {
            UnitConfig = unitConfig;
            _blockCard = troopCard;
            DrawPolyomino();
            DrawUnit();
            DrawTroopDescription();
        }
        public void Init(CardData _card) {
            UnitConfig = _card.UnitConfig;
            _blockCard = _card.BlockCard;
            DrawPolyomino();
            DrawUnit();
            DrawTroopDescription();
        }

        public void SetCardData(ICard card)
        {
            UnitConfig = card.UnitConfig;
            _blockCard = card.BlockCard;
            DrawPolyomino();
            DrawUnit();
            DrawTroopDescription();
        }

        // 카드 클릭 이벤트 핸들러 설정
        public void SetOnClickListener(UnityEngine.Events.UnityAction action)
        {
            GetComponent<Button>().onClick.AddListener(action);
        }


        #region Presentation
        private void DrawPolyomino() {
            if (Polyomino == null) return;

            _polyominoDrawer.Draw(Polyomino.Shape);
        }

        private void DrawUnit() {
            _unitInCardDrawer.Draw(UnitConfig, BlockCard.StatDecorator.Attack, BlockCard.StatDecorator.HP);
        }

        private void DrawTroopDescription() {
            _troopDescription.text = "";
            _troopDescription.text += BlockCard.TroopEffect.Description;
        }

        public ICard DeepCopy()
        {
            BaseCard newCard = new GameObject().AddComponent<BaseCard>();
            newCard._polyominoDrawer = _polyominoDrawer;
            newCard._unitInCardDrawer = _unitInCardDrawer;
            newCard._troopDescription = _troopDescription;
            newCard.Init(this.UnitConfig, this.BlockCard);
            return newCard;
        }
        #endregion

    }
}