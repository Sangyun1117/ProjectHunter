using UnityEngine;
using System.Collections;

public class CharacterAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private Character ownerCharacter;
    [SerializeField]
    private Character targetCharacter;

    private SpriteRenderer spriteRenderer;

    private Color originColor;

    [Header("Move Settings")]
    [SerializeField]
    private float moveRatio = 0.5f;
    [SerializeField]
    private float moveDuration = 0.15f;

    private Vector3 originPosition;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        originColor = spriteRenderer.color;
        originPosition = ownerCharacter.transform.position;
    }

    public void OnAnimationEnd()
    {
        ownerCharacter.animationFinished = true;
        ownerCharacter.ChangeState(BattleAnimState.Idle);
    }

    public void StartHitFlash()
    {
        spriteRenderer.color = new Color(1f, 0.5f, 0.5f);
    }
    public void EndHitFlash()
    {
        spriteRenderer.color = originColor;
    }

    public void PlayTargetHitAnimation()
    {
        targetCharacter.ChangeState(BattleAnimState.Hit);
    }

    public void EndTargetAnimation()
    {
        targetCharacter.animationFinished = true;
        targetCharacter.ChangeState(BattleAnimState.Idle);
    }

    // 공격 시 앞으로 이동
    public void MoveForward()
    {
        // 현재 위치에서 상대 위치 방향벡터 계산
        Vector3 direction = (targetCharacter.transform.position - ownerCharacter.transform.position).normalized;

        // 나와 상대 사이 실제 거리 계산
        float distance = Vector3.Distance(ownerCharacter.transform.position, targetCharacter.transform.position);

        // 어디까지 이동할지 계산 (moveRatio 비율만큼 이동)
        float moveDistance = distance * moveRatio;

        Vector3 targetPos = originPosition + direction * moveDistance;

        StartMove(targetPos);
        Debug.Log($"Moving forward to target position: {targetPos}");
    }

    // 원래 위치로 복귀
    public void MoveBack()
    {
        StartMove(originPosition);
        Debug.Log($"Moving back to original position: {originPosition}");
    }

    private void StartMove(Vector3 targetPos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(SmoothMove(targetPos));
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        Vector3 startPos = ownerCharacter.transform.position;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;

            ownerCharacter.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        ownerCharacter.transform.position = targetPos;

        moveCoroutine = null;
    }

}