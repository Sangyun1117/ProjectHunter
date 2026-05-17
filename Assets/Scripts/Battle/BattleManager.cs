using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    BattlePhase battlePhase;
    [SerializeField]
    Player player;
    [SerializeField]
    Enemy enemy;

    [SerializeField]
    private BattleOverlay battleOverlay;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        battlePhase = BattlePhase.Start;

        player.Initialize();
        enemy.Initialize();

        yield return StartCoroutine(battleOverlay.PlayStartAnimation());

        SetBattle(BattlePhase.Select);
    }

    public BattlePhase GetBattlePhase()
    {
        return battlePhase;
    }

    public void SetBattle(BattlePhase newPhase)
    {
        battlePhase = newPhase;
    }

    public IEnumerator ExecuteTurn(int playerSkillIndex)
    {
        Debug.Log($"[BattleManager] ExecuteTurn called with playerSkillIndex: {playerSkillIndex}");

        int enemySkillIndex = enemy.ChooseSkill();
        if (enemySkillIndex == -1)
        {
            Debug.LogWarning("[BattleManager] Enemy has no skills to use.");
            yield break;
        }
        SkillData playerSkill = player.SkillBooks.GetEquippedSkill(playerSkillIndex);
        SkillData enemySkill = enemy.SkillBooks.GetEquippedSkill(enemySkillIndex);

        SetBattle(BattlePhase.Action);
        // 턴 시작 스킬 타입 애니메이션
        yield return StartCoroutine(battleOverlay.PlaySkillFrameAnimation(playerSkill.SkillType, enemySkill.SkillType));

        Character first, second;
        SkillData firstSkill, secondSkill;

        first = player;
        second = enemy;

        firstSkill = playerSkill;
        secondSkill = enemySkill;

        // 스킬 사용
        yield return StartCoroutine(UseSkill(firstSkill, first, second));

        yield return null;
        yield return new WaitForSeconds(0.5f);
        // 사망 체크
        //if (player.IsDead || enemy.IsDead)
        //    yield break;
        yield return StartCoroutine(UseSkill(secondSkill, second, first));
        
        SetBattle(BattlePhase.Select);
    }

    public IEnumerator UseSkill(SkillData skill, Character caster, Character target)
    {
       // SkillData skill = caster.SkillBooks.GetEquippedSkill(skillIndex);

        caster.ChangeMP(-skill.MpCost);

        foreach (SkillEffectEntry effectEntry in skill.Effects)
        {
            if (effectEntry.Effect == null)
            {
                Debug.LogWarning($"[BattleManager] {skill.SkillName}의 이펙트가 null입니다. 인스펙터에서 Effect 에셋을 확인하세요.");
                continue;
            }
            effectEntry.Effect.Apply(effectEntry.Param, caster, target);
        }

        caster.animationFinished = false;
        target.animationFinished = false;
        caster.ChangeState(BattleAnimState.Attack);
        //target.ChangeState(BattleAnimState.Hit);

        yield return new WaitUntil(() => caster.animationFinished);
        yield return new WaitUntil(() => target.animationFinished);
    }
}
