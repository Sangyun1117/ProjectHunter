using System;
using UnityEngine;

[System.Serializable]
public class CharacterStats : MonoBehaviour
{
    [Header("HP")]
    public float maxHp;
    public float currentHp;

    [Header("MP")]
    public float maxMp;
    public float currentMp;

    [Header("Combat")]
    public float attack;
    public float defense;
    public float speed;
    public float abilityPower;
    public float magicDefense;

    public event Action OnHpChanged;
    public event Action OnMpChanged;

    public void Initialize(MonsterData monsterData)
    {
        maxHp = monsterData.maxHp;
        currentHp = maxHp;

        maxMp = monsterData.maxMp;
        currentMp = maxMp;

        attack = monsterData.attack;
        defense = monsterData.defense;
        speed = monsterData.speed;
        abilityPower = monsterData.abilityPower;
    }

    public void Initialize(ClassData classData)
    {
        maxHp = classData.maxHp;
        currentHp = maxHp;

        maxMp = classData.maxMp;
        currentMp = maxMp;

        attack = classData.attack;
        defense = classData.defense;
        speed = classData.speed;
        abilityPower = classData.abilityPower;
    }

    public void ChangeHP(float amount)
    {
        currentHp += amount;

        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHpChanged?.Invoke();

    }

    public void ChangeMP(float amount)
    {
        currentMp += amount;
        currentMp = Mathf.Clamp(currentMp, 0, maxMp);
        OnMpChanged?.Invoke();
    }

    public void Dead()
    {
        Debug.Log($"{gameObject.name} is dead!");

        SceneTransitionManager.Instance.ReturnToMapScene();
    }
}