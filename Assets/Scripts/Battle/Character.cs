using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected CharacterStats stats;
    protected SkillBooks skillBooks;
    public CharacterStats Stats => stats;
    public SkillBooks SkillBooks => skillBooks;

    protected Animator animator;

    public bool animationFinished = false;

    static readonly int AnimStateHash = Animator.StringToHash("AnimState"); // 캐릭터의 애니메이션 상태를 나타내는 파라미터의 해시값

    protected virtual void Awake()
    {
        stats = GetComponent<CharacterStats>();
        skillBooks = GetComponent<SkillBooks>();
        Initialize();
    }

    public virtual void Initialize()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Attack();

    public void ChangeHP(float amount)
    {
        stats.ChangeHP(amount);
    }
    public void ChangeMP(float amount)
    {
        stats.ChangeMP(amount);
    }

    // BattleAnimState에 따라 애니메이션 상태를 변경하는 함수
    public void ChangeState(BattleAnimState state)
    {
        animator.SetInteger(AnimStateHash, (int)state);
        Debug.Log($"Changed animation state to {state}");
    }
}
