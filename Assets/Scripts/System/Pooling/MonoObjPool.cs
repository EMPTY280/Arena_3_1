using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    public class MonoObjPool<T> : ArenaBaseObjectMono
    {
        [SerializeField] protected GameObject prefab = null;
        protected LinkedList<T> items = new LinkedList<T>();
        [SerializeField] protected int initailCount = 10;

        private void Awake()
        {
            if (prefab != null)
                InstantiatePrefab(initailCount);
        }

        public void SetPrefab(GameObject prefab)
        {
            this.prefab = prefab;
            InstantiatePrefab(initailCount);
        }

        protected void InstantiatePrefab(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newItem = Instantiate(prefab, transform);
                if (newItem.TryGetComponent(out T component))
                {
                    ModNewObject(component);
                    items.AddFirst(component);
                }
                newItem.SetActive(false);
            }
        }

        protected virtual void ModNewObject(T obj)
        {

        }

        public T GetItem()
        {
            T inst = items.First.Value;
            if (!CanUse(inst))
            {
                InstantiatePrefab(items.Count);
                inst = items.First.Value;
            }
            items.RemoveFirst();
            items.AddLast(inst);
            return inst;
        }

        protected virtual bool CanUse(T inst)
        {
            return !(inst as MonoBehaviour).gameObject.activeSelf;
        }
    }
}