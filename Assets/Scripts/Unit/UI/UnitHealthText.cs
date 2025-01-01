using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Unit.UI {
/// <summary>
/// 유닛이 힐이나 데미지를 받았을 때, 그 수치를 표시해주기 위해 사용되는 클래스
/// </summary>
    public class UnitHealthText : MonoBehaviour {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private TMP_Text _textPrefab;

        [SerializeField]
        private float _displayingTime = 1f;

        private Dictionary<TMP_Text, float> _textCountDic = new Dictionary<TMP_Text, float>();

        private Vector2 _canvasSize;

        public void DisplayText(int number, Color color) {
            if (number == 0 || _canvas == null || _canvas.transform == null)
                return;

            //Set random position in the canvas
            if (_canvasSize == null) {
                _canvasSize = _canvas.GetComponent<RectTransform>().sizeDelta;
            }
            float xPos = Random.Range(-30, 30);
            float yPos = Random.Range(-80, -20);
            Vector2 textPos = new Vector2(xPos, yPos);

            TMP_Text newText = Instantiate(_textPrefab, _canvas.transform);
            newText.transform.localPosition = new Vector3(textPos.x, textPos.y, 0);

            if (0 < number) {
                newText.text = "+";
            }

            newText.text = number.ToString();
            newText.color = color;

            _textCountDic.Add(newText, 0f);
        }

        private void Update() {
            Dictionary<TMP_Text, float> temp = new Dictionary<TMP_Text, float>(_textCountDic);

            foreach (TMP_Text text in temp.Keys) {
                //movement
                text.transform.localPosition += Vector3.up * 0.05f;

                _textCountDic[text] += Time.deltaTime;

                if (_displayingTime < _textCountDic[text]) {
                    _textCountDic.Remove(text);
                    Destroy(text.gameObject);
                }
            }
        }
    }
}
