using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    CharacterStats stats;

    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    public Slider hpBar;
    [SerializeField]
    public Slider mpBar;
    [SerializeField]
    public TextMeshProUGUI hpBarText;
    [SerializeField]
    public TextMeshProUGUI mpBarText;

    public void Initialize(CharacterStats stats)
    {
        this.stats = stats;

        nameText.text = stats.gameObject.name;

        stats.OnHpChanged += UpdateHP;
        stats.OnMpChanged += UpdateMP;

        UpdateHP();
        UpdateMP();
    }

    void UpdateHP()
    {
        hpBar.value = stats.currentHp / stats.maxHp;
        hpBarText.text = $"{stats.currentHp}/{stats.maxHp}";
    }

    void UpdateMP()
    {
        mpBar.value = stats.currentMp / stats.maxMp;
        mpBarText.text = $"{stats.currentMp}/{stats.maxMp}";
    }
}
