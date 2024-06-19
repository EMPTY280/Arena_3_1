using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    public class ProjectilePool : MonoObjPool<Projectile>
    {
        public void DeactivateAll()
        {
            foreach (Projectile p in items)
                p.StartDisappear();
        }
    
    }
}