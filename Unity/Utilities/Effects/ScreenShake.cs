using Noise;
using UnityEngine;

namespace ArTiX.Effects
{
    [RequireComponent(typeof(Camera))]
    public class ScreenShake : MonoBehaviour
    {
        private static ScreenShake instance;
        public static ScreenShake Instance
        {
            get
            {
                instance ??= new ScreenShake();
                return instance;
            }
        }

        [SerializeField, Tooltip("Use for the translational part of the shake.")] private Vector2 maxOffset;
        [SerializeField, Tooltip("Use for the rotational part of the shake.")] private float maxAngle;
        [SerializeField, Range(0, 1)] private float shakeDecreaseSpeed = 0.2f;
        [SerializeField] private float noiseScale = 1;

        private Transform shakyCamera;

        /// <summary>
        /// Do not set it directly, use SetTrauma.
        /// </summary>
        private float trauma;

        private float Shake => trauma * trauma * trauma;

        private void Start()
        {
            instance = this;
            shakyCamera = transform;
        }

        private void Update()
        {
            float time = Time.time;

            FastNoise angleNoise = new FastNoise(1);
            FastNoise xNoise = new FastNoise(2);
            FastNoise yNoise = new FastNoise(3);
            angleNoise.SetFrequency(noiseScale);
            xNoise.SetFrequency(noiseScale);
            yNoise.SetFrequency(noiseScale);

            float angle = maxAngle * Shake * angleNoise.GetPerlin(time, 0);
            float offsetX = maxOffset.x * Shake * xNoise.GetPerlin(time, 0);
            float offsetY = maxOffset.y * Shake * yNoise.GetPerlin(time, 0);

            Transform baseCamera = Camera.main.transform;
            shakyCamera.eulerAngles = baseCamera.eulerAngles + (Vector3.forward * angle);
            shakyCamera.position = baseCamera.position + new Vector3(offsetX, offsetY, 0);

            AddShake(-shakeDecreaseSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensity">Must be between 0 & 1</param>
        public void AddShake(float intensity)
        {
            trauma = Mathf.Clamp(trauma + intensity, 0, 1);
        }
    }
}