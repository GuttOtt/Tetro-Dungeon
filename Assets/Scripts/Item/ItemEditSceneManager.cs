using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemEditSceneManager : MonoBehaviour {
    [SerializeField]
    private List<Item> _allItem;

    [SerializeField]
    private ItemDrawer _itemDrawerPrefab;

    private void Awake() {
        _allItem = Resources.LoadAll<Item>("ScriptableObjects/Item").ToList();
    }

    private void DisplayAllItem() {
        foreach (var item in _allItem) {
            ItemDrawer drawer = Instantiate(_itemDrawerPrefab);
            drawer.Draw(item);
        }

        Arrange();
    }

    private void Arrange() {

    }
}
