using UnityEngine;

[System.Serializable]
public class SkillEffectEntry
{
    [SerializeField] private SkillEffect effect;      // 인스펙터에서 에셋 드래그앤드롭
    [SerializeField] private SkillEffectParams param; // 인스펙터에서 배율 직접 편집

    public SkillEffect Effect => effect;
    public SkillEffectParams Param => param;
}