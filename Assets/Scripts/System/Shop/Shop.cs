using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGame
{
    public class Shop : ArenaBaseObjectMono
    {
        private RectTransform rectTransform;
        private Vector2 outScreenPos = Vector2.zero;
        private float animTime = 0f;
        [SerializeField] private float animSpeed = 2f;

        [SerializeField] private Card[] cards;
        [SerializeField] private CardSelector selector = null;
        private int selectorIdx = 2;
        [SerializeField] private RectTransform[] selectorPosition;

        [SerializeField] private Text moneyCounter = null;
        private StringBuilder sb = new();

        private int state = 0; // 0 OUT_SCREEN   1 MOVING_IN   2 IN_SCREEN   3 MOVING_OUT
        private bool canInteract = false;

        [SerializeField] private string[] itemPrefixes;
        [SerializeField] private string[] itemNames;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            outScreenPos.y = Screen.height * 2;
            rectTransform.anchoredPosition = outScreenPos;
        }

        private void Update()
        {
            if (animTime > 0f)
            {
                animTime -= Time.deltaTime * animSpeed;
                if (animTime < 0f)
                    animTime = 0f;
            }

            switch (state)
            {
                case 1:
                    rectTransform.anchoredPosition = Vector2.Lerp(outScreenPos, Vector2.zero, -Mathf.Pow(animTime, 2) + 1);
                    if (TryExitAnimState())
                        canInteract = true;
                    break;
                case 3:
                    rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, outScreenPos, -Mathf.Pow(animTime, 2) + 1);
                    if (TryExitAnimState())
                        Game.Instance.StartNextRound();
                    break;
            }
        }

        public void UpdateMoneyCounter()
        {
            sb.Clear();
            sb.Append(Game.Instance.Money);
            sb.Append("$");
            moneyCounter.text = sb.ToString();
        }

        private bool TryExitAnimState()
        {
            if (animTime > 0f) return false;

            outScreenPos.y *= -1;
            state++;
            return true;
        }

        /// <summary>
        /// Change State to next
        /// </summary>
        public void TryStartNextAnim()
        {
            if (animTime > 0f) return;

            state = (state + 1) % 4;
            animTime = 1f;
        }

        public void ForceCloseShop()
        {
            rectTransform.anchoredPosition = outScreenPos;
            canInteract = false;
            state = 0;
            animTime = 1f;
        }

        /// <summary>
        /// Set card selector's position, returns if tried to move to same pos
        /// </summary>
        /// <param name="idx"></param>
        public bool SetSelector(int idx)
        {
            if (!canInteract) return false;
            if (idx == selectorIdx) return false;
            selectorIdx = idx;
            selector.SetPosition(selectorPosition[selectorIdx]);
            return true;
        }

        public void MoveSelector(int amount)
        {
            if (!canInteract) return;
            selectorIdx += amount;
            if (selectorIdx > 3) selectorIdx = 0;
            else if (selectorIdx < 0) selectorIdx = 3;

            selector.SetPosition(selectorPosition[selectorIdx]);
        }

        public void Select(int idx = -1)
        {
            if (!canInteract) return;
            if (idx == -1)
                idx = selectorIdx;
            switch (idx)
            {
                case 0:
                    TryStartNextAnim();
                    canInteract = false;
                    break;
                default:
                    if (cards[idx - 1].TryPurchase(Game.Instance.Money))
                    {
                        TryStartNextAnim();
                        canInteract = false;
                    }
                    else
                        selector.Alert();
                    break;
            }
        }

        public void Restock()
        {
            foreach (Card c in cards)
                c.Reroll();
        }

        public string GetRandomName()
        {
            sb.Clear();
            sb.Append(itemPrefixes[Random.Range(0, itemPrefixes.Length)]);
            sb.Append(" ");
            sb.Append(itemNames[Random.Range(0, itemNames.Length)]);
            return sb.ToString();
        }
    }
}