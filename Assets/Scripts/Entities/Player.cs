using System;
using UnityEngine;

namespace ArenaGame
{
    [RequireComponent(typeof(EntityMovement), typeof(EntityGun))]
    public class Player : Entity
    {
        private EntityMovement movement;
        private EntityGun gun;

        public delegate void COnHPMod(float curr, bool isDamage);
        private COnHPMod onHPMod;
        public delegate void COnStatChange(GunStats s, float moveSpeed, float trueSpeed);
        private COnStatChange onStatChange;
        public Action cOnDied;

        private bool isDead = false;

        public void AddCallbackHPMod(COnHPMod a)
        {
            onHPMod += a;
        }

        public void AddCallbackStatChange(COnStatChange a)
        {
            onStatChange += a;
        }

        public void AddCallbackOnDied(Action a)
        {
            cOnDied += a;
        }

        protected override void OnAwake()
        {
            if (!(TryGetComponent(out movement) &&
                TryGetComponent(out gun)))
                enabled = false;
        }

        private void Start()
        {
            onHPMod(hpCurr, false);
            onStatChange(gun.Stat, movement.MoveSpeed, movement.TrueMoveSpeed);
        }

        protected override void OnUpdate()
        {
            if (isDead)
            {
                ResetHitColor();
                return;
            }
            gun.RotateTo(Game.Instance.MousePosOnScreen);
        }

        public void AddStat(GunStats stat, float moveSpeed)
        {
            movement.MoveSpeed += moveSpeed;
            onStatChange(gun.ChangeStat(stat), movement.MoveSpeed, movement.TrueMoveSpeed);
        }

        public void TryMove(Vector2 dir, float deltaTime)
        {
            if (dir.magnitude == 0)
            {
                movement.SetDragEnabled(true);
                return;
            }
            movement.AddForce(dir, deltaTime);
            movement.SetDragEnabled(false);
        }

        public void TryFireGun()
        {
            gun.Fire();
        }

        protected override DamageResponse OnHit(Damage damage)
        {
            if (isDead) return default;
            hpCurr -= damage.amount;
            onHPMod(hpCurr, true);
            Game.Instance.EntityHitParticle.Play(transform.position, Color.yellow);
            CheckKill();

            DamageResponse response;
            response.amount = damage.amount;
            return response;
        }

        private void CheckKill()
        {
            if (hpCurr <= 0)
                Kill();
        }

        protected override void Kill()
        {
            isDead = true;
            cOnDied();
        }

        public override void ModHP(float amount)
        {
            if (isDead) return;
            base.ModHP(amount);
            onHPMod(hpCurr, false);
            CheckKill();
        }
    }

}