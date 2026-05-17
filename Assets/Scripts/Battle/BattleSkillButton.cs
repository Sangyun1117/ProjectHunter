using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleSkillButton : MonoBehaviour
{
    [SerializeField] private Player player;
    private SkillBooks skillBooks;

    [SerializeField] private Button[] skillButtons = new Button[4];

    public void Start()
    {
        skillBooks = player.GetComponentInChildren<SkillBooks>();
        SetSkills();

        player.Stats.OnMpChanged += RefreshSkillButtons;
        RefreshSkillButtons();
    }

    public void SetSkills()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillBooks.GetEquippedSkill(i) == null || skillButtons == null)
            {
                return;
            }

            // 스킬 이름 세로로 표시, 공백은 작은 크기로 처리
            string verticalText = string.Join("\n", skillBooks.GetEquippedSkill(i).SkillName
                .ToCharArray()
                .Select(c => c == ' ' ? "<size=30%> </size>" : c.ToString()));
            skillButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = verticalText;

        }
    }
    public void OnClickSkill(int skillIndex)
    {
        if (BattleManager.Instance.GetBattlePhase() != BattlePhase.Select)
        {
            Debug.Log("스킬을 사용할 수 있는 단계가 아닙니다.");
            return;
        }
        if (player.Stats.currentMp < skillBooks.GetEquippedSkill(skillIndex).MpCost)
        {
            Debug.Log("MP가 부족하여 스킬을 사용할 수 없습니다.");
            return;
        }
        Debug.Log($"스킬 {skillBooks.GetEquippedSkill(skillIndex).SkillName} 사용!");

        StartCoroutine(BattleManager.Instance.ExecuteTurn(skillIndex));
    }

    private void RefreshSkillButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            SkillData skill = skillBooks.GetEquippedSkill(i);
            if (skill == null)
            {
                continue;
            }
            skillButtons[i].interactable = player.Stats.currentMp >= skill.MpCost;
        }
    }
}
