using UnityEngine;
using System.Collections;

public class BattleOverlay : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winResultPanel;
    [SerializeField] private GameObject loseResultPanel;
    [SerializeField] private GameObject playerSkillFrame;
    [SerializeField] private GameObject enemySkillFrame;

    public IEnumerator PlayOverlayPanelAnimation(BattlePhase battlePhase)
    {
        switch (battlePhase)
        {
            case BattlePhase.Start:
                startPanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                startPanel.SetActive(false);
                break;
            case BattlePhase.Win:
                winResultPanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                winResultPanel.SetActive(false);
                break;
            case BattlePhase.Lose:
                loseResultPanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                loseResultPanel.SetActive(false);
                break;
        }
    }

    public IEnumerator PlaySkillFrameAnimation(SkillType playerSkillType, SkillType enemySkillType)
    {
        playerSkillFrame.SetActive(true);
        enemySkillFrame.SetActive(true);
        Coroutine playerAnim = StartCoroutine(playerSkillFrame.GetComponent<SkillFrame>().ShowSkillFrame(playerSkillType));
        Coroutine enemyAnim = StartCoroutine(enemySkillFrame.GetComponent<SkillFrame>().ShowSkillFrame(enemySkillType));

        // 2. 두 애니메이션이 모두 끝날 때까지 대기
        yield return playerAnim;
        yield return enemyAnim;
        playerSkillFrame.SetActive(false);
        enemySkillFrame.SetActive(false);
    }
}
