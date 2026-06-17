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

    public override void ChangeHP(float amount)
    {
        base.ChangeHP(amount);

        // HP가 일정 이하로 떨어지면 화면 가장자리가 붉게 깜빡임
        if (Stats.currentHp / Stats.maxHp < 0.5f)
        {
            BattleManager.Instance.StartVignetteBlink();
        }
        else
        {
            BattleManager.Instance.StopVignetteBlink();
        }
    }
}
