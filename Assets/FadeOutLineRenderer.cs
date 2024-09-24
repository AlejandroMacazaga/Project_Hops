using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public class FadeOutLineRenderer : ValidatedMonoBehaviour
{
    [SerializeField, Self] private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(FadeLineRenderer());
    }
    
    IEnumerator FadeLineRenderer ()
    {
        Gradient lineRendererGradient = new Gradient();
        float fadeSpeed = 1f;
        float timeElapsed = 0f;
        float alpha = 1f;

        while (timeElapsed < fadeSpeed)
        {
            alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeSpeed);

            lineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f) }
            );
            lineRenderer.colorGradient = lineRendererGradient;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
