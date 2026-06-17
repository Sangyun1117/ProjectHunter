using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skills")]
public class SkillData : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private SkillType skillType;
    [SerializeField] private BattleAnimState animState;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<SkillEffectEntry> effects;
    [SerializeField] private List<SkillFeedback> castFeedbacks;
    [SerializeField] private List<SkillFeedback> hitFeedbacks;

    [SerializeField] private float mpCost;
    [SerializeField] private GameObject casterEffectPrefab;
    [SerializeField] private GameObject targetEffectPrefab;

    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public BattleAnimState AnimState => animState;
    public Sprite Icon => icon;
    public List<SkillEffectEntry> Effects => effects;
    public List<SkillFeedback> CastFeedbacks => castFeedbacks;
    public List<SkillFeedback> HitFeedbacks => hitFeedbacks;

    public float MpCost => mpCost;
    public GameObject CasterEffectPrefab => casterEffectPrefab;
    public GameObject TargetEffectPrefab => targetEffectPrefab;
}
