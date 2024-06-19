using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    [Serializable]
    public struct Damage
    {
        public Damage(float _amount)
        {
            amount = _amount;
        }

        public float amount;
    }

    public struct DamageResponse
    {
         public float amount;
    }

    public interface IDamageable
    {
        public DamageResponse Hit(Damage damage);
    }
}