using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{ 
    public class EnemyChaser : EntityEnemy
    {
        [SerializeField] private Transform target;
        [SerializeField] private EntityMovement movement;

        protected override void OnAwake()
        {
            target = Game.Instance.Player.transform;
            if (TryGetComponent(out EntityGun gun))
                gun.SetTarget(target);
        }

        protected override void OnUpdate()
        {
            Vector2 dir = (target.position - transform.position).normalized;
            movement.AddForce(dir, Time.deltaTime);
        }
    }
}