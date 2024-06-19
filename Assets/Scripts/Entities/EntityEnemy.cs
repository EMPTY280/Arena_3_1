using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    public abstract class EntityEnemy : Entity
    {
        private Action onKill;
        private bool canBeSpawned = true;

        [SerializeField] private Damage meleeDamage;
        [SerializeField] private bool canMeleeAttack = false;
        [SerializeField] private float meleeDelay = 1f;
        private float meleeDelayCurr = 0f;

        protected override void Update()
        {
            base.Update();
            if (meleeDelayCurr > 0f)
                meleeDelayCurr -= Time.deltaTime;
        }

        public bool CanBeSpawned
        {
            get
            {
                return canBeSpawned;
            }
            set
            {
                canBeSpawned = value;
            }
        }

        public void AddListener(Action a)
        {
            onKill += a;
        }

        protected override void Kill()
        {
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);
            canBeSpawned = true;
            onKill();
        }

        protected override DamageResponse OnHit(Damage damage)
        {
            hpCurr -= damage.amount;
            Game.Instance.EntityHitParticle.Play(transform.position, Color.red);
            if (hpCurr <= 0)
                Kill();

            DamageResponse response;
            response.amount = damage.amount;
            return response;
        }

        private void OnEnable()
        {
            hpCurr = hpMax;
            ResetHitColor();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            MeleeAttack(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            MeleeAttack(collision);
        }

        private void MeleeAttack(Collision2D collision)
        {
            if (!canMeleeAttack
                || meleeDelayCurr > 0f
                || !collision.gameObject.CompareTag("Player")) return;
            if (collision.gameObject.TryGetComponent(out IDamageable player))
            {
                player.Hit(meleeDamage);
                meleeDelayCurr = meleeDelay;
            }
        }
    }
}