using UnityEngine;

namespace ArenaGame
{
    public enum OwnerType
    {
        PLAYER,
        ENEMY
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : ArenaBaseObjectMono
    {
        private Rigidbody2D rb = null;
        private SpriteRenderer coloredSprite = null;
        private SpriteRenderer colorlessSprite = null;
        private Collider2D hitCollider = null;
        private TrailRenderer tr = null;

        OwnerType owner = OwnerType.PLAYER;
        Color bulletColor = Color.white;

        // CHANGE LATER WITH PLAYER STATS
        Damage damage = new(0);
        private float lifeTime = 0;
        private bool isDisappearing = false;
        private float disappearTime = 0.2f;
        private float disappearCounter = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coloredSprite = GetComponent<SpriteRenderer>();
            colorlessSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            hitCollider = GetComponent<Collider2D>();
            tr = GetComponentInChildren<TrailRenderer>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            CalcLifeTime(deltaTime);

            if (isDisappearing)
            {
                disappearCounter -= deltaTime;

                Color colorBuffer = Color.white;
                colorBuffer.a = disappearCounter / disappearTime;
                coloredSprite.color = colorBuffer;

                colorBuffer = Color.white;
                colorBuffer.a = (disappearCounter - deltaTime*2) / disappearTime;
                colorlessSprite.color = colorBuffer;

                colorBuffer = tr.startColor;
                colorBuffer.a = (disappearCounter - deltaTime) / disappearTime;
                tr.startColor = colorBuffer;

                if (disappearCounter <= 0f)
                {
                    isDisappearing = false;
                    gameObject.SetActive(false);
                }
            }
        }

        public void Fire(Vector2 pos, Vector2 dir, OwnerType owner, GunStats stat)
        {
            gameObject.SetActive(true);
            transform.position = pos;
            this.owner = owner;

            rb.velocity = dir * Mathf.Clamp(stat.speed, 5f, 100f);
            damage = stat.damage;
            damage.amount = Mathf.Max(0.1f, damage.amount);
            lifeTime = 5f;

            switch (owner)
            {
                case OwnerType.PLAYER:
                    bulletColor = Color.yellow;
                    break;
                case OwnerType.ENEMY:
                    bulletColor = Color.red;
                    break;
            }

            bulletColor.a = 0.5f;
            coloredSprite.color = bulletColor;
            tr.startColor = bulletColor;
            bulletColor.a = 0f;
            tr.endColor = bulletColor;

            tr.Clear();
        }

        /// <summary>
        /// Calculate lifeTime
        /// </summary>
        private void CalcLifeTime(float deltaTime)
        {
            if (lifeTime <= 0f) return;

            lifeTime -= deltaTime;

            if (lifeTime >= 0f) return;

            gameObject.SetActive(false);
            lifeTime = 0f;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!gameObject.activeSelf || isDisappearing) return;
            if (collision.CompareTag("Wall"))
            {
                Game.Instance.EntityHitParticle.Play(transform.position, Color.white);
                gameObject.SetActive(false);
            }
            else if (collision.TryGetComponent(out IDamageable target))
            {
                if (owner == OwnerType.PLAYER && collision.CompareTag("Player")) return;
                if (owner == OwnerType.ENEMY && collision.CompareTag("Enemy")) return;

                target.Hit(damage);
                gameObject.SetActive(false);
            }
        }

        public void StartDisappear()
        {
            if (isDisappearing) return;

            isDisappearing = true;
            disappearTime = 4f / rb.velocity.magnitude;
            disappearCounter = disappearTime;
            hitCollider.enabled = false;
        }

        public void OnEnable()
        {
            isDisappearing = false;
            hitCollider.enabled = true;
            colorlessSprite.color = Color.white;
        }
    }
}