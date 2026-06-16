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
        if (state == BattleAnimState.Hit)
        {
            //Instantiate(hitEffectPrefab, effectPos, Quaternion.identity);
            if (BattleManager.Instance.NowPhaseSkill.TargetEffectPrefab == null)
            {
                return;
            }

            GameObject effect = Instantiate(BattleManager.Instance.NowPhaseSkill.TargetEffectPrefab);

            if (effect.TryGetComponent(out VFXInfo vfxInfo) == true)
            {
                if(this is Player)
                {
                    vfxInfo.SpawnOnPlayer(transform);
                }
                else if (this is Enemy)
                {
                    vfxInfo.SpawnOnEnemy(transform);
                }
            }

            //Vector3 effectPos = transform.position;
            //Instantiate(BattleManager.Instance.NowPhaseSkill.TargetEffectPrefab, effectPos, Quaternion.identity);
        }
        else if (state == BattleAnimState.Attack || state == BattleAnimState.Concentrate || state == BattleAnimState.Interrupt)
        {
            if (BattleManager.Instance.NowPhaseSkill.CasterEffectPrefab == null)
            {
                return;
            }

            GameObject effect = Instantiate(BattleManager.Instance.NowPhaseSkill.CasterEffectPrefab);

            if (effect.TryGetComponent(out VFXInfo vfxInfo) == true)
            {
                if (this is Player)
                {
                    vfxInfo.SpawnOnPlayer(transform);
                }
                else if (this is Enemy)
                {
                    vfxInfo.SpawnOnEnemy(transform);
                }
            }
        }
    }

    public bool IsDead => stats.currentHp <= 0;
}
