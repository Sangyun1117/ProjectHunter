using System.Collections.Generic;
using UnityEngine;

public class SDSMapGenerator : MonoBehaviour
{
    [SerializeField] private Transform boardContainer;
    [SerializeField] private List<PointOfInterest> pointsOfInterestPrefabs;
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private int numberOfStartingPoints = 4; //시작 노드의 개수
    [SerializeField] private int mapLength = 10; //층 수
    [SerializeField] private int maxWidth = 5; //각 층의 최대 노드 수(가로)
    [SerializeField] private float xMaxSize = 10; //맵 가로 최대 크기
    [SerializeField] private float yPadding = 5; //층 사이의 간격
    [SerializeField] private bool allowCrisscrossing = true; //경로 교차 허용 여부
    [Range(0.1f, 1f), SerializeField] private float chancePathMiddle; //중앙으로 연결될 확률
    [Range(0f, 1f), SerializeField] private float chancePathSide; //양 옆으로 연결될 확률
    [SerializeField, Range(0.9f, 5f)] private float multiplicativeSpaceBetweenLines = 2.5f;
    [SerializeField, Range(1f, 5.5f)] private float multiplicativeNumberOfMinimunConnections = 3f;

    private PointOfInterest[][] _pointOfInterestsPerFloor;
    private List<PointOfInterest> pointsOfInterest = new();
    private int numberOfConnections = 0;
    private float lineLength;
    private float lineHeight;
    [SerializeField] private int maxRetryCount = 10;
    private int currentRetryCount = 0;

    private void Start()
    {
        RecreateBoard();
    }

    public void RecreateBoard()
    {
        if (currentRetryCount >= maxRetryCount)
        {
            Debug.LogError("Map generation failed: Max retry count reached.");
            return;
        }
        currentRetryCount++;
        //lineLength = pathPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * pathPrefab.transform.localScale.z;
        //lineHeight = pathPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * pathPrefab.transform.localScale.y;
        lineLength = 1f;
        lineHeight = 0.2f;

        // 기존 맵 삭제
        DestroyImmediateAllChildren(boardContainer);
        //연결 갯수 초기화
        numberOfConnections = 0;
        //랜덤 시드 초기화
        GenerateRandomSeed();

        pointsOfInterest.Clear();
        _pointOfInterestsPerFloor = new PointOfInterest[mapLength][];
        for (int i = 0; i < _pointOfInterestsPerFloor.Length; i++)
        {
            _pointOfInterestsPerFloor[i] = new PointOfInterest[maxWidth];
        }
        CreateMap();
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    private PointOfInterest InstantiatePointOfInterest(int floorN, int xNum) //
    {
        if (_pointOfInterestsPerFloor[floorN][xNum] != null)
        {
            return _pointOfInterestsPerFloor[floorN][xNum];
        }

        float xSize = xMaxSize / maxWidth;
        float xPos = (xSize * xNum) - (xMaxSize / 2f) + (xSize / 2f);
        float yPos = yPadding * floorN;

        //Add a random padding
        xPos += Random.Range(-xSize / 4f, xSize / 4f);
        yPos += Random.Range(-yPadding / 4f, yPadding / 4f);

        Vector3 pos = new Vector3(xPos, yPos, 0);
        PointOfInterest randomPOI = pointsOfInterestPrefabs[Random.Range(0, pointsOfInterestPrefabs.Count)];
        PointOfInterest instance = Instantiate(randomPOI, boardContainer);
        pointsOfInterest.Add(instance);

        instance.transform.localPosition = pos;
        _pointOfInterestsPerFloor[floorN][xNum] = instance;
        int created = 0;

        void InstantiateNextPoint(int index_i, int index_j)
        {
            PointOfInterest nextPOI = InstantiatePointOfInterest(index_i, index_j);
            AddLineBetweenPoints(instance, nextPOI);
            instance.NextPointsOfInterest.Add(nextPOI);
            created++;
            numberOfConnections++;
        }

        while (created == 0 && floorN < mapLength - 1)
        {
            if (xNum > 0 && Random.Range(0f, 1f) < chancePathSide)
            {
                if (allowCrisscrossing || _pointOfInterestsPerFloor[floorN + 1][xNum - 1] == null)
                {
                    InstantiateNextPoint(floorN + 1, xNum - 1);
                }
            }

            if (xNum < maxWidth - 1 && Random.Range(0f, 1f) < chancePathSide)
            {
                if (allowCrisscrossing || _pointOfInterestsPerFloor[floorN + 1][xNum + 1] == null)
                {
                    InstantiateNextPoint(floorN + 1, xNum + 1);
                }
            }

            if (Random.Range(0f, 1f) < chancePathMiddle)
            {
                InstantiateNextPoint(floorN + 1, xNum);
            }
        }

        return instance;
    }

    private void CreateMap()
    {
        // 시작 노드 선정
        List<int> positions = GetRandomIndexes(numberOfStartingPoints);
        foreach (int pos in positions)
        {
            _ = InstantiatePointOfInterest(0, pos);
        }


        if (numberOfConnections <= mapLength * multiplicativeNumberOfMinimunConnections)
        {
            Debug.Log($"Recreating board with {numberOfConnections} connections");
            RecreateBoard();
            return;
        }

        currentRetryCount = 0;
        Debug.Log($"Created board with {numberOfConnections} connections");
        Debug.Log($"Created board with {pointsOfInterest.Count} points");
    }

    // 선택된 두 노드 사이에 선을 그리는 함수
    private void AddLineBetweenPoints(PointOfInterest thisPoint, PointOfInterest nextPoint)
    {
        Vector3 start = thisPoint.transform.position;
        Vector3 end = nextPoint.transform.position;

        Vector3 dir = end - start;

        float dist = dir.magnitude;

        GameObject line = Instantiate(pathPrefab, boardContainer);

        line.transform.position = (start + end) / 2f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        line.transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 scale = line.transform.localScale;
        scale.x = dist;
        line.transform.localScale = scale;

    }

    // 중복 없는 랜덤 인덱스 생성 함수
    private List<int> GetRandomIndexes(int n)
    {
        List<int> indexes = new List<int>();
        if (n > maxWidth)
        {
            throw new System.Exception("Number of starting points greater than maxWidth!");
        }

        while (indexes.Count < n)
        {
            int randomNum = Random.Range(0, maxWidth);
            if (!indexes.Contains(randomNum))
            {
                indexes.Add(randomNum);
            }
        }
        return indexes;
    }

    // Transform의 모든 자식 오브젝트를 즉시 삭제하는 함수
    private void DestroyImmediateAllChildren(Transform transform)
    {
        List<Transform> toKill = new();

        foreach (Transform child in transform)
        {
            toKill.Add(child);
        }

        //역순으로 삭제해야 하는 이유는, DestroyImmediate가 즉시 삭제하기 때문에, 리스트를 순회하면서 삭제하면 인덱스가 꼬이기 때문입니다.
        for (int i = toKill.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(toKill[i].gameObject);
        }
    }
}
