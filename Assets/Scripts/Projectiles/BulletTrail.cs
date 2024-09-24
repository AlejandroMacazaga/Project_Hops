using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using UnityEngine;
using Utils.Flyweight;

namespace Projectiles
{
    public class BulletTrail : Flyweight
    {
        new BulletTrailSettings settings => (BulletTrailSettings) base.settings;

        [SerializeField]
        private LineRenderer lineRenderer;
        private CoroutineHandle _fadeOutHandle;

        void OnValidate()
        {
            this.ValidateRefs();
        }

        void OnEnable()
        {
            _fadeOutHandle = Timing.RunCoroutine(FadeLineRenderer());
        }

        void Awake()
        {
            lineRenderer ??= GetComponent<LineRenderer>();
        }

        void OnDisable()
        {
            Timing.KillCoroutines(_fadeOutHandle);
            FlyweightManager.ReturnToPool(this);
        }

        public void SetPosition(Vector3 start, Vector3 end)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
        
        IEnumerator<float> FadeLineRenderer ()
        {
            Gradient lineRendererGradient = new Gradient();
            float fadeSpeed = settings.despawnTime;
            float timeElapsed = 0f;
            float alpha = 1f;

            while (timeElapsed < fadeSpeed)
            {
                alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeSpeed);

                lineRendererGradient.SetKeys
                (
                    lineRenderer.colorGradient.colorKeys,
                    new[] { new GradientAlphaKey(alpha, 1f) }
                );
                lineRenderer.colorGradient = lineRendererGradient;

                timeElapsed += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }
            
            FlyweightManager.ReturnToPool(this);
        }
    }
}
