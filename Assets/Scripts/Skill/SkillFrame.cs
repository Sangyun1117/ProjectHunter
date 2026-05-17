using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SkillFrame : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f; // 애니메이션 지속 시간
    [SerializeField] private float startScale = 0.5f; // 시작 크기 (0.5배에서 시작)

    private Animator animator;
    private TextMeshProUGUI skillTypeText;
    [SerializeField] private Image skillTypeImage;
    private Coroutine appearanceRoutine;

    public bool animationFinished = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        skillTypeText = GetComponentInChildren<TextMeshProUGUI>();
        skillTypeText.gameObject.SetActive(false);
    }
    public IEnumerator ShowSkillFrame(SkillType type)
    {
        switch (type)
        {
            case SkillType.Concentrate:
                skillTypeText.text = "집중";
                skillTypeText.color = new Color32(214, 255, 232, 255);
                skillTypeImage.color = new Color32(31, 122, 72, 255);//초록색
                break;
            case SkillType.Interrupt:
                skillTypeText.text = "방해";
                skillTypeText.color = new Color32(214, 228, 255, 255);
                skillTypeImage.color = new Color32(31, 59, 122, 255);//파란색
                break;
            case SkillType.Attack:
                skillTypeText.text = "공격";
                skillTypeText.color = new Color32(255, 214, 214, 255);
                skillTypeImage.color = new Color32(145, 45, 45, 255); //붉은색
                break;
            case SkillType.Ultimate:
                skillTypeText.text = "필살";
                skillTypeText.color = new Color32(34, 34, 34, 255);
                skillTypeImage.color = new Color32(255, 255, 255, 255); //하얀색
                break;
        }
        animationFinished = false;
        yield return null; // 한 프레임 대기하여 활성화가 적용되도록 함
        animator.Play("Run", 0, 0f);
        yield return new WaitUntil(() => animationFinished);
    }

    public void ShowText()
    {
        // 이미 실행 중인 애니메이션이 있다면 중지
        if (appearanceRoutine != null) StopCoroutine(appearanceRoutine);

        // 애니메이션 시작
        appearanceRoutine = StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        skillTypeText.gameObject.SetActive(true);

        RectTransform rectTransform = skillTypeText.rectTransform;
        Color originalColor = skillTypeText.color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 1. 점점 커지는 효과 (Scale)
            float currentScale = Mathf.Lerp(startScale, 1f, progress);
            rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);

            // 2. 서서히 보이는 효과 (Alpha)
            float currentAlpha = Mathf.Lerp(0f, 1f, progress);
            skillTypeText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

            yield return null;
        }

        // 마지막 상태 확정
        rectTransform.localScale = Vector3.one;
        skillTypeText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    // 애니메이션이 끝난 후 프레임을 숨기는 함수
    public void HideSkillFrame()
    {
        skillTypeText.gameObject.SetActive(false);
        animationFinished = true;
    }
}
