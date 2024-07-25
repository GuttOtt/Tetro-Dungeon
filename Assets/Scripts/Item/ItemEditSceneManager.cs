using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditSceneManager : MonoBehaviour {
    [SerializeField]
    private List<Item> _inventory;

    [SerializeField]
    private ItemDrawer _itemDrawerPrefab;

    [SerializeField]
    private Image _inventoryPanel, _usePanel;

    [SerializeField]
    private Vector2 _gap, _origin;

    private List<ItemDrawer> _drawerInventory = new List<ItemDrawer>();
    private List<ItemDrawer> _drawerUse = new List<ItemDrawer>();

    private void Awake() {
        _inventory = Resources.LoadAll<Item>("Scriptable Objects/Item").ToList();

        DisplayInventory();
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
        ItemDrawer drawer = Instantiate(_itemDrawerPrefab, _usePanel.transform);
        drawer.Draw(item);
        drawer.OnClick += () => DeleteFromUse(item);

        _drawerUse.Add(drawer);

        Arrange(_drawerUse);
    }

    private void DeleteFromUse(Item item) {
        Debug.Log("Delete From Use");
    }
}
