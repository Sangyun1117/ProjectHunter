using UnityEngine;
using UnityEngine.UIElements;

public class HexTileIcon : MonoBehaviour
{
    // 각 타일마다 인스펙터에서 다르게 지정할 PNG 아이콘
    [SerializeField] private Texture2D startIcon;
    [SerializeField] private Texture2D bossIcon;
    [SerializeField] private Texture2D battleIcon;

    // 모든 타일이 공유해서 사용할 정적(static) 프로퍼티 블록
    private static MaterialPropertyBlock _mpb;

    private Renderer myRenderer;

    private void Awake()
    {
        // 블록이 비어있다면 최초 1회만 생성
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        myRenderer = GetComponent<Renderer>();
    }

    public void SetTileIcon(HexType hexType)
    {
        if (myRenderer == null) return;
        // 이 타일 '자신'이 기존에 가지고 있던 프로퍼티 블록 정보를 공유 작업대로 긁어옵니다.
        // 만약 처음이라면 빈 값이 들어오고, 이미 세팅된 게 있다면 그 값이 들어옵니다.
        myRenderer.GetPropertyBlock(_mpb);

        switch (hexType)
        {
            case HexType.Start:
                _mpb.SetTexture("_BaseMap", startIcon);
                _mpb.SetColor("_BaseColor", new Color(0f, 0f, 0f, 1f));
                break;
            case HexType.Boss:
                _mpb.SetTexture("_BaseMap", bossIcon);
                _mpb.SetColor("_BaseColor", new Color(0f, 0f, 0f, 1f));
                break;
            case HexType.Battle:
                _mpb.SetTexture("_BaseMap", battleIcon);
                _mpb.SetColor("_BaseColor", new Color(0f, 0f, 0f, 1f));
                break;
            default:
                //_mpb.SetTexture("_BaseMap", null); // 텍스처를 null로 설정하면 오류 생김. 투명 처리로 대체.
                _mpb.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0f));
                break;
        }

        //수정된 블록을 다시 렌더러에 먹임
        myRenderer.SetPropertyBlock(_mpb);
    }
}
