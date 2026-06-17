using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Data/SkillFeedback/HitStop")]
public class HitStop : SkillFeedback
{
    [SerializeField] private float duration = 0.08f; // 히트스탑 지속 시간 (초)
    [SerializeField, Range(0f, 1f)] private float timeScale = 0.1f; // 히트스탑 동안의 시간 배율 (0이면 완전 정지)

    static Coroutine activeCoroutine;
    public override void Play()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogWarning("[HitStopFeedback] BattleManager.Instance is missing.");
            return;
        }

        if (activeCoroutine != null)
        {
            BattleManager.Instance.StopCoroutine(activeCoroutine);
            Time.timeScale = 1f;
        }

        activeCoroutine = BattleManager.Instance.StartCoroutine(HitStopRoutine());
    }

    IEnumerator HitStopRoutine()
    {
        float previousTimeScale = Time.timeScale; // 현재 시간 배율 저장
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration); // 히트스탑 지속 시간 동안 대기

        Time.timeScale = previousTimeScale; // 이전 시간 배율로 복원
        activeCoroutine = null;
    }
}
