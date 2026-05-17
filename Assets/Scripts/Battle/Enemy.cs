using UnityEngine;

public class Enemy : Character
{

    [SerializeField]
    public MonsterType monsterType;

    private MonsterData monsterData;

    [SerializeField]
    private BattleHUD enemyHUD;

    public override void Initialize()
    {
        base.Initialize();
        monsterData = Resources.Load<MonsterData>($"Data/Monsters/{monsterType}");

        stats.Initialize(monsterData);
        enemyHUD.Initialize(stats);

        animator.runtimeAnimatorController = monsterData.animatorController;
    }

    public override void Attack()
    {
        // Implement enemy attack behavior
    }

    public int ChooseSkill()
    {
        if (skillBooks.GetEquippedSkillsLength() == 0)
        {
            Debug.LogWarning($"[Enemy] {monsterType} has no skills defined.");
            return -1; // No skill available
        }
        int randomIndex = Random.Range(0, skillBooks.GetEquippedSkillsLength());
        return randomIndex;
    }
}
