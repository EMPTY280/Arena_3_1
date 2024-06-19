using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGame
{
    public class GameRound : ArenaBaseObjectMono
    {
        private RectTransform centerTextRect = null;
        private CanvasGroup centerTextGroup = null;
        private Text mainTextShadow;
        private Text mainText;
        private Text subTextShadow;
        private Text subText;

        private StringBuilder sb = new();
        private int round = 0;
        
        private int enemyRemain = -1;
        [SerializeField] private Transform enemyPoolRoot = null;
        [SerializeField] private GameObject[] enemyPrefabs = null;
        private EnemyPool[] enemyPools = null;

        [SerializeField] private SpawnerPool spawnerPool = null;

        private float animTime = 0f;
        private Vector2 animPos = Vector2.zero;

        private bool isSpawning = false;
        private int spawnRemain = 0;
        private int spawnAtOnce = 1;
        [Header("Spawn Order")]
        [SerializeField] private int enemyVarient = 2;
        [SerializeField] private int[] newEnemyRounds;
        private int orderPointer = 0;
        [Header("Spawn Count")]
        [SerializeField] private int spawnCountInitial = 8;
        [SerializeField] private int spawnCountIncrementPerRound = 2;
        [Header("Spawn Delay")]
        [SerializeField] private float spawnDelayInitial = 1f;
        [SerializeField] private float spawnDelayDecrementPerRound = 0.05f;
        [SerializeField] private float spawnDelayMin = 0.25f;
        private float spawnDelay = 1f;
        private float spawnDelayCurr = 0f;

        private bool isGameOver = false;

        private void Awake()
        {
            GameObject centerText = GameObject.FindGameObjectWithTag("CenterText");
            centerTextRect = centerText.GetComponent<RectTransform>();
            centerTextGroup = centerText.GetComponent<CanvasGroup>();
            mainTextShadow = centerTextRect.transform.GetChild(0).GetComponent<Text>();
            mainText = centerTextRect.transform.GetChild(1).GetComponent<Text>();
            subTextShadow = centerTextRect.transform.GetChild(2).GetComponent<Text>();
            subText = centerTextRect.transform.GetChild(3).GetComponent<Text>();

            Init();
        }

        public void Init()
        {
            enemyPools = new EnemyPool[enemyPrefabs.Length];
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                GameObject pool = new(enemyPrefabs[i].name);
                pool.transform.SetParent(enemyPoolRoot);
                enemyPools[i] = pool.AddComponent<EnemyPool>();
                enemyPools[i].SetKillCallback(KilledEnemy);
                enemyPools[i].SetPrefab(enemyPrefabs[i]);
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            CenterTextAnim(deltaTime);
            SpawnEnemy(deltaTime);
        }

        private void CenterTextAnim(float deltaTime)
        {
            if (animTime <= 0f)
                return;

            float animRatio = 0;
            if (animTime <= 0.5f)
            {
                animTime -= deltaTime;
                animRatio = (0.5f - animTime) / 0.5f;
                animPos.y = Mathf.Lerp(0, 25, animRatio);
                centerTextRect.anchoredPosition = animPos;
                centerTextGroup.alpha = 1 - animRatio;
                if (animTime <= 0f)
                    OnAnimationEnd();
            }
            else if (animTime <= 1f)
            {
                animTime -= deltaTime;
                animPos.y = 0;
                centerTextRect.anchoredPosition = animPos;
                centerTextGroup.alpha = 1;
            }
            else // <= 1.5f
            {
                animTime -= deltaTime;
                animRatio = (1.5f - animTime) / 0.5f;
                animPos.y = Mathf.Lerp(-25, 0, animRatio);
                centerTextRect.anchoredPosition = animPos;
                centerTextGroup.alpha = animRatio;
            }
        }

        private void PrepareToSpawn()
        {
            enemyRemain = spawnCountInitial + (spawnCountIncrementPerRound * (round - 1));
            spawnRemain = enemyRemain;

            int phaseCount = Mathf.Max(3, enemyRemain / 5);
            spawnAtOnce = Mathf.Max(2, enemyRemain / phaseCount);
            spawnDelay = spawnDelayInitial - (spawnDelayDecrementPerRound * (round - 1));
            spawnDelay = Mathf.Max(spawnDelayMin, spawnDelay);
        }

        private void CheckNewEnemy()
        {
            if (orderPointer >= newEnemyRounds.Length) return;
            if (round == newEnemyRounds[orderPointer])
            {
                enemyVarient++;
                orderPointer++;
            }
        }

        private void SpawnEnemy(float deltaTime)
        {
            if (!isSpawning || isGameOver) return;
            if (spawnDelayCurr > 0f)
            {
                spawnDelayCurr -= deltaTime;
                return;
            }

            spawnDelayCurr = spawnDelay;
            for (int i = 0; i < spawnAtOnce; i++)
            {
                Vector2 spawnPos = new(Random.Range(-28f, 28f), Random.Range(-14f, 14f));
                EnemySpawner s = spawnerPool.GetItem();
                s.gameObject.SetActive(true);
                EntityEnemy e = enemyPools[Random.Range(0, enemyVarient)].GetItem();
                e.CanBeSpawned = false;
                s.StartSpawning(e, spawnPos);

                spawnRemain--;
                if (spawnRemain <= 0)
                {
                    isSpawning = false;
                    spawnDelayCurr = 0f;
                    return;
                }
            }
        }

        public void StartRound()
        {
            round++;
            PrepareToSpawn();
            CheckNewEnemy();

            sb.Clear();
            sb.Append("라운드 ");
            sb.Append(round);
            SetMainText(sb.ToString());
            SetSubText("");
            StartAnim();
        }

        public void KilledEnemy()
        {
            if (isGameOver) return;
            enemyRemain--;
            if (enemyRemain <= 0)
                Game.Instance.EndRound();
        }

        public void EndRound()
        {
            if (isGameOver) return;
            SetMainText("라운드 완료");
            SetSubText("+50$");
            StartAnim();
        }

        private void OnAnimationEnd()
        {
            // Round Starts
            if (enemyRemain > 0)
                isSpawning = true;
            else // Round Ends
                Game.Instance.ShowShop();
        }

        private void StartAnim()
        {
            animTime = 1.5f;
        }

        private void SetMainText(string str)
        {
            mainTextShadow.text = str;
            mainText.text = str;
        }

        private void SetSubText(string str)
        {
            subTextShadow.text = str;
            subText.text = str;
        }

        public void SetGameOver()
        {
            isGameOver = true;
        }
    }
}