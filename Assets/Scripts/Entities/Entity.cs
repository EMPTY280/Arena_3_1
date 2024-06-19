using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

namespace ArenaGame
{
    public abstract class Entity : ArenaBaseObjectMono, IDamageable
    {
        [SerializeField] protected float hpMax = 10f;
        [SerializeField] protected float hpCurr = 10f;

        private SpriteRenderer _SR_HITCOLOR;
        protected Color originColor = Color.white;
        [SerializeField] protected Color hitColor = Color.red;
        protected float hitTimer = 0f;

        private void Awake()
        {
            _SR_HITCOLOR = GetComponent<SpriteRenderer>();
            originColor = _SR_HITCOLOR.color;

            OnAwake();
        }
        protected abstract void OnAwake();

        protected virtual void Update()
        {
            if (hitTimer > 0f)
            {
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0f)
                    hitTimer = 0f;
                _SR_HITCOLOR.color = Color.Lerp(originColor, hitColor, hitTimer * 2.5f);
            }

            OnUpdate();
        }

        protected void ResetHitColor()
        {
            _SR_HITCOLOR.color = originColor;
            hitTimer = 0f;
        }

        protected abstract void OnUpdate();

        public virtual void ModHP(float amount)
        {
            hpCurr += amount;
            if (hpCurr <= 0)
                Kill();
        }

        public DamageResponse Hit(Damage damage)
        {
            hitTimer = 0.4f;
            return OnHit(damage);
        }

        protected abstract void Kill();

        protected abstract DamageResponse OnHit(Damage damage);
    }
}