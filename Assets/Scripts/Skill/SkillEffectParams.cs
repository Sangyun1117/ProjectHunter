using UnityEngine;

[System.Serializable]
public class SkillEffectParams
{
    [SerializeField] private float basePower;
    [SerializeField] private float attackMultiplier;
    [SerializeField] private float abilityPowerMultiplier;
    [SerializeField] private float hpMultiplier;
    [SerializeField] private float mpMultiplier;
    [SerializeField] private float defenseMultiplier;
    [SerializeField] private float magicDefenseMultiplier;

    public float BasePower => basePower;
    public float AttackMultiplier => attackMultiplier;
    public float AbilityPowerMultiplier => abilityPowerMultiplier;
    public float HpMultiplier => hpMultiplier;
    public float MpMultiplier => mpMultiplier;
    public float DefenseMultiplier => defenseMultiplier;
    public float MagicDefenseMultiplier => magicDefenseMultiplier;
}