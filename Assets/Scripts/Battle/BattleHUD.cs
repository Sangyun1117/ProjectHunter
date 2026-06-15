using System.Collections;
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
    public Slider hpDelayedBar;
    [SerializeField] 
    public Slider mpDelayedBar;
    [SerializeField]
    public TextMeshProUGUI hpBarText;
    [SerializeField]
    public TextMeshProUGUI mpBarText;

    private float targetHp;
    private float targetMp;



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
        targetHp = stats.currentHp / stats.maxHp;
        hpBarText.text = $"{stats.currentHp}/{stats.maxHp}";

        StartCoroutine(SmoothHPChange());
        
    }

    void UpdateMP()
    {
        targetMp = stats.currentMp / stats.maxMp;
        mpBarText.text = $"{stats.currentMp}/{stats.maxMp}";

        StartCoroutine(SmoothMPChange());
    }

    IEnumerator SmoothHPChange()
    {
        hpBar.value = targetHp;

        yield return new WaitForSeconds(0.3f); // 딜레이 후에 hpDelayedBar가 hpBar를 따라오도록 함

        // 뒤 체력바 천천히 감소
        while (Mathf.Abs(hpDelayedBar.value - targetHp) > 0.001f)
        {
            hpDelayedBar.value = Mathf.Lerp(
                hpDelayedBar.value,
                targetHp,
                Time.deltaTime * 4f
            );

            yield return null;
        }

        hpDelayedBar.value = targetHp;

        if(hpDelayedBar.value <= 0.0f)
        {
            stats.Dead();
        }
    }

    IEnumerator SmoothMPChange()
    {
        mpBar.value = targetMp;
        yield return new WaitForSeconds(0.3f); // 딜레이 후에 mpDelayedBar가 mpBar를 따라오도록 함
        // 뒤 마나바 천천히 감소
        while (Mathf.Abs(mpDelayedBar.value - targetMp) > 0.001f)
        {
            mpDelayedBar.value = Mathf.Lerp(
                mpDelayedBar.value,
                targetMp,
                Time.deltaTime * 4f
            );
            yield return null;
        }
        mpDelayedBar.value = targetMp;
    }
}
