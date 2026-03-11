using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeData
{
    public int id;
    public NodeType type;
    public Vector2 position;
    public List<int> nextNodeIds = new List<int>();
}

[System.Serializable]
public class MapGraph
{
    public List<NodeData> nodes = new List<NodeData>();
    public int startNodeId;
    public int exitNodeId;

    public NodeData GetNodeById(int id)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == id)
            {
                return nodes[i];
            }
        }

        return null;
    }
}

