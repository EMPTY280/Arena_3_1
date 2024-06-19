using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArenaGame
{
    public class Game : ArenaBaseObjectMono
    {
        static private Game instance = null;
    
        static public Game Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject foundObj = GameObject.FindGameObjectWithTag("GameController");
                    if (foundObj)
                        instance = foundObj.GetComponent<Game>();
                    else
                    {
                        GameObject gameObj = new("===== GAME =====");
                        instance = gameObj.AddComponent<Game>();
                    }
                    instance.Initialize();
                }

                return instance;
            }
        }

        // ******************************** //
        // ******************************** //
        // бу STATIC FIELDS / PROPERTIES бу //
        // ******************************** //
        // ******************************** //

        private Player player = null;
        [SerializeField] private ProjectilePool projectilePool = null;
        private Camera mainCam = null;
        [SerializeField] private EntityHitParticle entityhit = null;
        [SerializeField] private Shop shop = null;

        private GameInput input = null;
        private GameRound round = null;
        private GamePlayerUI playerUI = null;

        private float camShake = 0f;

        private int money = 0;

        private void Initialize()
        {
            //DontDestroyOnLoad(gameObject);
            if (!TryGetComponent(out input))
                input = gameObject.AddComponent<GameInput>();
            if (!TryGetComponent(out round))
                round = gameObject.AddComponent<GameRound>();
            if (!TryGetComponent(out playerUI))
                playerUI = gameObject.AddComponent<GamePlayerUI>();

            mainCam = Camera.main;
            Player.AddCallbackHPMod((curr, isDam) =>
            {
                if (isDam) camShake = 0.3f;
            });
            Player.AddCallbackOnDied(() => {
                camShake = 1.5f;
                GameOver();
            });
        }

        public void StartGame()
        {
            Invoke("StartNextRound", 2f);
            input.SetState(GameInput.State.PLAYER);
            playerUI.StartGame();
        }

        private void Update()
        {
            if (camShake <= 0f) return;

            camShake -= Time.deltaTime;
            if (camShake < 0f) camShake = 0f;
            float shakePower = camShake * 0.5f;
            mainCam.transform.localPosition = new(Random.value * shakePower, Random.value * shakePower, -10);
        }

        /// <summary>
        /// Returns player component
        /// </summary>
        public Player Player
        {
            get
            {
                if (player == null)
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                return player;
            }
        }

        public int Money
        {
            get
            {
                return money;
            }
            set
            {
                money = value;
            }
        }

        public EntityHitParticle EntityHitParticle
        {
            get
            {
                return entityhit;
            }
        }

        /// <summary>
        /// Returns mouse position on screen
        /// </summary>
        public Vector2 MousePosOnScreen
        {
            get
            {
                return mainCam.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public Projectile CreateProjectile()
        {
            return projectilePool.GetItem();
        }

        public void StartNextRound()
        {
            round.StartRound();
            playerUI.UpdateRoundText();
            input.SetState(GameInput.State.PLAYER);
        }

        public void EndRound()
        {
            money += 50;
            shop.UpdateMoneyCounter();
            round.EndRound();
            input.SetState(GameInput.State.SHOP);
            projectilePool.DeactivateAll();
        }

        public void ShowShop()
        {
            shop.TryStartNextAnim();
            shop.Restock();
            shop.SetSelector(2);
        }

        public void GameOver()
        {
            shop.ForceCloseShop();
            input.SetState(GameInput.State.GAMEOVER);
            playerUI.SetGameOver();
            round.SetGameOver();
        }

        public void ResetGame()
        {
            Game.instance = null;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}