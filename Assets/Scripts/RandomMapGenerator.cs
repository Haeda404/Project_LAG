// RandomMapGenerator.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 15;
    public int numberOfStartNodes; // 최소 시작 노드
    public List<MapNode> mapNodes = new List<MapNode>();
    public Transform mapNodeContainer; // 맵 노드 버튼을 담을 컨테이너
    public GameObject mapNodeButtonPrefab; // 맵 노드 버튼 프리팹
    public GameObject mapNodePrefab; // 미리 구성된 맵 노드 프리팹
    public GameObject pathPrefab; // 노드 간 경로를 나타낼 프리팹

    public Sprite battleSprite; // 배틀 노드 스프라이트
    public Sprite eventSprite; // 이벤트 노드 스프라이트
    public Sprite treasureSprite; // 보물 노드 스프라이트
    public Sprite restSprite; // 휴식 노드 스프라이트
    public Sprite shopSprite; // 상점 노드 스프라이트
    public Sprite eliteSprite; // 엘리트 노드 스프라이트
    public Sprite bossSprite; // 보스 노드 스프라이트

    private Dictionary<MapNode.NodeType, Sprite> spriteCache;
    private Vector2Int currentPlayerPosition;
    private List<GameObject> mapNodeButtons = new List<GameObject>();
    private List<GameObject> paths = new List<GameObject>();

    private void Awake()
    {
        // 스프라이트 캐싱 초기화
        spriteCache = new Dictionary<MapNode.NodeType, Sprite>
        {
            { MapNode.NodeType.Boss, bossSprite },
            { MapNode.NodeType.Battle, battleSprite },
            { MapNode.NodeType.Shop, shopSprite },
            { MapNode.NodeType.Elite, eliteSprite },
            { MapNode.NodeType.Event, eventSprite },
            { MapNode.NodeType.Treasure, treasureSprite },
            { MapNode.NodeType.Rest, restSprite }
        };
    }

    public void GenerateRandomMap()
    {
        if (mapNodeContainer == null || mapNodeButtonPrefab == null || mapNodePrefab == null || pathPrefab == null)
        {
            Debug.LogError("mapNodeContainer, mapNodeButtonPrefab, mapNodePrefab 또는 pathPrefab이 할당되지 않았습니다. Inspector에서 할당해 주세요.");
            return;
        }

        ClearPreviousMap();

        bool[,] mapGrid = new bool[gridWidth, gridHeight];

        // 시작 노드 선택 (1층)
        List<MapNode> startNodes = new List<MapNode>();
        numberOfStartNodes = Mathf.Clamp(Random.Range(2, 7), 2, 6); // 시작 노드의 수를 최소 2개에서 최대 6개로 설정
        for (int i = 0; i < numberOfStartNodes; i++)
        {
            int x;
            do
            {
                x = Random.Range(0, gridWidth);
            } while (startNodes.Exists(n => n.position.x == x));

            MapNode startNode = CreateNode(x, 0, MapNode.NodeType.Battle);
            startNodes.Add(startNode);
            mapGrid[x, 0] = true;
            mapNodes.Add(startNode);
        }

        // 경로 생성 (1층부터 15층까지 경로를 연결)
        for (int i = 0; i < 6; i++)
        {
            MapNode current = startNodes[i % startNodes.Count];
            for (int y = 1; y < gridHeight; y++)
            {
                if (current.position.y + 1 != y) // 바로 다음 층으로만 연결되도록 설정
                {
                    continue;
                }
                int newX = Mathf.Clamp(current.position.x + Random.Range(-1, 2), 0, gridWidth - 1);
                Vector2Int newPosition = new Vector2Int(newX, y);

                if (!mapGrid[newX, y] && current.position.y + 1 == y)
                {
                    MapNode newNode = CreateNode(newX, y, GetRandomNodeType(newY: y, currentNode: current)); // 노드 타입 생성 시 조건에 따라 설정
                    mapNodes.Add(newNode);
                    mapGrid[newX, y] = true;
                    current.connectedNodes.Add(newNode);
                    newNode.connectedNodes.Add(current);
                    CreatePathBetweenNodes(current, newNode);
                    current = newNode;
                }
                else
                {
                    MapNode connectedNode = mapNodes.Find(node => node.position == newPosition);
                    if (connectedNode != null && !current.connectedNodes.Contains(connectedNode))
                    {
                        current.connectedNodes.Add(connectedNode);
                        connectedNode.connectedNodes.Add(current);
                        CreatePathBetweenNodes(current, connectedNode);
                        current = connectedNode;
                    }
                }
            }
        }

        // 보스 스테이지 노드 생성 (마지막 층 위, X좌표 0에 단 하나만 위치)
        MapNode bossNode = CreateNode(3, gridHeight, MapNode.NodeType.Boss);
        mapNodes.Add(bossNode);

        // 마지막 층의 모든 노드와 보스 노드 연결
        foreach (MapNode node in mapNodes)
        {
            if (node.position.y == gridHeight - 1)
            {
                node.connectedNodes.Add(bossNode);
                bossNode.connectedNodes.Add(node);
                CreatePathBetweenNodes(node, bossNode);
            }
        }

        currentPlayerPosition = new Vector2Int(startNodes[0].position.x, -1); // 플레이어 초기 위치 설정
        PopulateMapNodes();
    }

    private void ClearPreviousMap()
    {
        foreach (GameObject button in mapNodeButtons)
        {
            Destroy(button);
        }
        foreach (GameObject path in paths)
        {
            Destroy(path);
        }
        mapNodeButtons.Clear();
        paths.Clear();
        mapNodes.Clear();
    }

    private MapNode CreateNode(int x, int y, MapNode.NodeType nodeType)
    {
        GameObject nodeObject = Instantiate(mapNodePrefab, mapNodeContainer);
        nodeObject.name = $"MapNode_{x}_{y}";

        MapNode newNode = nodeObject.GetComponent<MapNode>() ?? nodeObject.AddComponent<MapNode>();
        newNode.nodeType = nodeType;
        newNode.position = new Vector2Int(x, y);

        // 스프라이트 설정
        UnityEngine.UI.Image spriteRenderer = nodeObject.GetComponent<UnityEngine.UI.Image>();
        if (spriteCache.TryGetValue(nodeType, out Sprite sprite))
        {
            spriteRenderer.sprite = sprite;

            if (nodeType == MapNode.NodeType.Boss)
            {
                nodeObject.transform.localScale = new Vector3(2, 2, 1); // 보스 스프라이트 크기 두 배로 설정
            }
        }
        else
        {
            Debug.LogWarning($"스프라이트가 누락되었습니다: {nodeType}");
        }

        newNode.transform.localPosition = new Vector3(x * 200.0f + 100.0f, y * 150.0f - 2400.0f, 0);
        nodeObject.transform.SetAsLastSibling();
        return newNode;
    }

    private void CreatePathBetweenNodes(MapNode fromNode, MapNode toNode)
    {
        // 새로운 경로 UI 요소 생성
        GameObject pathObject = new GameObject("Path");
        pathObject.transform.SetParent(mapNodeContainer, false);
        UnityEngine.UI.Image lineImage = pathObject.AddComponent<UnityEngine.UI.Image>();

        // 선 이미지 설정 (여기서는 흰색의 사각형 이미지를 사용)
        lineImage.color = Color.white;
        lineImage.sprite = null; // 원하는 스프라이트로 교체 가능 (기본 사각형)

        // 선의 시작과 끝 위치 설정
        Vector3 startPosition = fromNode.transform.localPosition;
        Vector3 endPosition = toNode.transform.localPosition;

        // 거리와 각도 계산
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;

        // 이미지의 RectTransform 설정
        RectTransform rectTransform = pathObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(distance, 5f); // 선의 길이와 두께
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.localPosition = startPosition;
        rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        paths.Add(pathObject);
    }

    private MapNode.NodeType GetRandomNodeType(int newY, MapNode currentNode)
    {
        // 층별 조건에 따라 노드 타입 결정
        if (newY == 0)
        {
            return MapNode.NodeType.Battle;
        }
        else if (newY == 8)
        {
            return MapNode.NodeType.Treasure;
        }
        else if (newY == 14)
        {
            return MapNode.NodeType.Rest;
        }
        else if (newY <= 5)
        {
            // 1층부터 5층까지 엘리트와 휴식은 제외하고 생성
            float randomValue = Random.value;
            if (randomValue < 0.05f)
            {
                return MapNode.NodeType.Shop;
            }
            else if (randomValue < 0.22f)
            {
                return MapNode.NodeType.Event;
            }
            else
            {
                return MapNode.NodeType.Battle;
            }
        }
        else
        {
            // 6층 이후부터는 모든 유형의 노드가 생성될 수 있음
            float randomValue = Random.value;
            if (randomValue < 0.05f)
            {
                return MapNode.NodeType.Shop;
            }
            else if (randomValue < 0.12f)
            {
                return MapNode.NodeType.Rest;
            }
            else if (randomValue < 0.22f)
            {
                return MapNode.NodeType.Event;
            }
            else if (randomValue < 0.16f)
            {
                return MapNode.NodeType.Elite;
            }
            else
            {
                return MapNode.NodeType.Battle;
            }
        }
    }

    private void PopulateMapNodes()
    {
        // 이전에 생성된 버튼 삭제
        foreach (GameObject button in mapNodeButtons)
        {
            Destroy(button);
        }
        mapNodeButtons.Clear();

        foreach (MapNode node in mapNodes)
        {
            if (mapNodeButtonPrefab == null)
            {
                Debug.LogError("mapNodeButtonPrefab이 할당되지 않았습니다. Inspector에서 할당해 주세요.");
                return;
            }

            if (IsNodeReachable(node.position))
            {
                GameObject nodeButton = Instantiate(mapNodeButtonPrefab, mapNodeContainer);
                mapNodeButtons.Add(nodeButton);
                MapNode capturedNode = node;
                nodeButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                nodeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnMapNodeSelected(capturedNode));
                nodeButton.SetActive(true);
                nodeButton.transform.localPosition = node.transform.localPosition;
#if UNITY_EDITOR
                Debug.Log("맵 노드 버튼 생성: " + node.nodeType);
#endif
            }
        }
    }

    private bool IsNodeReachable(Vector2Int nodePosition)
    {
        if (currentPlayerPosition.y == -1)
        {
            // 첫 층의 모든 노드에 접근 가능
            return nodePosition.y == 0;
        }

        // 현재 플레이어의 위치와 연결된 노드 중 다음 층에 위치한 노드만 접근 가능하도록 설정
        MapNode currentNode = mapNodes.Find(node => node.position == currentPlayerPosition);
        if (currentNode != null)
        {
            return currentNode.connectedNodes.Exists(node => node.position == nodePosition && node.position.y == currentPlayerPosition.y + 1);
        }
        return false;
    }

    private void OnMapNodeSelected(MapNode node)
    {
        // 이전 플레이어 위치 노드의 시각적 표시 제거
        MapNode previousNode = mapNodes.Find(n => n.position == currentPlayerPosition);
        if (previousNode != null)
        {
            GameObject previousNodeObject = previousNode.gameObject;
            UnityEngine.UI.Image previousSpriteRenderer = previousNodeObject.GetComponent<UnityEngine.UI.Image>();
            previousSpriteRenderer.color = Color.white; // 원래 색으로 복원
        }
#if UNITY_EDITOR
        Debug.Log("선택된 노드: " + node.nodeType);
#endif

        currentPlayerPosition = node.position;

        // 현재 플레이어 위치와 같은 층 및 아래 층의 노드를 회색으로 표시
        foreach (MapNode mapNode in mapNodes)
        {
            if (mapNode.position.y <= currentPlayerPosition.y)
            {
                UnityEngine.UI.Image nodeSpriteRenderer = mapNode.gameObject.GetComponent<UnityEngine.UI.Image>();
                nodeSpriteRenderer.color = Color.gray; // 같은 층 및 아래 층 노드를 회색으로 표시
            }
        }

        // 현재 플레이어 위치 노드의 시각적 표시 추가
        UnityEngine.UI.Image spriteRenderer = node.gameObject.GetComponent<UnityEngine.UI.Image>();
        spriteRenderer.color = Color.yellow; // 현재 플레이어 위치를 노란색으로 표시

        // 노드 유형에 따른 스테이지 시작
        switch (node.nodeType)
        {
            case MapNode.NodeType.Battle:
                StartBattleStage();
                break;
            case MapNode.NodeType.Shop:
                OpenShop();
                break;
            case MapNode.NodeType.Event:
                StartEvent();
                break;
            case MapNode.NodeType.Treasure:
                OpenTreasure();
                break;
            case MapNode.NodeType.Rest:
                RestStage();
                break;
            case MapNode.NodeType.Boss:
                StartBossBattle();
                break;
            default:
                Debug.LogWarning("알 수 없는 노드 타입: " + node.nodeType);
                break;
        }
        PopulateMapNodes();
    }

    // 각 스테이지를 시작하는 메서드들
    private void StartBattleStage()
    {
        // 전투 시스템을 호출하여 전투 시작
        Debug.Log("전투 시작");
        // 전투 시스템 로직을 여기에 구현
    }

    private void OpenShop()
    {
        // 상점 UI를 호출하여 플레이어가 상호작용할 수 있게 함
        Debug.Log("상점 열기");
        // 상점 시스템 로직을 여기에 구현
    }

    private void StartEvent()
    {
        // 이벤트를 시작하여 무작위 이벤트 발생
        Debug.Log("이벤트 시작");
        // 이벤트 로직을 여기에 구현
    }

    private void OpenTreasure()
    {
        // 보물 상자를 열고 보상을 제공
        Debug.Log("보물 상자 열기");
        // 보물 로직을 여기에 구현
    }

    private void RestStage()
    {
        // 휴식 장소에서 체력 회복 또는 스킬 강화
        Debug.Log("휴식 시작");
        // 휴식 로직을 여기에 구현
    }

    private void StartBossBattle()
    {
        // 보스 전투 시작
        Debug.Log("보스 전투 시작");
        // 보스 전투 로직을 여기에 구현
    }
}
