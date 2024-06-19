using System;

namespace ArenaGame
{
    public class EnemyPool : MonoObjPool<EntityEnemy>
    {
        private Action enemyOnKill = null;

        public void SetKillCallback(Action a)
        {
            enemyOnKill = a;
            foreach(EntityEnemy e in items)
                e.AddListener(a);
        }

        protected override void ModNewObject(EntityEnemy obj)
        {
            if (enemyOnKill == null) return;
            obj.AddListener(enemyOnKill);
        }

        protected override bool CanUse(EntityEnemy inst)
        {
            return inst.CanBeSpawned;
        }
    }
}