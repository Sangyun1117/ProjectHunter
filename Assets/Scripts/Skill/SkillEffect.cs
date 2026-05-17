using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Apply(SkillEffectParams skillParams, Character player, Character target);
}
