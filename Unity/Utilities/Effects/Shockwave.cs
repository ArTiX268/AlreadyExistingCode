using ArTiX.Effects.Tween;
using System;
using UnityEngine;

namespace ArTiX.Shmup.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Shockwave : MonoBehaviour
    {
        [Serializable]
        public struct SShockwave
        {
            public float targetRadius;
            public MasterTween.AnimParams animParams;
            [Range(0, MAX_WIDTH)] public float width;
            [Range(0, MAX_CHROMATIC_ABERRATION)] public float chromaticAberrationStrength;
        }

        private static readonly Pool<Shockwave> pool;
        private static Shockwave prfb_Shockwave;

        static Shockwave()
        {
            pool = new Pool<Shockwave>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetRadius"></param>
        /// <param name="animParams"></param>
        /// <param name="width">Must be in [0;0.25] range.</param>
        /// <param name="chromaticAberrationStrength">Must be in [0;0.1] range.</param>
        /// <returns></returns>
        public static Shockwave Create(in Vector3 position, in SShockwave datas)
        {
            pool.Take(out Shockwave shockwave);
            if (shockwave == null)
            {
                if (prfb_Shockwave == null) prfb_Shockwave = Resources.Load<GameObject>(SHOCKWAVE_PATH).GetComponent<Shockwave>();
                shockwave = Instantiate(prfb_Shockwave);
            }

            shockwave.transform.position = position;
            shockwave.transform.localScale = new Vector3(datas.targetRadius, datas.targetRadius, 1);
            shockwave.animParams = datas.animParams;
            shockwave.material.SetFloat(WIDTH, Mathf.Clamp(datas.width, 0, MAX_WIDTH));
            shockwave.material.SetFloat(CHROMATIC_ABERRATION_STRENGTH, Mathf.Clamp(datas.chromaticAberrationStrength, 0, MAX_CHROMATIC_ABERRATION));
            return shockwave;
        }

        private const string SHOCKWAVE_PATH = "Effects/ShockWave";

        private const string RADIUS = "_Radius";
        private const string WIDTH = "_Width";
        private const string CHROMATIC_ABERRATION_STRENGTH = "_ChromaticAberrationOffset";

        private const float MAX_RADIUS = 0.5f;
        private const float MAX_WIDTH = 0.5f;
        private const float MAX_CHROMATIC_ABERRATION = 0.5f;

        private Material material;

        private MasterTween.AnimParams animParams;

        private Shockwave() { }

        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material;
        }

        private void OnEnable()
        {
            material.TweenFloatProperty(RADIUS, 1, animParams).SetFinishedEvent(Kill);
        }

        private void Kill()
        {
            material.SetFloat(RADIUS, 0);
            pool.MoveIn(this);
        }

        private void OnDestroy()
        {
            pool.Remove(this);
        }
    }
}