using UnityEngine;
using System.Collections;

public class SkillFrameAnim : MonoBehaviour
{
    [SerializeField] private SkillFrame skillFrame;
    public void ShowText()
    {
        skillFrame.ShowText();
    }

    // 애니메이션이 끝난 후 프레임을 숨기는 함수
    public void HideSkillFrame()
    {
        skillFrame.HideSkillFrame();
    }
}
