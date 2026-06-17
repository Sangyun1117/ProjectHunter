using UnityEngine;
using System.Collections;

public class HitReactionEffect : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float shakeDuration = 0.12f;
    [SerializeField] float shakeAmplitude = 0.4f;

    [Header("Radial Distortion")]
    [SerializeField] Material distortionMaterial;
    [SerializeField] float distortionDuration = 0.15f;
    [SerializeField] float distortionStrength = 0.01f;
    [SerializeField] float distortionRadius = 0.45f;

    Coroutine shakeCoroutine;
    Vector3 originalCameraLocalPosition;

    void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            originalCameraLocalPosition = cameraTransform.localPosition;

        //ResetDistortion();
    }

    public void PlayHitReaction()
    {
        PlayCameraShake();
        PlayRadialDistortion();
    }
    void PlayCameraShake()
    {
        if (cameraTransform == null)
            return;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    void PlayRadialDistortion()
    {
        // material parameter animation
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / shakeDuration);
            float power = 1f - t;

            Vector2 randomOffset = Random.insideUnitCircle * shakeAmplitude * power;
            cameraTransform.localPosition = originalCameraLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            yield return null;
        }

        cameraTransform.localPosition = originalCameraLocalPosition;
        shakeCoroutine = null;
    }
}
