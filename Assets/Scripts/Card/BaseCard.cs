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
        #endregion

        #region properties
        public Polyomino Polyomino { get; private set; }
        public UnitConfig UnitConfig { get => _unitConfig; private set => _unitConfig = value; }
        #endregion

        public void Init(UnitConfig unitConfig, Polyomino polyomino) {
            UnitConfig = unitConfig;
            Polyomino = polyomino;
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

        private void Start() {
        }
    }
}