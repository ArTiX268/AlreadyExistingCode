using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool addToDontDestroyOnLoad;

    private GameObject emptyHolder;

    private static GameObject particleSystemEmpty;
    private static GameObject gameObjectEmpty;
    private static GameObject soundFXEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
    private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

    public enum PoolType
    {
        ParticleSystem,
        GameObject,
        SoundFX
    }

    public static PoolType PoolingType;

    #endregion

    private void Awake()
    {
        objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    private void SetupEmpties()
    {
        emptyHolder = new GameObject("Object Pools");

        particleSystemEmpty = new GameObject("Particle Systems");
        particleSystemEmpty.transform.SetParent(emptyHolder.transform);

        gameObjectEmpty = new GameObject("Game Objects");
        gameObjectEmpty.transform.SetParent(emptyHolder.transform);

        soundFXEmpty = new GameObject("SoundFX");
        soundFXEmpty.transform.SetParent(emptyHolder.transform);

        if (addToDontDestroyOnLoad)
            DontDestroyOnLoad(particleSystemEmpty.transform.root);
    }

    #region Create Pool

    private static void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObject)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, position, rotation, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
            );

        objectPools.Add(prefab, pool);
    }

    private static void CreatePool(GameObject prefab, Transform parent, Quaternion rotation, PoolType poolType = PoolType.GameObject)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, parent, rotation, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
            );

        objectPools.Add(prefab, pool);
    }

    #endregion

    #region Create Object

    private static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObject)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, position, rotation);

        prefab.SetActive(true);

        Transform parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject);

        return obj;
    }

    private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion localRotation, PoolType poolType = PoolType.GameObject)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, parent);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = Vector3.one;

        prefab.SetActive(true);

        return obj;
    }

    private static Transform SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObject:
                return gameObjectEmpty.transform;

            case PoolType.ParticleSystem:
                return particleSystemEmpty.transform;

            case PoolType.SoundFX:
                return soundFXEmpty.transform;

            default:
                return null;
        }
    }

    #endregion

    #region Pool Function

    private static void OnGetObject(GameObject obj)
    {

    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (cloneToPrefabMap.ContainsKey(obj))
        {
            cloneToPrefabMap.Remove(obj);
        }
    }

    #endregion

    #region Spawn Object Function

    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion rotation, PoolType poolType = PoolType.GameObject) where T : Object
    {
        if (!objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPosition, rotation, poolType);
        }

        GameObject obj = objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!cloneToPrefabMap.ContainsKey(obj))
                cloneToPrefabMap.Add(obj, objectToSpawn);

            obj.transform.position = spawnPosition;
            obj.transform.rotation = rotation;

            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}.");
                return null;
            }

            return component;
        }

        return null;
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion rotation, PoolType poolType = PoolType.GameObject) where T : Object
    {
        if (!objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, parent, rotation, poolType);
        }

        GameObject obj = objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!cloneToPrefabMap.ContainsKey(obj))
                cloneToPrefabMap.Add(obj, objectToSpawn);

            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = rotation;

            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}.");
                return null;
            }

            return component;
        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.GameObject) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPosition, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation,  PoolType poolType = PoolType.GameObject)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPosition, spawnRotation, poolType);
    }

    public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObject) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObject)
    {
        return SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
    }

    #endregion

    public static void ReturnObjectToPool(GameObject objectToReturn, PoolType poolType = PoolType.GameObject)
    {
        if (cloneToPrefabMap.TryGetValue(objectToReturn, out GameObject prefab))
        {
            Transform parentObject = SetParentObject(poolType);

            if (objectToReturn.transform.parent != parentObject)
                objectToReturn.transform.SetParent(parentObject);

            if (objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                pool.Release(objectToReturn);
        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " +  objectToReturn.name);
        }
    }
}