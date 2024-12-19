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

    public NodeType nodeType; // ����� ���� (�Ϲ� ����, �̺�Ʈ, ���� ��)
    public Vector2Int position; // ����� ��ġ (���� ��ǥ)

    // ����� ��� ���� ����
    public List<MapNode> connectedNodes = new List<MapNode>(); // ����� ����

    // ���� ��� Ȯ���� ���� �߰��� �� �ִ� �޼��峪 �Ӽ�
}
