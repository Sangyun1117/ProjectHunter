using UnityEngine;

public enum HexType
{
    Empty,
    Start,
    Battle,
    Elite,
    Rest,
    Shop,
    Treasure,
    Boss
}

public class HexTile : MonoBehaviour
{
    [Header("Coordinates")]
    public Vector2Int axialCoordinate; // (q, r) 좌표

    [Header("Tile Info")]
    public HexType hexType;
    public bool isExplored = false;

    SpriteRenderer sr;
    MeshRenderer mr;

    private HexTileIcon icon;

    public int visitCount = 0;
    public void Awake()
    {
        //sr = GetComponentInChildren<SpriteRenderer>();
        mr = GetComponentInChildren<MeshRenderer>();
        icon = GetComponentInChildren<HexTileIcon>();
    }
    public void Initialize(Vector2Int coord, HexType type)
    {
        axialCoordinate = coord;

        SetHexType(type);
    }

    public void SetName()
    {
        gameObject.name = $"Hex_{axialCoordinate.x}_{axialCoordinate.y} ({hexType})";
    }

    public void SetHexType(HexType type)
    {
        hexType = type;
        switch (hexType)
        {
            case HexType.Start:
                mr.materials[1].SetColor("_BaseColor", new Color32(118, 255, 95, 255));
                //sr.color = Color.red;
                break;
            case HexType.Battle:
                mr.materials[1].SetColor("_BaseColor", new Color32(246, 41, 57, 255));
                break;
            //case HexType.Elite:
            //    sr.color = new Color(0.5f, 0f, 0f); // Dark Red
            //    break;
            //case HexType.Rest:
            //    sr.color = Color.green;
            //    break;
            //case HexType.Shop:
            //    sr.color = Color.blue;
            //    break;
            //case HexType.Treasure:
            //    sr.color = Color.yellow;
            //    break;
            case HexType.Boss:
                mr.materials[1].SetColor("_BaseColor", new Color32(159, 32, 138, 255)); // Purple
                break;
            default:
                mr.materials[1].SetColor("_BaseColor", new Color32(255, 169, 1, 255));
                break;
        }
        icon.SetTileIcon(hexType);
        SetName();
    }
}
