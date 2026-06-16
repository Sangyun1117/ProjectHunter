using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private Dictionary<Vector2Int, HexTile> mapTiles = new Dictionary<Vector2Int, HexTile>();
    private HexMapGenerator mapGenerator;

    [SerializeField] private GameObject playerPawn;
    [SerializeField] private float playerSpeed = 5f;
    private Vector2Int playerCurrentPos;

    private Coroutine movePlayerCoroutine;

    bool canMove = true;

    private void Awake()
    {
        mapGenerator = GetComponent<HexMapGenerator>();
    }
    public void InitializeMap(Dictionary<Vector2Int, HexTile> tiles, Vector2Int playerStartPos)
    {
        mapTiles = tiles;
        // 플레이어 초기 위치 설정
        //MovePlayerPawn(playerStartPos);
        Vector3 pos = mapTiles[playerStartPos].transform.position;
        playerPawn.transform.position = new Vector3(pos.x, pos.y, playerPawn.transform.position.z);
        playerCurrentPos = playerStartPos;
    }

    public void OnClickMap(Vector3 worldPos)
    {
        if(canMove == false)
        {
            return;
        }

        Vector2Int hexCoord = mapGenerator.WorldToHex(worldPos);
        if (mapTiles.TryGetValue(hexCoord, out HexTile tile) == true)
        {
            // 클릭한 타일이 플레이어의 현재 위치에서 인접한 타일인지 확인
            bool isNeighbor = false;
            foreach (var neighbor in GetNeighbors(playerCurrentPos))
            {
                if (neighbor.axialCoordinate == tile.axialCoordinate)
                {
                    isNeighbor = true;
                    break;
                }
            }
            if (isNeighbor == true)
            {
                //Debug.Log($"Move Player: {tile.name}");
                playerCurrentPos = tile.axialCoordinate;
                MovePlayerPawn(tile.axialCoordinate);
            }
            else
            {
                //Debug.Log("Clicked tile is not adjacent to the player.");
            }
        }
        else
        {
            //Debug.Log("Clicked outside of map bounds.");
        }
    }

    // 플레이어의 현재 위치에서 인접한 타일들을 반환하는 함수
    public List<HexTile> GetNeighbors(Vector2Int coord)
    {
        List<HexTile> neighbors = new List<HexTile>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
            new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborCoord = coord + dir;
            // 인접한 타일이 맵에 존재하는지 확인
            if (mapTiles.ContainsKey(neighborCoord) == true)
            {
                neighbors.Add(mapTiles[neighborCoord]);
            }
        }
        return neighbors;
    }

    private void MovePlayerPawn(Vector2Int pos)
    {
        HexTile tile = mapTiles[pos];
        Vector3 tilePos = tile.transform.position;
        //playerPawn.transform.position = new Vector3(tilePos.x, tilePos.y, playerPawn.transform.position.z);
        Vector3 targetPos = new Vector3(tilePos.x, tilePos.y, playerPawn.transform.position.z);

        if (movePlayerCoroutine != null)
        {
            StopCoroutine(movePlayerCoroutine);
        }

        canMove = false;
        movePlayerCoroutine = StartCoroutine(MovePlayerPawnRoutine(targetPos, tile));
    }

    private IEnumerator MovePlayerPawnRoutine(Vector3 targetPos, HexTile targetTile)
    {
        while (Vector3.Distance(playerPawn.transform.position, targetPos) > 0.01f)
        {
            playerPawn.transform.position = Vector3.MoveTowards(
                playerPawn.transform.position,
                targetPos,
                playerSpeed * Time.deltaTime
            );

            yield return null;
        }


        playerPawn.transform.position = targetPos;

        yield return new WaitForSeconds(0.1f);

        CheckTileEvent(targetTile);

        canMove = true;
        movePlayerCoroutine = null;
    }

    private void CheckTileEvent(HexTile hexTile)
    {
        hexTile.Explore();

        switch (hexTile.hexType)
        {
            case HexType.Empty:
                Debug.Log("Empty tile. No event.");
                break;
            case HexType.Start:
                Debug.Log("Start tile. No event.");
                break;
            case HexType.Battle:
                if(hexTile.visitCount == 1)
                {

                    SceneTransitionManager.Instance.LoadBattleScene();
                    Debug.Log("First time on battle tile. Prepare for a tough fight!");
                }
                else
                {
                    Debug.Log("Returning to battle tile. Enemies may be weaker.");
                }
                break;
            case HexType.Boss:
                if(hexTile.visitCount == 1)
                {
                    SceneTransitionManager.Instance.LoadBattleScene();
                    Debug.Log("First time on boss tile. Get ready for the ultimate challenge!");
                }
                else
                {
                    Debug.Log("Returning to boss tile. Boss may be weaker.");
                }
                break;
        }
    }

}
