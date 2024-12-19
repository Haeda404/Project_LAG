// RandomMapGenerator.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 15;
    public int numberOfStartNodes; // �ּ� ���� ���
    public List<MapNode> mapNodes = new List<MapNode>();
    public Transform mapNodeContainer; // �� ��� ��ư�� ���� �����̳�
    public GameObject mapNodeButtonPrefab; // �� ��� ��ư ������
    public GameObject mapNodePrefab; // �̸� ������ �� ��� ������
    public GameObject pathPrefab; // ��� �� ��θ� ��Ÿ�� ������

    public Sprite battleSprite; // ��Ʋ ��� ��������Ʈ
    public Sprite eventSprite; // �̺�Ʈ ��� ��������Ʈ
    public Sprite treasureSprite; // ���� ��� ��������Ʈ
    public Sprite restSprite; // �޽� ��� ��������Ʈ
    public Sprite shopSprite; // ���� ��� ��������Ʈ
    public Sprite eliteSprite; // ����Ʈ ��� ��������Ʈ
    public Sprite bossSprite; // ���� ��� ��������Ʈ

    private Dictionary<MapNode.NodeType, Sprite> spriteCache;
    private Vector2Int currentPlayerPosition;
    private List<GameObject> mapNodeButtons = new List<GameObject>();
    private List<GameObject> paths = new List<GameObject>();

    private void Awake()
    {
        // ��������Ʈ ĳ�� �ʱ�ȭ
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
            Debug.LogError("mapNodeContainer, mapNodeButtonPrefab, mapNodePrefab �Ǵ� pathPrefab�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ��� �ּ���.");
            return;
        }

        ClearPreviousMap();

        bool[,] mapGrid = new bool[gridWidth, gridHeight];

        // ���� ��� ���� (1��)
        List<MapNode> startNodes = new List<MapNode>();
        numberOfStartNodes = Mathf.Clamp(Random.Range(2, 7), 2, 6); // ���� ����� ���� �ּ� 2������ �ִ� 6���� ����
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

        // ��� ���� (1������ 15������ ��θ� ����)
        for (int i = 0; i < 6; i++)
        {
            MapNode current = startNodes[i % startNodes.Count];
            for (int y = 1; y < gridHeight; y++)
            {
                if (current.position.y + 1 != y) // �ٷ� ���� �����θ� ����ǵ��� ����
                {
                    continue;
                }
                int newX = Mathf.Clamp(current.position.x + Random.Range(-1, 2), 0, gridWidth - 1);
                Vector2Int newPosition = new Vector2Int(newX, y);

                if (!mapGrid[newX, y] && current.position.y + 1 == y)
                {
                    MapNode newNode = CreateNode(newX, y, GetRandomNodeType(newY: y, currentNode: current)); // ��� Ÿ�� ���� �� ���ǿ� ���� ����
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

        // ���� �������� ��� ���� (������ �� ��, X��ǥ 0�� �� �ϳ��� ��ġ)
        MapNode bossNode = CreateNode(3, gridHeight, MapNode.NodeType.Boss);
        mapNodes.Add(bossNode);

        // ������ ���� ��� ���� ���� ��� ����
        foreach (MapNode node in mapNodes)
        {
            if (node.position.y == gridHeight - 1)
            {
                node.connectedNodes.Add(bossNode);
                bossNode.connectedNodes.Add(node);
                CreatePathBetweenNodes(node, bossNode);
            }
        }

        currentPlayerPosition = new Vector2Int(startNodes[0].position.x, -1); // �÷��̾� �ʱ� ��ġ ����
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

        // ��������Ʈ ����
        UnityEngine.UI.Image spriteRenderer = nodeObject.GetComponent<UnityEngine.UI.Image>();
        if (spriteCache.TryGetValue(nodeType, out Sprite sprite))
        {
            spriteRenderer.sprite = sprite;

            if (nodeType == MapNode.NodeType.Boss)
            {
                nodeObject.transform.localScale = new Vector3(2, 2, 1); // ���� ��������Ʈ ũ�� �� ��� ����
            }
        }
        else
        {
            Debug.LogWarning($"��������Ʈ�� �����Ǿ����ϴ�: {nodeType}");
        }

        newNode.transform.localPosition = new Vector3(x * 200.0f + 100.0f, y * 150.0f - 2400.0f, 0);
        nodeObject.transform.SetAsLastSibling();
        return newNode;
    }

    private void CreatePathBetweenNodes(MapNode fromNode, MapNode toNode)
    {
        // ���ο� ��� UI ��� ����
        GameObject pathObject = new GameObject("Path");
        pathObject.transform.SetParent(mapNodeContainer, false);
        UnityEngine.UI.Image lineImage = pathObject.AddComponent<UnityEngine.UI.Image>();

        // �� �̹��� ���� (���⼭�� ����� �簢�� �̹����� ���)
        lineImage.color = Color.white;
        lineImage.sprite = null; // ���ϴ� ��������Ʈ�� ��ü ���� (�⺻ �簢��)

        // ���� ���۰� �� ��ġ ����
        Vector3 startPosition = fromNode.transform.localPosition;
        Vector3 endPosition = toNode.transform.localPosition;

        // �Ÿ��� ���� ���
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;

        // �̹����� RectTransform ����
        RectTransform rectTransform = pathObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(distance, 5f); // ���� ���̿� �β�
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.localPosition = startPosition;
        rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        paths.Add(pathObject);
    }

    private MapNode.NodeType GetRandomNodeType(int newY, MapNode currentNode)
    {
        // ���� ���ǿ� ���� ��� Ÿ�� ����
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
            // 1������ 5������ ����Ʈ�� �޽��� �����ϰ� ����
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
            // 6�� ���ĺ��ʹ� ��� ������ ��尡 ������ �� ����
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
        // ������ ������ ��ư ����
        foreach (GameObject button in mapNodeButtons)
        {
            Destroy(button);
        }
        mapNodeButtons.Clear();

        foreach (MapNode node in mapNodes)
        {
            if (mapNodeButtonPrefab == null)
            {
                Debug.LogError("mapNodeButtonPrefab�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ��� �ּ���.");
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
                Debug.Log("�� ��� ��ư ����: " + node.nodeType);
#endif
            }
        }
    }

    private bool IsNodeReachable(Vector2Int nodePosition)
    {
        if (currentPlayerPosition.y == -1)
        {
            // ù ���� ��� ��忡 ���� ����
            return nodePosition.y == 0;
        }

        // ���� �÷��̾��� ��ġ�� ����� ��� �� ���� ���� ��ġ�� ��常 ���� �����ϵ��� ����
        MapNode currentNode = mapNodes.Find(node => node.position == currentPlayerPosition);
        if (currentNode != null)
        {
            return currentNode.connectedNodes.Exists(node => node.position == nodePosition && node.position.y == currentPlayerPosition.y + 1);
        }
        return false;
    }

    private void OnMapNodeSelected(MapNode node)
    {
        // ���� �÷��̾� ��ġ ����� �ð��� ǥ�� ����
        MapNode previousNode = mapNodes.Find(n => n.position == currentPlayerPosition);
        if (previousNode != null)
        {
            GameObject previousNodeObject = previousNode.gameObject;
            UnityEngine.UI.Image previousSpriteRenderer = previousNodeObject.GetComponent<UnityEngine.UI.Image>();
            previousSpriteRenderer.color = Color.white; // ���� ������ ����
        }
#if UNITY_EDITOR
        Debug.Log("���õ� ���: " + node.nodeType);
#endif

        currentPlayerPosition = node.position;

        // ���� �÷��̾� ��ġ�� ���� �� �� �Ʒ� ���� ��带 ȸ������ ǥ��
        foreach (MapNode mapNode in mapNodes)
        {
            if (mapNode.position.y <= currentPlayerPosition.y)
            {
                UnityEngine.UI.Image nodeSpriteRenderer = mapNode.gameObject.GetComponent<UnityEngine.UI.Image>();
                nodeSpriteRenderer.color = Color.gray; // ���� �� �� �Ʒ� �� ��带 ȸ������ ǥ��
            }
        }

        // ���� �÷��̾� ��ġ ����� �ð��� ǥ�� �߰�
        UnityEngine.UI.Image spriteRenderer = node.gameObject.GetComponent<UnityEngine.UI.Image>();
        spriteRenderer.color = Color.yellow; // ���� �÷��̾� ��ġ�� ��������� ǥ��

        // ��� ������ ���� �������� ����
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
                Debug.LogWarning("�� �� ���� ��� Ÿ��: " + node.nodeType);
                break;
        }
        PopulateMapNodes();
    }

    // �� ���������� �����ϴ� �޼����
    private void StartBattleStage()
    {
        // ���� �ý����� ȣ���Ͽ� ���� ����
        Debug.Log("���� ����");
        // ���� �ý��� ������ ���⿡ ����
    }

    private void OpenShop()
    {
        // ���� UI�� ȣ���Ͽ� �÷��̾ ��ȣ�ۿ��� �� �ְ� ��
        Debug.Log("���� ����");
        // ���� �ý��� ������ ���⿡ ����
    }

    private void StartEvent()
    {
        // �̺�Ʈ�� �����Ͽ� ������ �̺�Ʈ �߻�
        Debug.Log("�̺�Ʈ ����");
        // �̺�Ʈ ������ ���⿡ ����
    }

    private void OpenTreasure()
    {
        // ���� ���ڸ� ���� ������ ����
        Debug.Log("���� ���� ����");
        // ���� ������ ���⿡ ����
    }

    private void RestStage()
    {
        // �޽� ��ҿ��� ü�� ȸ�� �Ǵ� ��ų ��ȭ
        Debug.Log("�޽� ����");
        // �޽� ������ ���⿡ ����
    }

    private void StartBossBattle()
    {
        // ���� ���� ����
        Debug.Log("���� ���� ����");
        // ���� ���� ������ ���⿡ ����
    }
}
