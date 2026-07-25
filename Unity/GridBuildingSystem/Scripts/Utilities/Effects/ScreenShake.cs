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

        [SerializeField, Tooltip("Because the screen shake will move and rotate the camera, it needs a camera holder to reset to it's initial state.")]
        private Transform cameraHolder;
        [SerializeField, Tooltip("Use for the translational part of the shake.")] private Vector2 maxOffset;
        [SerializeField, Tooltip("Use for the rotational part of the shake.")] private float maxAngle;
        [SerializeField, Range(0, 1)] private float shakeDecreaseSpeed = 0.2f;
        [SerializeField] private float jitteriness = 1;
        [SerializeField] private bool scaleWithTime = true;

        private Transform shakyCamera;

        private Vector2 camOffset;
        /// <summary>
        /// Do not set it directly, use AddShake.
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
            angleNoise.SetFrequency(jitteriness);
            xNoise.SetFrequency(jitteriness);
            yNoise.SetFrequency(jitteriness);

            float angle = maxAngle * Shake * angleNoise.GetPerlin(time, 0);
            float offsetX = maxOffset.x * Shake * xNoise.GetPerlin(time, 0);
            float offsetY = maxOffset.y * Shake * yNoise.GetPerlin(time, 0);
            offsetX += camOffset.x;
            offsetY += camOffset.y;

            shakyCamera.eulerAngles = cameraHolder.eulerAngles + (Vector3.forward * angle);
            shakyCamera.position = cameraHolder.position + new Vector3(offsetX, offsetY);

            AddShake(-shakeDecreaseSpeed * (scaleWithTime ? Time.deltaTime : Time.unscaledDeltaTime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensity">Must be between 0 & 1</param>
        public void AddShake(float intensity)
        {
            trauma = Mathf.Clamp(trauma + intensity, 0, 1);
        }

        public void PushCamera(in Vector2 offset, in float duration)
        {
            if (duration <= 0) return;

            Tween.MasterTween.Tween tween = Tween.MasterTween.Create();
            tween.TweenEvent(camOffset, offset, SetCamOffset, 
                animParams: new Tween.MasterTween.AnimParams(
                    duration, 
                    Tween.MasterTween.ETransition.SmoothStopArch3)
            );
        }

        private void SetCamOffset(Vector2 result) => camOffset = result;
    }
}