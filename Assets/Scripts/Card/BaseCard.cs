using UnityEngine;

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
        private UnitConfig _unitConfig;
        private TroopCard _troopCard;
        #endregion

        #region properties
        public Polyomino Polyomino { get => _troopCard.Polyomino; }
        public UnitConfig UnitConfig { get => _unitConfig; private set => _unitConfig = value; }
        #endregion

        public void Init(UnitConfig unitConfig, TroopCard troopCard) {
            UnitConfig = unitConfig;
            _troopCard = troopCard;

            DrawPolyomino();
            DrawUnit();
        }

        #region Presentation
        private void DrawPolyomino() {
            if (Polyomino == null) return;

            _polyominoDrawer.Draw(Polyomino.Shape);
        }

        private void DrawUnit() {
            _unitInCardDrawer.Draw(UnitConfig);
        }
        #endregion

    }
}