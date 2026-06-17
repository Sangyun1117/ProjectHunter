using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Data/SkillFeedback/CameraShake")]
public class CameraShake : SkillFeedback
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float magnitude = 0.1f;

    static Coroutine activeCoroutine;
    static Transform activeCameraTransform;
    static Vector3 activeOriginalLocalPosition;

    public override void Play()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogWarning("[CameraShakeFeedback] BattleManager.Instance is missing.");
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[CameraShakeFeedback] MainCamera is missing.");
            return;
        }

        if (activeCoroutine != null)
        {
            BattleManager.Instance.StopCoroutine(activeCoroutine);

            if (activeCameraTransform != null)
                activeCameraTransform.localPosition = activeOriginalLocalPosition;
        }

        activeCameraTransform = mainCamera.transform;
        activeOriginalLocalPosition = activeCameraTransform.localPosition;
        activeCoroutine = BattleManager.Instance.StartCoroutine(ShakeRoutine(activeCameraTransform, activeOriginalLocalPosition));
    }

    IEnumerator ShakeRoutine(Transform cameraTransform, Vector3 originalLocalPosition)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float power = 1f - t;

            Vector2 randomOffset = Random.insideUnitCircle * magnitude * power;
            cameraTransform.localPosition = originalLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            yield return null;
        }

        cameraTransform.localPosition = originalLocalPosition;
        activeCoroutine = null;
        activeCameraTransform = null;
    }
}
