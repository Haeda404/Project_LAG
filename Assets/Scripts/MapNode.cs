// MapNode.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public enum NodeType
    {
        Battle,
        Event,
        Treasure,
        Rest,
        Shop,
        Boss,
        Elite
    }

    public NodeType nodeType; // 노드의 유형 (일반 전투, 이벤트, 상점 등)
    public Vector2Int position; // 노드의 위치 (격자 좌표)

    // 노드의 경로 연결 정보
    public List<MapNode> connectedNodes = new List<MapNode>(); // 연결된 노드들

    // 향후 기능 확장을 위해 추가될 수 있는 메서드나 속성
}
