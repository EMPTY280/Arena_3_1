using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArenaGame
{
    [Serializable]
    public struct ItemData
    {
        public GunStats gunStats;
        public float moveSpeed;
        public float hpRestore;
        public int price;
    }

    public class Card : ArenaBaseObjectMono, IPointerMoveHandler
    {
        [SerializeField] private Shop shop;
        [SerializeField] private ItemData data;
        [SerializeField] private int cardIndex = -1;

        [SerializeField] private Text nameText;
        [SerializeField] private Text descText;
        [SerializeField] private Text priceText;

        private StringBuilder sb = new();
        private EventTrigger eventTrigger = null;
        private List<int> randomSelector = new List<int>();
        private List<int> selectedStats = new List<int>();

        private void Awake()
        {
            eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry onClick = new();
            onClick.eventID = EventTriggerType.PointerClick;
            onClick.callback.AddListener((e) => {
                if ((e as PointerEventData).button != PointerEventData.InputButton.Left) return;
                shop.Select();
            });
            eventTrigger.triggers.Add(onClick);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            shop.SetSelector(cardIndex);
        }

        public void Reroll()
        {
            float goodPoint = UnityEngine.Random.Range(2f, 4f);
            float badPoint = goodPoint * UnityEngine.Random.value;
            float rank = 1 - badPoint / goodPoint;

            // Random Stats Pool
            randomSelector.Clear();
            for (int i = 0; i < 7; i++)
                randomSelector.Add(i);

            // GOOD
            sb.Clear();
            sb.Append("<color=#00ff00>");
            ChangeItemData(goodPoint, 1, sb);
            sb.AppendLine("</color>");

            // BAD
            sb.Append("<color=#ff0000>");
            ChangeItemData(badPoint, -1, sb);
            sb.Append("</color>");
            descText.text = sb.ToString();

            // NAME
            int price = (int)(100 * rank);
            data.price = price;
            Color nameColor = Color.red;
            if (price < 20)
                nameColor = Color.black;
            else if (price < 40)
                nameColor = Color.white;
            else if (price < 60)
                nameColor = Color.cyan;
            else if (price < 80)
                nameColor = Color.magenta;

            nameText.text = shop.GetRandomName();
            nameText.color = nameColor;

            sb.Clear();
            sb.Append(price);
            sb.Append("$");
            priceText.text = sb.ToString();
        }

        private void ChangeItemData(float pointMax, float multi, StringBuilder strBuilder)
        {
            int statCount = 4 - (int)MathF.Sqrt(UnityEngine.Random.Range(1f, 15f));

            selectedStats.Clear();
            for (int i = 0; i < statCount; i++)
            {
                int idx = UnityEngine.Random.Range(0, randomSelector.Count);
                selectedStats.Add(randomSelector[idx]);
                randomSelector.RemoveAt(idx);
            }
            selectedStats.Sort();

            string str = "";
            while (selectedStats.Count != 0)
            {
                float point = UnityEngine.Random.Range(2, (10 / statCount));
                point *= 0.1f * pointMax;
                if (selectedStats.Count == 1)
                    point = pointMax;

                int type = selectedStats[selectedStats.Count - 1];
                float value = multi * point;
                switch (type)
                {
                    case 0: // DAMAGE
                        value *= 0.6f;
                        data.gunStats.damage.amount = value;
                        str = "공격력 ";
                        break;
                    case 1: // MOVESPEED
                        value *= 1f;
                        data.moveSpeed = value;
                        str = "이동 속도 ";
                        break;
                    case 2: // FIRE RATE
                        value *= 1f;
                        data.gunStats.roundPerSec = value;
                        str = "연사 속도 ";
                        break;
                    case 3: // PROJECTILE SPEED
                        value *= 10f;
                        data.gunStats.speed = value;
                        str = "탄속 ";
                        break;
                    case 4: // SPREAD
                        value *= -15f;
                        data.gunStats.spread = value;
                        str = "산탄도 ";
                        break;
                    case 5: // MULTI SHOT
                        value *= 0.75f;
                        data.gunStats.shots = value;
                        str = "탄환 수 ";
                        break;
                    case 6: // HP
                        value *= 1;
                        value = Mathf.Ceil(Mathf.Abs(value)) * multi;
                        data.hpRestore = value;
                        str = "HP ";
                        break;
                }

                if (MathF.Abs(value) > 0.01f)
                {
                    strBuilder.Append(str);
                    if (value >= 0) strBuilder.Append("+");
                    strBuilder.AppendLine((MathF.Ceiling(value * 100) * 0.01f).ToString("0.00"));
                }

                selectedStats.RemoveAt(selectedStats.Count - 1);
                pointMax -= point;
            }
        }

        /// <summary>
        /// Try Purchase
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public bool TryPurchase(int money)
        {
            if (data.price > money)
                return false;
            Game game = Game.Instance;
            game.Player.ModHP(data.hpRestore);
            game.Player.AddStat(data.gunStats, data.moveSpeed);
            game.Money -= data.price;
            return true;
        }
    }
}