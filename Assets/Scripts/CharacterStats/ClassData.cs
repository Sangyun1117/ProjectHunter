using UnityEngine;

[CreateAssetMenu(menuName = "Data/Class Data")]
public class ClassData : ScriptableObject
{
    [SerializeField]
    public ClassType classType;
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