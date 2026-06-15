using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HexMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int tileCount = 30; // 타일 개수로 맵 생성
    [SerializeField] private int mapRadius = 5;
    [SerializeField] private float hexSize = 1f; // 이제 1.0을 넣으면 너비 1단위 타일에 딱 맞습니다.
    [SerializeField] private float spacing = 1.0f;
    [SerializeField] private bool isFlatTop = true;
    [SerializeField] private Transform mapContainer;

    [Header("Tiles")]
    [SerializeField] private float battleTileSpawnChance = 0.3f;

    [Header("Randomness")]
    [Range(0f, 1f)]
    [SerializeField] private float generateChance = 0.8f;

    [Header("Visuals")]
    [SerializeField] private float tileVisualScale = 1.0f; // 기본 스케일 1.0

    [Header("Prefabs")]
    [SerializeField] private GameObject hexTilePrefab;


    [SerializeField] private int startTolerance = 1; // 시작 타일로 간주할 최대 거리 허용치 (중심에서 가장 먼 타일과의 거리 차이)
    [SerializeField] private int bossTolerance = 1; // 보스 타일로 간주할 최대 거리 허용치 (시작 타일에서 가장 먼 타일과의 거리 차이)

    // 생성된 타일들을 관리하기 위한 딕셔너리 (좌표 기반 검색용)
    private Dictionary<Vector2Int, HexTile> mapTiles = new Dictionary<Vector2Int, HexTile>();

    [SerializeField] private Camera mainCamera;
    private Vector3 cameraOffset;

    private Vector2Int playerStartPos = Vector2Int.zero; // 플레이어의 현재 타일 좌표

    private MapManager mapManager;

    private void Start()
    {
        cameraOffset = mainCamera.transform.position; //카메라 초기 위치 저장
        GenerateMap();
    }

    private void Awake()
    {
        mapManager = GetComponent<MapManager>();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        //GenerateMapByMapRadius();
        GenerateMapByTileCount(tileCount);
    }

    private void GenerateMapByMapRadius()
    {
        ClearMap();

        Random.InitState((int)System.DateTime.Now.Ticks);

        for (int q = -mapRadius; q <= mapRadius; q++)
        {
            int r1 = Mathf.Max(-mapRadius, -q - mapRadius);
            int r2 = Mathf.Min(mapRadius, -q + mapRadius);

            for (int r = r1; r <= r2; r++)
            {
                // 확률에 따라 타일 생성 여부 결정
                if (Random.value <= generateChance)
                {
                    CreateTile(q, r);
                }
            }
        }
    }

    [ContextMenu("Generate Map By Tile Count")]
    private void GenerateMapByTileCount(int targetTileCount)
    {
        // 기존 맵 제거
        ClearMap();

        HashSet<Vector2Int> createdTiles = new(); //생성된 타일
        List<Vector2Int> frontier = new(); // 아직 주변으로 자라날 수 있는 타일

        // 중심 타일부터 시작
        Vector2Int center = Vector2Int.zero;

        createdTiles.Add(center);
        frontier.Add(center);

        CreateTile(center.x, center.y);

        Vector2Int[] directions =
        {
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1),
            new Vector2Int(0,1)
        };

        while (createdTiles.Count < targetTileCount) // 타일 개수가 목표에 도달할 때까지 반복
        {
            Vector2Int current = frontier[Random.Range(0, frontier.Count)]; // 무작위로 frontier에서 타일 선택

            // 선택된 타일의 6방향 이웃 중 아직 생성되지 않은 타일 후보 찾기
            List<Vector2Int> candidates = new();
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (!createdTiles.Contains(next))
                {
                    candidates.Add(next);
                }
            }

            // 후보가 없으면 frontier에서 제거하고 다음으로 넘어감
            if (candidates.Count == 0)
            {
                frontier.Remove(current);
                continue;
            }

            // 후보 중 무작위로 하나 선택
            Vector2Int chosen = candidates[Random.Range(0, candidates.Count)];

            createdTiles.Add(chosen);
            frontier.Add(chosen);

            // 타일 생성
            if (Random.value <= battleTileSpawnChance)
            {
                CreateTile(chosen.x, chosen.y, HexType.Battle);
            }
            else
            {
                CreateTile(chosen.x, chosen.y);
            }
        }

        // 타일 타입 지정
        SetTilesType();

        mapManager.InitializeMap(mapTiles, playerStartPos);
    }

    private void CreateTile(int q, int r, HexType hexType = HexType.Empty)
    {
        // hexSize의 절반을 바깥쪽 원의 반지름(Size)로 사용
        //float size = (hexSize * 0.5f) * tileVisualScale * spacing;
        Vector3 worldPos = HexToWorld(q, r);

        // 일관된 간격으로 생성
        GameObject go = Instantiate(hexTilePrefab, worldPos, Quaternion.identity, mapContainer);

        // 타일의 시각적 크기 설정
        go.transform.localScale = Vector3.one * tileVisualScale;

        HexTile tile = go.GetComponent<HexTile>();
        if (tile == null) tile = go.AddComponent<HexTile>();

        //HexType randomType = (HexType)Random.Range(1, System.Enum.GetValues(typeof(HexType)).Length);

        tile.Initialize(new Vector2Int(q, r), hexType);
        mapTiles.Add(new Vector2Int(q, r), tile);
    }

    // Hex 좌표를 월드 좌표로 변환
    public Vector3 HexToWorld(int q, int r)
    {
        float x, y;
        // hexSize의 절반을 바깥쪽 원의 반지름(Size)로 사용
        float size = (hexSize * 0.5f) * tileVisualScale * spacing;

        if (isFlatTop)
        {
            float horizontalSpacing = 1.5f * size;
            float verticalSpacing = Mathf.Sqrt(3) * size;

            x = horizontalSpacing * q;
            y = (verticalSpacing * 0.5f * q) + (verticalSpacing * r);
        }
        else
        {
            float horizontalSpacing = Mathf.Sqrt(3) * size;
            float verticalSpacing = 1.5f * size;

            x = (horizontalSpacing * q) + (horizontalSpacing * 0.5f * r);
            y = verticalSpacing * r;
        }

        return new Vector3(x, y, 0);
    }

    public Vector2Int WorldToHex(Vector3 worldPos)
    {
        float q, r;
        // hexSize의 절반을 바깥쪽 원의 반지름(Size)로 사용
        float size = (hexSize * 0.5f) * tileVisualScale * spacing;

        if (isFlatTop)
        {
            q = (2f / 3f * worldPos.x) / size;

            r = (
                (-1f / 3f * worldPos.x)
                + (Mathf.Sqrt(3f) / 3f * worldPos.y)
            ) / size;
        }
        else
        {
            q = (
                (Mathf.Sqrt(3f) / 3f * worldPos.x)
                - (1f / 3f * worldPos.y)
            ) / size;

            r = (2f / 3f * worldPos.y) / size;
        }

        return HexRound(q, r);
    }
    private Vector2Int HexRound(float q, float r)
    {
        float s = -q - r;

        int rq = Mathf.RoundToInt(q);
        int rr = Mathf.RoundToInt(r);
        int rs = Mathf.RoundToInt(s);

        float qDiff = Mathf.Abs(rq - q);
        float rDiff = Mathf.Abs(rr - r);
        float sDiff = Mathf.Abs(rs - s);

        if (qDiff > rDiff && qDiff > sDiff)
        {
            rq = -rr - rs;
        }
        else if (rDiff > sDiff)
        {
            rr = -rq - rs;
        }

        return new Vector2Int(rq, rr);
    }
    private void ClearMap()
    {
        if (mapContainer == null) mapContainer = this.transform;

        foreach (var tile in mapTiles.Values)
        {
            if (tile != null) DestroyImmediate(tile.gameObject);
        }
        mapTiles.Clear();

        // 컨테이너 하위의 남은 객체들도 정리
        for (int i = mapContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapContainer.GetChild(i).gameObject);
        }
    }

    private void SetTilesType()
    {
        Vector2Int startCoord = SetStartTile(); // 중심에서 가장 먼 타일들 중에서 시작점 선택

        playerStartPos = startCoord; // 최조 플레이어 위치 지정 

        Vector2Int bossCoord = SetBossTile(startCoord); // 시작점에서 가장 먼 타일을 보스 위치로 지정

        mapTiles[startCoord].SetHexType(HexType.Start);

        mapTiles[bossCoord].SetHexType(HexType.Boss);

        MoveCamera(mapTiles[startCoord].transform.position);
    }

    // 시작 타일 정하기 (중심에서 먼 타일들 찾기)
    private Vector2Int SetStartTile()
    {
        List<Vector2Int> candidateTiles = new();

        int maxDistance = -1;

        // 가장 먼 거리 찾기
        foreach (var tile in mapTiles)
        {
            int distance = HexDistance(Vector2Int.zero, tile.Key);

            maxDistance = Mathf.Max(maxDistance, distance);
        }

        // 최대 거리 근처만 외곽으로 사용
        foreach (var tile in mapTiles)
        {
            int distance = HexDistance(Vector2Int.zero, tile.Key);

            // 최대 거리에서 outerTolerance 이내인 타일들을 외곽 타일로 간주
            if (distance >= maxDistance - startTolerance)
            {
                candidateTiles.Add(tile.Key);
            }
        }

        return candidateTiles[Random.Range(0, candidateTiles.Count)];
    }

    // 보스 타일 정하기 (특정 좌표에서 가장 먼 타일들 찾기)
    private Vector2Int SetBossTile(Vector2Int from)
    {
        List<Vector2Int> candidateTiles = new();
        int maxDistance = -1;

        // 최대 거리 찾기
        foreach (var tile in mapTiles)
        {
            int distance = HexDistance(from, tile.Key);

            maxDistance = Mathf.Max(maxDistance, distance);
        }

        // 최대 거리 근처 타일 수집
        foreach (var tile in mapTiles)
        {
            int distance = HexDistance(from, tile.Key);

            // 최대 거리에서 bossTolerance 이내
            if (distance >= maxDistance - bossTolerance)
            {
                candidateTiles.Add(tile.Key);
            }
        }

        return candidateTiles[Random.Range(0, candidateTiles.Count)];
    }



    // 헥스 좌표 간의 거리 계산 (큐브 좌표로 변환하여 계산)
    private int HexDistance(Vector2Int a, Vector2Int b)
    {
        int aq = a.x;
        int ar = a.y;
        int aS = -aq - ar;

        int bq = b.x;
        int br = b.y;
        int bS = -bq - br;

        return (Mathf.Abs(aq - bq) + Mathf.Abs(ar - br) + Mathf.Abs(aS - bS)) / 2;
    }

    private void MoveCamera(Vector3 targetPos)
    {
        Debug.Log($"Moving camera to: {targetPos}");
        mainCamera.transform.position = targetPos + mainCamera.transform.rotation * cameraOffset;
    }
}
