using UnityEngine;

public enum SkillFeedbackType
{
    CameraShake,
    RadialDistortion,
    HitStop,
    Sound,
    Vfx,
    Flash
}

[System.Serializable]
public abstract class SkillFeedback : ScriptableObject
{
    public abstract void Play();
}
