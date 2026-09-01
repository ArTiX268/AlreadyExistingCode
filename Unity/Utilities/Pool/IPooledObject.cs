using UnityEngine;

namespace ArTiX.Utils.Pool
{
    public interface IPooledObject<T> where T : Component
    {
        public Pool<T> Pool { get; set; }
        public void Release();
    }
}