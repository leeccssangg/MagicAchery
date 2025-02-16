using UnityEngine;

namespace Core.SimplePool
{
    // simple pool for mono behaviour will be implemented in the future
    public interface IPoolAble<out T> where T : Component 
    {
        T OnSpawn();
        void OnDespawn();
    }
    public static class PoolAbleExtension
    {
        public static T Spawn<T>(this IPoolAble<T> poolAble) where T : Component
        {
            T t = TW.Utility.DesignPattern.SimplePool.Spawn(poolAble as T);
            IPoolAble<T> poolAble1 = t as IPoolAble<T>;
            poolAble1?.OnSpawn();
            return t;
        }
        public static T Spawn<T>(this IPoolAble<T> poolAble, Vector3 position, Quaternion rotation) where T : Component
        {
            T t = TW.Utility.DesignPattern.SimplePool.Spawn(poolAble as T, position, rotation);
            IPoolAble<T> poolAble1 = t as IPoolAble<T>;
            poolAble1?.OnSpawn();
            return t;
        }
        public static T Spawn<T>(this IPoolAble<T> poolAble, Transform parent) where T : Component
        {
            T t = TW.Utility.DesignPattern.SimplePool.Spawn(poolAble as T, parent);
            IPoolAble<T> poolAble1 = t as IPoolAble<T>;
            poolAble1?.OnSpawn();
            return t;
        }
        public static T Spawn<T>(this IPoolAble<T> poolAble, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            T t = TW.Utility.DesignPattern.SimplePool.Spawn(poolAble as T, position, rotation, parent);
            IPoolAble<T> poolAble1 = t as IPoolAble<T>;
            poolAble1?.OnSpawn();
            return t;
        }
        public static void Despawn<T>(this IPoolAble<T> poolAble) where T : Component
        {
            poolAble.OnDespawn();
            TW.Utility.DesignPattern.SimplePool.DeSpawn(((T)poolAble).gameObject);
        }
    }
}