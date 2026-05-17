using UnityEngine;

[CreateAssetMenu(menuName = "Data/SkillEffects/Damage")]
public class DamageEffect : SkillEffect
{
    public override void Apply(SkillEffectParams skillParams,Character player, Character target)
    {
        float attackPart = Mathf.Max(0, player.Stats.attack - target.Stats.defense) * skillParams.AttackMultiplier;

        float magicPart = Mathf.Max(0, player.Stats.abilityPower - target.Stats.magicDefense) * skillParams.AbilityPowerMultiplier;

        float hpPart = player.Stats.maxHp * skillParams.HpMultiplier;

        float mpPart = player.Stats.maxMp * skillParams.MpMultiplier;

        float defensePart = target.Stats.defense * skillParams.DefenseMultiplier;

        float magicDefensePart = target.Stats.magicDefense * skillParams.MagicDefenseMultiplier;

        float finalDamage = skillParams.BasePower + attackPart + magicPart + hpPart + mpPart + defensePart + magicDefensePart;

        finalDamage = Mathf.Max(finalDamage, 1);
        Debug.Log($"DamageEffect: BaseDamage={skillParams.BasePower}, AttackPart={attackPart}, MagicPart={magicPart}, HpPart={hpPart}, MpPart={mpPart}, DefensePart={defensePart}, MagicDefensePart={magicDefensePart}, FinalDamage={finalDamage}");

        target.ChangeHP(-finalDamage);
    }
}
