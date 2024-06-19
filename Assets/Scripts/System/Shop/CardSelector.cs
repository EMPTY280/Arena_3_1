using ArenaGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ArenaGame
{
    public class CardSelector : ArenaBaseObjectMono
    {
        [SerializeField] private Text alertText = null;
        private RectTransform rectTransform = null;
        private Image image = null;

        private Color clear = new(0f, 0.75f, 1f, 0f);
        private Color red = Color.red;
        private Color blue = new(0f, 0.75f, 1f);

        private float time = 0f;

        private float alerting = 0f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (alerting > 0f)
            {
                alerting -= Time.deltaTime;
                image.color = Color.Lerp(blue, red, alerting * 2.5f);
                alertText.color = Color.Lerp(Color.clear, red, alerting * 2.5f);
            }
            else
            {
                time += Time.deltaTime * 2f;
                image.color = Color.Lerp(clear, blue, Mathf.Abs(Mathf.Cos(time)));
            }
        }

        public void Alert()
        {
            alerting = 0.4f;
            time = 0f;
        }

        public void SetPosition(RectTransform rect)
        {
            rectTransform.anchoredPosition = rect.anchoredPosition;
            Vector2 newSize = rect.sizeDelta;
            newSize.x += 20f;
            newSize.y += 20f;
            rectTransform.sizeDelta = newSize;
            time = 0f;
        }
    }
}