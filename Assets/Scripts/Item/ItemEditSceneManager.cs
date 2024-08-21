using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts;


public class ItemEditSceneManager : MonoBehaviour {
    [SerializeField]
    private List<Item> _inventory;

    [SerializeField]
    private List<Item> _itemInUse = new List<Item>();

    [SerializeField]
    private ItemDrawer _itemDrawerPrefab;

    [SerializeField]
    private Image _inventoryPanel, _usePanel;

    [SerializeField]
    private Vector2 _gap, _origin;

    [SerializeField]
    private int _maxItemUse = 5;

    [SerializeField]
    private SceneChanger _sceneChanger;

    private List<ItemDrawer> _drawerInventory = new List<ItemDrawer>();
    private List<ItemDrawer> _drawerUse = new List<ItemDrawer>();
    private Player _player;

    private void Awake() {
        _player = Player.Instance;
    }

    private void Start() {
        _inventory = _player.ItemInInv.ToList();
        DisplayInventory();

        List<Item> itemInUse = _player.ItemInUse.ToList();

        foreach (Item item in itemInUse) {
            AddToUse(item);
        }
    }

    private void DisplayInventory() {
        foreach (var item in _inventory) {
            ItemDrawer drawer = Instantiate(_itemDrawerPrefab, _inventoryPanel.transform);
            drawer.Draw(item);
            drawer.OnClick += () => AddToUse(item);

            _drawerInventory.Add(drawer);
        }

        Arrange(_drawerInventory);
    }

    private void Arrange(List<ItemDrawer> drawers) {
        if (drawers.Count == 0)
            return;

        Vector2 drawerSize = drawers[0].GetComponent<RectTransform>().sizeDelta;

        for (int i = 0; i < drawers.Count; i++) {
            int x = i % 2;
            int y = i / 2;

            Vector2 pos = _origin + new Vector2(x * drawerSize.x, -y * drawerSize.y) + new Vector2(x * _gap.x, -y * _gap.y);

            drawers[i].transform.localPosition = pos;
        }
    }

    private void AddToUse(Item item) {
        if (_maxItemUse <= _drawerUse.Count) {
            Debug.Log($"아이템은 최대 {_maxItemUse} 까지만 사용할 수 있습니다.");
            return;
        }

        ItemDrawer drawer = Instantiate(_itemDrawerPrefab, _usePanel.transform);
        drawer.Draw(item);
        drawer.OnClick += () => DeleteFromUse(drawer, item);

        _drawerUse.Add(drawer);
        _itemInUse.Add(item);

        Arrange(_drawerUse);
    }

    private void DeleteFromUse(ItemDrawer drawer, Item item) {
        _drawerUse.Remove(drawer);
        _itemInUse.Remove(item);

        Destroy(drawer.gameObject);

        Arrange(_drawerUse);
    }

    private void SaveItemInUse() {
        _player.SaveItemInUse(_itemInUse);
    }

    public void ToBattleScene() {
        SaveItemInUse();
        _sceneChanger.LoadBattleScene();
    }

    public void ToStageScene() {
        SaveItemInUse();
        _sceneChanger.LoadStageScene();
    }
}
