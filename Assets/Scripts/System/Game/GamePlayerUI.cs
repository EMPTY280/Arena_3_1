using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGame
{
    public class GamePlayerUI : ArenaBaseObjectMono
    {
        private StringBuilder sb = new();
        [SerializeField] private Text playerHPText = null;

        [SerializeField] private Text playerUIRoundText;
        private int round = 0;

        [SerializeField] private Text damageText;
        [SerializeField] private Text moveSpeedText;
        [SerializeField] private Text fireRateText;
        [SerializeField] private Text bulletSpeedText;
        [SerializeField] private Text spreadText;
        [SerializeField] private Text shotsText;

        [SerializeField] private CanvasGroup hitEffect;
        [SerializeField] private float hitEffectTime = 0.3f;
        private float hitEffectTimeCurr = 0;


        [SerializeField] private CanvasGroup titleBoard;
        [SerializeField] private CanvasGroup gameUIBoard;
        private bool isGameStarted = false;
        private float startTime = 1.0f;

        [SerializeField] private CanvasGroup gameOverBoard;
        private bool isGameOver = false;
        private float gameOverTimeCurr = 0f;
        private float popUpTiming = 1.5f;

        private void Awake()
        {
            Game.Instance.Player.AddCallbackHPMod(UpdateHPUI);
            Game.Instance.Player.AddCallbackStatChange(UpdateStatUI);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateHitEffect(deltaTime);
            UpdateGameOver(deltaTime);
            UpdateGameStartBoard(deltaTime);
        }

        private void UpdateHitEffect(float deltaTime)
        {
            if (hitEffectTimeCurr <= 0f) return;
            hitEffectTimeCurr -= deltaTime;
            hitEffect.alpha = hitEffectTimeCurr / hitEffectTime * 0.5f;
        }

        private void UpdateGameStartBoard(float deltaTime)
        {
            if (!isGameStarted) return;
            startTime -= deltaTime;
            titleBoard.alpha = startTime;
            gameUIBoard.alpha = 1 - startTime;
        }

        private void UpdateGameOver(float deltaTime)
        {
            if (!isGameOver) return;
            gameOverTimeCurr += deltaTime;
            gameOverBoard.alpha = MathF.Max(gameOverTimeCurr - popUpTiming, 0f) / 2.5f;
        }

        private void UpdateHPUI(float hp, bool isDamage)
        {
            sb.Clear();
            for (int i = 0; i < hp; i++)
            {
                if (i > 4)
                {
                    sb.Append(" +");
                    sb.Append(hp - 5);
                    break;
                }
                sb.Append("O");
            }

            if (hp <= 1)
                playerHPText.color = Color.red;
            else if (hp <= 3)
                playerHPText.color = Color.yellow;
            else
                playerHPText.color = Color.green;

            playerHPText.text = sb.ToString();

            if (isDamage)
                hitEffectTimeCurr = hitEffectTime;
        }

        public void UpdateStatUI(GunStats stat, float moveSpeed, float trueSpeed)
        {
            sb.Clear();
            sb.Append(MathF.Round(Mathf.Max(0.1f, stat.damage.amount), 2));
            if (stat.damage.amount < 0.1f)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(stat.damage.amount, 2));
                sb.Append(")");
            }
            damageText.text = sb.ToString();

            sb.Clear();
            sb.Append(MathF.Round(moveSpeed, 2));
            if (trueSpeed < 0f || trueSpeed > 60)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(trueSpeed, 2));
                sb.Append(")");
            }
            moveSpeedText.text = sb.ToString();

            sb.Clear();
            sb.Append(MathF.Round(Mathf.Max(0.2f, stat.roundPerSec), 2));
            sb.Append(" / Sec");
            if (stat.roundPerSec < 0.2f)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(stat.roundPerSec, 2));
                sb.Append(")");
            }
            fireRateText.text = sb.ToString();

            sb.Clear();
            sb.Append(MathF.Round(Mathf.Clamp(stat.speed, 5f, 100f), 2));
            if (stat.speed > 100f || stat.speed < 5f)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(stat.speed, 2));
                sb.Append(")");
            }
            bulletSpeedText.text = sb.ToString();

            sb.Clear();
            sb.Append(MathF.Round(Mathf.Clamp(stat.spread, 0f, 360f), 2));
            sb.Append(" deg");
            if (stat.spread > 360f || stat.spread < 0f)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(stat.spread, 2));
                sb.Append(")");
            }
            spreadText.text = sb.ToString();

            sb.Clear();
            sb.Append(Mathf.Max(1, MathF.Round(stat.shots, 2)));
            if (stat.shots < 1f)
            {
                sb.Append(" (");
                sb.Append(MathF.Round(stat.shots, 2));
                sb.Append(")");
            }
            shotsText.text = sb.ToString();
        }

        public void UpdateRoundText()
        {
            sb.Clear();
            sb.Append("¶ó¿îµå ");
            sb.Append(++round);
            playerUIRoundText.text = sb.ToString();
        }

        public void StartGame()
        {
            isGameStarted = true;
        }

        public void SetGameOver()
        {
            isGameOver = true;
        }
    }
}