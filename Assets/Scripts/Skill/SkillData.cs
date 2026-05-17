using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skills")]
public class SkillData : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private SkillType skillType;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<SkillEffectEntry> effects;

    [SerializeField] private float mpCost;


    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public Sprite Icon => icon;
    public List<SkillEffectEntry> Effects => effects;
    public float MpCost => mpCost;
}
