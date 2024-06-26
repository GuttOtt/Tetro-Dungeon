using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Unit.UI
{
    public class UnitHealthBar : MonoBehaviour
    {
        [SerializeField]
        public Slider slider;
        public Gradient gradient;
        public Image fill;
        private void Awake()
        {
            if (slider == null)
            {
                slider = GetComponent<Slider>();
                Debug.LogWarning("Slider was not assigned and had to be fetched in UnitHealthBar");
            }

            if (fill == null && slider != null)
            {
                fill = slider.fillRect.GetComponent<Image>();
                Debug.LogWarning("Fill was not assigned and had to be fetched in UnitHealthBar");
            }
        }

        private void Start()
        {
            // 컴포넌트가 없으면 자동으로 가져오기
            if (slider == null)
                slider = GetComponent<Slider>();
            if (fill == null && slider != null)
                fill = slider.fillRect.GetComponent<Image>();
        }

        public void SetMaxHealth(int health)
        {
            slider.maxValue = health;
            slider.value = health;

            fill.color = gradient.Evaluate(1f);
        }

        public void SetHealth(int health)
        {
            slider.value = health;

            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
