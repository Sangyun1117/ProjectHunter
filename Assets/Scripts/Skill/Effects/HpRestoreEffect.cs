using UnityEngine;

[CreateAssetMenu(menuName = "Data/SkillEffects/HpRestore")]
public class HpRestoreEffect : SkillEffect
{
    public override void Apply(SkillEffectParams skillParams, Character player, Character target)
    {
        float attackPart = player.Stats.attack * skillParams.AttackMultiplier;

        float magicPart = player.Stats.abilityPower * skillParams.AbilityPowerMultiplier;

        float hpPart = player.Stats.maxHp * skillParams.HpMultiplier;

        float mpPart = player.Stats.maxMp * skillParams.MpMultiplier;

        float defensePart = target.Stats.defense * skillParams.DefenseMultiplier;

        float magicDefensePart = target.Stats.magicDefense * skillParams.MagicDefenseMultiplier;

        float finalHeal = skillParams.BasePower + attackPart + magicPart + hpPart + mpPart + defensePart + magicDefensePart;

        finalHeal = Mathf.Max(finalHeal, 1);
        Debug.Log($"DamageEffect: BasePower={skillParams.BasePower}, AttackPart={attackPart}, MagicPart={magicPart}, HpPart={hpPart}, MpPart={mpPart}, DefensePart={defensePart}, MagicDefensePart={magicDefensePart}, FinalDamage={finalHeal}");

        player.ChangeHP(finalHeal);
    }
}
