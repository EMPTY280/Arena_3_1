using System;
using UnityEngine;

namespace ArenaGame
{
    [Serializable]
    public struct GunStats
    {
        public Damage damage;
        public float roundPerSec;
        public float speed;
        public float spread;
        public float shots;
    }

    public class EntityGun : ArenaBaseObjectMono
    {
        private Game game = null;

        [SerializeField] private Transform gunTransform = null;
        [SerializeField] private Transform firePoint = null;

        [SerializeField] private Transform target = null;
        [SerializeField] private float rotateSpeed = 30f;
        [SerializeField] private float fireAngleThreshold = 15f;

        [SerializeField] private OwnerType owner = OwnerType.PLAYER;
        [SerializeField] private GunStats stat = new();

        private float fireDelay = 0f;
        private Quaternion desiredRotation = Quaternion.identity;

        public GunStats Stat
        {
            get
            {
                return this.stat;
            }
        }

        private void Awake()
        {
            game = Game.Instance;
        }

        private void OnEnable()
        {
            fireDelay = 1 / stat.roundPerSec;
        }

        private void Update()
        {
            if (fireDelay > 0f)
                fireDelay -= Time.deltaTime;

            if (target == null) return;
            RotateTo(target.position);

            if (Quaternion.Angle(gunTransform.rotation, desiredRotation) <= fireAngleThreshold)
                Fire();
        }

        /// <summary>
        /// Rotate to target
        /// </summary>
        /// <param name="targetPos"></param>
        public void RotateTo(Vector3 targetPos)
        {
            Vector2 dir = targetPos - gunTransform.position;
            float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            desiredRotation = Quaternion.AngleAxis(deg, Vector3.forward);

            gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Set Target to Attack
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Create a bullet heading gun's rotation
        /// </summary>
        public void Fire()
        {
            if (fireDelay > 0f) return;
            float repeat = Mathf.Max(1, Mathf.Floor(stat.shots));
            for (int i = 0; i < repeat; i++)
            {
                // spread + shots spread
                float spreadClamp = Mathf.Clamp(stat.spread, 0f, 360f);
                float accuracyMod = UnityEngine.Random.Range(-spreadClamp * 0.5f, spreadClamp * 0.5f);
                accuracyMod += (repeat - 1) * -5f + i * 10f;
                Quaternion accRotation = Quaternion.AngleAxis(accuracyMod, Vector3.forward);

                Vector2 direction = gunTransform.rotation * accRotation * Vector2.right;
                game.CreateProjectile().Fire(firePoint.position, direction, owner, stat);
            }
            fireDelay = (1 / Mathf.Max(0.2f, stat.roundPerSec));
        }

        public GunStats ChangeStat(GunStats g)
        {
            stat.damage.amount += g.damage.amount;
            stat.roundPerSec += g.roundPerSec;
            stat.speed += g.speed;
            stat.spread += g.spread;
            stat.shots += g.shots;
            return stat;
        }
    }
}