using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon", menuName = "Scriptable Objects/SO_Weapon")]
public class SO_Weapon : ScriptableObject
{
    [Serializable]
    public struct Stats
    {
        public ushort magazineSize;
        public float fireRate;
        public float reloadTime;
    }

    public Stats stats;
    [Tooltip("Wether you reload the weapon fully or a bullet at a time.")] public bool automaticReload;
    public bool automaticFire;
}