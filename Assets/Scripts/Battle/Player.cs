using UnityEngine;

public class Player : Character
{
    [SerializeField]
    public ClassType classType;

    private ClassData classData;

    [SerializeField]
    private BattleHUD playerHUD;

    public override void Initialize()
    {
        base.Initialize();
        classData = Resources.Load<ClassData>($"Data/Classes/{classType}");

        stats.Initialize(classData);
        playerHUD.Initialize(stats);

        animator.runtimeAnimatorController = classData.animatorController;        
    }

    public void UseSkill(int skillIndex)
    {
        Debug.Log($"Player uses skill {skillIndex}!");
    }

    public override void Attack()
    {
        Debug.Log("Player attacks!");

    }
}
