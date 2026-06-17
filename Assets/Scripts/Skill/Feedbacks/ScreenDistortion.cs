using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Data/SkillFeedback/ScreenDistortion")]
public class ScreenDistortion : SkillFeedback
{
    static readonly int StrengthId = Shader.PropertyToID("_Strength");
    static readonly int RadiusId = Shader.PropertyToID("_Radius");
    static readonly int CenterId = Shader.PropertyToID("_Center");

    [SerializeField] private Material distortionMaterial;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float strength = 0.015f;
    [SerializeField] private float radius = 0.45f;
    [SerializeField] private Vector2 center = new Vector2(0.5f, 0.5f);

    static Coroutine activeCoroutine;
    static Material activeMaterial;

    public override void Play()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogWarning("[ScreenDistortionFeedback] BattleManager.Instance is missing.");
            return;
        }

        if (distortionMaterial == null)
        {
            Debug.LogWarning("[ScreenDistortionFeedback] Distortion material is missing.");
            return;
        }

        if (activeCoroutine != null)
        {
            BattleManager.Instance.StopCoroutine(activeCoroutine);
            ResetMaterial(activeMaterial);
        }

        activeMaterial = distortionMaterial;
        activeCoroutine = BattleManager.Instance.StartCoroutine(ApplyDistortion());
    }

    private IEnumerator ApplyDistortion()
    {
        distortionMaterial.SetFloat(RadiusId, radius);
        distortionMaterial.SetVector(CenterId, center);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float power = 1f - t;
            float currentStrength = strength * power;

            distortionMaterial.SetFloat(StrengthId, currentStrength);

            yield return null;
        }

        ResetMaterial(distortionMaterial);
        activeCoroutine = null;
        activeMaterial = null;
    }

    private static void ResetMaterial(Material material)
    {
        if (material == null)
            return;

        material.SetFloat(StrengthId, 0f);
    }
}
