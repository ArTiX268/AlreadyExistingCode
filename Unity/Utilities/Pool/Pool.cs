using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Utils.Pool
{
    public class Pool<T> where T : Component
    {
        private readonly Stack<T> pool = new Stack<T>();
        private readonly int maxSize;

        private readonly Func<T> populateMethod;

        private int totalCount;

        public Pool(Func<T> populateMethod, int initialSize, int maxSize)
        {
            this.populateMethod = populateMethod;

            for (int i = 0; i < initialSize; i++)
            {
                pool.Push(Create());
            }

            this.maxSize = maxSize;

            Application.quitting += EmptyPool;
        }

        public T GetPooledObject()
        {
            if (pool.TryPop(out T objectTaken))
            {
                objectTaken.gameObject.SetActive(true);
                return objectTaken;
            }

            if (totalCount >= maxSize) return null;

            objectTaken = Create();
            objectTaken.gameObject.SetActive(true);

            return objectTaken;
        }

        public void ReturnToPool(in T obj)
        {
            if (pool.Contains(obj)) return;

            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }

        private T Create()
        {
            T instance = populateMethod();
            instance.GetComponent<IPooledObject<T>>().Pool = this;
            instance.gameObject.SetActive(false);
            totalCount++;

            return instance;
        }

        private void EmptyPool()
        {
            pool.Clear();
            totalCount = 0;
        }
    }
}
