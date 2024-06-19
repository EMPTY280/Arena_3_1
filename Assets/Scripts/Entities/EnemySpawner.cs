using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    public class EnemySpawner : ArenaBaseObjectMono
    {
        [SerializeField] private SpriteRenderer border = null;
        [SerializeField] private SpriteRenderer fill = null;

        [SerializeField] private float spawnTiming = 3f;
        [SerializeField] private float fadeTiming = 3.5f;
        private float spawnTimeCurr = 0f;
        private bool isFading = false;

        private Entity entityToSpawn = null;

        private void Update()
        {
            spawnTimeCurr += Time.deltaTime;

            if (!isFading)
            {
                float spawnRatio = spawnTimeCurr / spawnTiming;

                Color borderColor = Color.white;
                borderColor.a = Mathf.Min(0.5f, spawnRatio * 3);
                Color fillColor = Color.Lerp(Color.yellow, Color.red, spawnRatio);
                fillColor.a = borderColor.a;

                border.color = borderColor;
                fill.color = fillColor;
                fill.size = new(2, spawnRatio * 2);

                if (spawnRatio >= 1)
                {
                    entityToSpawn.gameObject.SetActive(true);
                    entityToSpawn.transform.position = transform.position;
                    isFading = true;
                }
            }
            else
            {
                float fadeRatio = (spawnTimeCurr - spawnTiming) / (fadeTiming - spawnTiming);

                Color borderColor = Color.white;
                borderColor.a = (1 - fadeRatio) * 0.5f;
                Color fillColor = Color.red;
                fillColor.a = borderColor.a;
                border.color = borderColor;
                fill.color = fillColor;

                Vector3 newScale = 0.1f * Mathf.Cos(fadeRatio * 10) * Vector3.one + Vector3.one;
                transform.localScale = newScale;

                if (fadeRatio >= 1)
                    gameObject.SetActive(false);
            }
        }

        public void StartSpawning(Entity spawnTarget, Vector2 pos)
        {
            transform.position = pos;
            entityToSpawn = spawnTarget;
            spawnTimeCurr = 0f;
            transform.localScale = Vector3.one;
            isFading = false;
        }
    }
}