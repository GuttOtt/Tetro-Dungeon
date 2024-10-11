using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Unit.UI {
/// <summary>
/// ������ ���̳� �������� �޾��� ��, �� ��ġ�� ǥ�����ֱ� ���� ���Ǵ� Ŭ����
/// </summary>
    public class UnitHealthText : MonoBehaviour {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private TMP_Text _textPrefab;

        [SerializeField]
        private float _displayingTime = 1f;

        private Dictionary<TMP_Text, float> _textCountDic = new Dictionary<TMP_Text, float>();

        public void DisplayText(int number, Color color) {
            TMP_Text newText = Instantiate(_textPrefab, _canvas.transform);

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
                _textCountDic[text] += Time.deltaTime;

                if (_displayingTime < _textCountDic[text]) {
                    _textCountDic.Remove(text);
                    Destroy(text.gameObject);
                }
            }
        }
    }
}
