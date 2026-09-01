using UnityEngine;

namespace ArTiX.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T s_instance;
        public static T Instance
        {
            get
            {
                s_instance ??= new GameObject(nameof(T), typeof(T)).GetComponent<T>();
                return s_instance;
            }
        }

        protected abstract void Awake();

        protected void SetInstance(in T instance)
        {
            if (s_instance != null)
            {
                Debug.LogWarning($"An instance of {nameof(T)} already exists. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            s_instance = instance;
        }

        private void OnDestroy()
        {
            s_instance = null;
        }
    }
}