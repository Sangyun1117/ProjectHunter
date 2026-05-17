using System.Collections.Generic;
using UnityEngine;

public class SkillBooks : MonoBehaviour
{
    [SerializeField]
    private List<SkillData> learnedSkills;
     
    [SerializeField]
    private SkillData[] equippedSkills = new SkillData[4];

    public List<SkillData> LearnedSkills => learnedSkills;

    public SkillData GetEquippedSkill(int index)
    {
        return equippedSkills[index];
    }

    public int GetEquippedSkillsLength()
    {
        return equippedSkills.Length;
    }

    public void LearnSkill(SkillData skill)
    {
        if (!learnedSkills.Contains(skill))
        {
            learnedSkills.Add(skill);
        }
    }

    public void ChangeSkill(int skillIndex, SkillData skill)
    {
        if (skillIndex < 0 || skillIndex >= equippedSkills.Length)
        {
            return;
        }
        equippedSkills[skillIndex] = skill;
    }
}
