using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    BattlePhase battlePhase;
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

    [SerializeField] private BattleOverlay battleOverlay;

    [SerializeField] private SkillData nowPhaseSkill;
    public SkillData NowPhaseSkill => nowPhaseSkill;

    private VignetteBlink vignetteBlink;

    private void Awake()
    {
        Instance = this;
        vignetteBlink = GetComponent<VignetteBlink>();
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

        yield return StartCoroutine(battleOverlay.PlayOverlayPanelAnimation(battlePhase));

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

        // 행동 순서 결정
        Character first, second;
        SkillData firstSkill, secondSkill;

        if (player.Stats.speed >= enemy.Stats.speed)
        {
            first = player;
            second = enemy;
            firstSkill = playerSkill;
            secondSkill = enemySkill;
        }
        else
        {
            first = enemy;
            second = player;
            firstSkill = enemySkill;
            secondSkill = playerSkill;
        }

        // 스킬 취소 체크
        bool firstCanceled = IsCanceled(firstSkill.SkillType, secondSkill.SkillType);
        bool secondCanceled = IsCanceled(secondSkill.SkillType, firstSkill.SkillType);

        // 스킬 사용
        if (firstCanceled == false)
        {
            yield return StartCoroutine(UseSkill(firstSkill, first, second));
            yield return new WaitForSeconds(0.5f);
            nowPhaseSkill = null;
        }

        ///yield return null;
        // 사망 체크
        //if (player.IsDead || enemy.IsDead)
        //    yield break;
        if (secondCanceled == false)
        {
            yield return StartCoroutine(UseSkill(secondSkill, second, first));
            nowPhaseSkill = null;
        }
        SetBattle(BattlePhase.Select);
    }

    public IEnumerator UseSkill(SkillData skill, Character caster, Character target)
    {
        nowPhaseSkill = skill;
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
        caster.ChangeState(skill.AnimState);
        //caster.ChangeState(BattleAnimState.Attack);
        //target.ChangeState(BattleAnimState.Hit);

        yield return new WaitUntil(() => caster.animationFinished);
        yield return new WaitUntil(() => target.animationFinished);
    }

    private bool IsCanceled(SkillType attackerType, SkillType defenderType)
    {
        return
            (attackerType == SkillType.Concentrate && defenderType == SkillType.Interrupt) ||
            (attackerType == SkillType.Interrupt && defenderType == SkillType.Attack) ||
            ((attackerType == SkillType.Interrupt || attackerType == SkillType.Attack) && defenderType == SkillType.Ultimate) ||
            (attackerType == SkillType.Ultimate && defenderType == SkillType.Concentrate);
    }

    public IEnumerator EndBattle()
    {
        // 전투 종료 시 비네트 깜빡임 중지
        StopVignetteBlink();
        // 승리/패배 처리
        if (player.IsDead)
        {
            // 패배 처리 로직 추가
            Debug.Log("Player is defeated!");
            battlePhase = BattlePhase.Lose;
            yield return StartCoroutine(battleOverlay.PlayOverlayPanelAnimation(battlePhase));
        }
        else if (enemy.IsDead)
        {
            // 승리 처리 로직 추가
            Debug.Log("Enemy is defeated!");
            battlePhase = BattlePhase.Win;
            yield return StartCoroutine(battleOverlay.PlayOverlayPanelAnimation(battlePhase));
        }
        // 씬 전환 등 추가 처리
        SceneTransitionManager.Instance?.ReturnToMapScene();
    }

    public void StartVignetteBlink()
    {
        if (vignetteBlink == null)
        {
            return;
        }
        vignetteBlink.StartBlinking();
    }

    public void StopVignetteBlink()
    {
        if (vignetteBlink == null)
        {
            return;
        }
        vignetteBlink.StopBlinking();
    }
}
