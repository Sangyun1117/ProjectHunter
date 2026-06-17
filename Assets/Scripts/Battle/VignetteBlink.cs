using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP를 사용하는 경우 필요

public class VignetteBlink : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume globalVolume;

    [Header("Blink Settings")]
    public float maxIntensity = 0.3f;
    public float duration = 2.0f; // 한 번 커졌다가 작아지는 데 걸리는 총 시간

    private Vignette vignette;
    private bool isBlinking = false;

    void Start()
    {
        // Global Volume에서 Vignette 프로파일을 가져옵니다.
        if (globalVolume != null && globalVolume.profile.TryGet(out vignette))
        {
            // 처음에 vignette을 활성화하고 초기값 세팅
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
        }
        else
        {
            Debug.LogError("Global Volume 또는 Vignette 프로필을 찾을 수 없습니다!");
        }
    }

    // 테스트용: 스페이스바를 누르면 반복 연출 시작/종료
    //void Update()
    //{
    //    if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        if (!isBlinking)
    //        {
    //            StartBlinking();
    //        }
    //        else
    //        {
    //            StopBlinking();
    //        }
    //    }
    //}

    // 효과 시작 함수
    public void StartBlinking()
    {
        if (isBlinking == false && vignette != null)
        {
            isBlinking = true;
            StartCoroutine(BlinkRoutine());
        }
    }

    // 효과 중지 함수
    public void StopBlinking()
    {
        isBlinking = false;
        StopAllCoroutines();
        if (vignette != null) vignette.intensity.value = 0f; // 원래대로 되돌림
    }

    // 무한 반복하는 코루틴 함수
    private IEnumerator BlinkRoutine()
    {
        float elapsedTime = 0f;

        while (isBlinking == true)
        {
            elapsedTime += Time.deltaTime;

            // 1. (elapsedTime / duration)에 의해 0에서 1까지 증가합니다.
            // 2. 거기에 Mathf.PI를 곱해 Sin이 0 -> 1 -> 0 순으로 움직이게 합니다.
            // 3. Mathf.Abs(절대값)를 씌워 elapsedTime이 커져도 음수로 떨어지지 않고 
            //    계속 0 -> 0.5 -> 0 -> 0.5를 반복하게 만듭니다.
            float currentT = Mathf.Abs(Mathf.Sin((elapsedTime / duration) * Mathf.PI));

            vignette.intensity.value = Mathf.Lerp(0f, maxIntensity, currentT);

            yield return null;
        }
    }
}