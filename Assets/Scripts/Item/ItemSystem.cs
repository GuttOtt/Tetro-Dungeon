using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System.Linq;
using UnityEngine.UI;

public class ItemSystem : MonoBehaviour {
    #region private members
    [SerializeField]
    private Image _itemPanel;

    [SerializeField]
    private ItemDrawer _itemDrawerPrefab;

    [SerializeField]
    private Vector2 _displaySpacing;

    private IGameManager _gameManager;
    private Board _board;
    private List<ItemContainer> _itemContainers = new List<ItemContainer>();
    private bool _isDisplayOn = false;
    
    #endregion

    private class ItemContainer {
        public Item Item;
        public bool IsSatisfied = false;
        public ItemDrawer Drawer;

        public ItemContainer(Item item, ItemDrawer drawer) {
            Item = item;
            Drawer = drawer;
            IsSatisfied = false;
        }
    }

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();

        CloseItemDisplay();

        SetItems();

        _board.onPlaceUnit += UpdateItemState;
        _gameManager.GetSystem<PhaseSystem>().OnStandbyPhase += UpdateItemState;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            if (_isDisplayOn) {
                CloseItemDisplay();
            }
            else {
                OpenItemDisplay();
            }
        }
    }

    private void SetItems() {
        List<Item> items = Player.Instance.ItemInUse;

        foreach (Item item in items) {
            ItemDrawer drawer = Instantiate(_itemDrawerPrefab, _itemPanel.transform);
            drawer.Draw(item);
            ItemContainer container = new ItemContainer(item, drawer);
            _itemContainers.Add(container);
        }

        ArrangeItemDisplay();
    }

    private void ArrangeItemDisplay() {
        Vector2 panelSize = _itemPanel.rectTransform.sizeDelta;
        Vector2 drawerSize = _itemDrawerPrefab.GetComponent<Image>().rectTransform.sizeDelta;
        Vector2 topMiddle = new Vector2(0, panelSize.y / 2f - drawerSize.y / 2f);

        for (int i = 0; i < _itemContainers.Count; i++) {
            ItemDrawer drawer = _itemContainers[i].Drawer;

            drawer.transform.localPosition = topMiddle + - new Vector2(0, drawerSize.y * i) + _displaySpacing * i;
        }
    }

    public void OpenItemDisplay() {
        _itemPanel.gameObject.SetActive(true);
        _isDisplayOn = true;
    }

    public void CloseItemDisplay() {
        _itemPanel.gameObject.SetActive(false);
        _isDisplayOn = false;
    }

    //아이템들의 조건이 만족되었는지 체크함
    public void UpdateItemState() {
        Cell[,] playerCells = _board.GetPlayerCells();
        bool[,] boolArray = Utils.ConvertCellArrayToBoolArray(playerCells);

        foreach(ItemContainer container in _itemContainers) { 
            if (container.Item.IsSatisfiedBy(boolArray)) {
                //처음으로 satisfied 된 거라면, OnSatisfiedEffect를 발동
                if (!container.IsSatisfied) {
                    container.Item.OnSatisfiedEffect(_gameManager.CreateTurnContext());
                }

                container.IsSatisfied = true;
                container.Drawer.SetColor(Color.blue);
            }
            else {
                container.IsSatisfied = false;
                container.Drawer.SetColor(new Color32(255, 214, 214, 214));
            }
        }
    }

    private void resetItemState() {

    }
}
