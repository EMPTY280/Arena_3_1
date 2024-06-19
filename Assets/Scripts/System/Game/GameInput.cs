using UnityEngine;

namespace ArenaGame
{
    public class GameInput : ArenaBaseObjectMono
    {
        public enum State
        {
            TITLE,
            PLAYER,
            SHOP,
            GAMEOVER
        }

        [SerializeField] private Shop shop;

        private Vector2 direction = Vector2.zero;
        private Player player = null;

        private State controlState = State.TITLE;

        private float gameOverDelay = 0f;

        private void Awake()
        {
            player = Game.Instance.Player;
        }

        void Update()
        {
            direction.Set(0f, 0f);
            float deltaTime = Time.deltaTime;
            switch (controlState)
            {
                case State.TITLE:
                    if (Input.GetKeyDown(KeyCode.Space)
                        || Input.GetMouseButton(0))
                        Game.Instance.StartGame();
                    break;
                case State.PLAYER:
                    if (Input.GetKey(KeyCode.W))
                        direction.y += 1f;
                    if (Input.GetKey(KeyCode.A))
                        direction.x -= 1f;
                    if (Input.GetKey(KeyCode.S))
                        direction.y -= 1f;
                    if (Input.GetKey(KeyCode.D))
                        direction.x += 1f;

                    if (Input.GetMouseButton(0))
                        player.TryFireGun();

                    direction.Normalize();
                    break;
                case State.SHOP:
                    if (Input.GetKeyDown(KeyCode.A))
                        shop.MoveSelector(-1);
                    if (Input.GetKeyDown(KeyCode.D))
                        shop.MoveSelector(1);
                    if (Input.GetKeyDown(KeyCode.Space))
                        shop.Select();
                    break;
                case State.GAMEOVER:
                    gameOverDelay += Time.deltaTime;
                    if (gameOverDelay < 2.3f) break;

                    if (Input.GetKeyDown(KeyCode.Space)
                        || Input.GetMouseButton(0))
                        Game.Instance.ResetGame();
                    break;
            }
            player.TryMove(direction, deltaTime);
        }

        public void SetState(State s)
        {
            controlState = s;
        }
    }
}