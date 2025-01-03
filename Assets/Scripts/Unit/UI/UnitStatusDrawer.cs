using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatusDrawer : MonoBehaviour {
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Vector2 _origin;
    [SerializeField] private Vector2 _statusImageSize;

    private List<Image> _statusImages = new List<Image>();

    public void DrawStatuses(List<Status> statuses) {
        if (_canvas == null || _canvas.transform == null)
            return;

        ClearImages();

        foreach(Status status in statuses) {
            DrawStatus(status);
        }

        Arrange();
    }

    private void ClearImages() {
        foreach (Image image in _statusImages) {
            Destroy(image);
        }
        _statusImages.Clear();
    }

    private void DrawStatus(Status status) {
        if (status.IconSprite == null)
            return;

        GameObject imageObject = new GameObject($"Status Image: {status.Name}");
        Image statusImage = imageObject.AddComponent<Image>();

        statusImage.sprite = status.IconSprite;
        statusImage.rectTransform.sizeDelta = _statusImageSize;
        statusImage.transform.parent = _canvas.transform;
        statusImage.transform.localScale = new Vector3(1, 1, 1);

        _statusImages.Add(statusImage);
    }

    private void Arrange() {
        for (int i = 0; i < _statusImages.Count; i++) {
            _statusImages[i].transform.localPosition = _origin + Vector2.right * i * _statusImageSize.x;
        }
    }
}
