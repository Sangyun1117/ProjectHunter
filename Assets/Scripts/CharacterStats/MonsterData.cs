using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster Data")]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    public MonsterType monsterType;
    [SerializeField]
    public RuntimeAnimatorController animatorController;
    [SerializeField]
    public float maxHp;
    [SerializeField]
    public float maxMp;
    [SerializeField]
    public float attack;
    [SerializeField]
    public float defense;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float abilityPower;
}