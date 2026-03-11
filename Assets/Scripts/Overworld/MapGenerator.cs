using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 5;

    [Header("Node Counts")]
    [SerializeField] private int minNodes = 12;
    [SerializeField] private int maxNodes = 16;

    [Header("Type Counts")]
    [SerializeField] private int healNodes = 2;
    [SerializeField] private int shopNodes = 1;

    /// <summary>
    /// Generates a random MapGraph that always has a valid path
    /// from Start to Exit and a few branches.
    /// </summary>
    public MapGraph Generate()
    {
        MapGraph graph = new MapGraph();

        int targetNodeCount = Mathf.Clamp(Random.Range(minNodes, maxNodes + 1), 4, rows * columns);

        // 1) Build a main path from column 0 to last column
        List<NodeData> mainPath = BuildMainPath(graph, columns);

        // 2) Add branch nodes until we reach target count
        AddBranches(graph, mainPath, targetNodeCount);

        // 3) Assign node types
        AssignNodeTypes(graph);

        return graph;
    }

    private List<NodeData> BuildMainPath(MapGraph graph, int columnCount)
    {
        List<NodeData> mainPath = new List<NodeData>();

        int idCounter = 0;
        float colStep = columnCount > 1 ? 1f / (columnCount - 1) : 1f;
        float rowStep = rows > 1 ? 1f / (rows - 1) : 1f;

        int previousId = -1;

        for (int col = 0; col < columnCount; col++)
        {
            int row = Random.Range(0, rows);

            NodeData node = new NodeData
            {
                id = idCounter++,
                type = NodeType.Enemy, // temporary, real types are assigned later
                position = new Vector2(colStep * col, rowStep * row)
            };

            graph.nodes.Add(node);
            mainPath.Add(node);

            if (previousId >= 0)
            {
                NodeData prevNode = graph.GetNodeById(previousId);
                if (prevNode != null)
                {
                    prevNode.nextNodeIds.Add(node.id);
                }
            }

            previousId = node.id;
        }

        if (mainPath.Count > 0)
        {
            graph.startNodeId = mainPath[0].id;
            graph.exitNodeId = mainPath[mainPath.Count - 1].id;
        }

        return mainPath;
    }

    private void AddBranches(MapGraph graph, List<NodeData> mainPath, int targetNodeCount)
    {
        if (mainPath.Count < 2)
        {
            return;
        }

        int idCounter = graph.nodes.Count;
        float colStep = columns > 1 ? 1f / (columns - 1) : 1f;
        float rowStep = rows > 1 ? 1f / (rows - 1) : 1f;

        int maxAttempts = 100;
        int attempts = 0;

        while (graph.nodes.Count < targetNodeCount && attempts < maxAttempts)
        {
            attempts++;

            // pick a random node on the main path except the last (exit)
            int mainIndex = Random.Range(0, mainPath.Count - 1);
            NodeData fromNode = mainPath[mainIndex];

            // pick a target column further to the right to ensure forward progress
            int toMainIndex = Random.Range(mainIndex + 1, mainPath.Count);
            NodeData toNode = mainPath[toMainIndex];

            // create 1-2 nodes path from fromNode to toNode
            int branchLength = Random.Range(1, 3);
            NodeData lastBranchNode = fromNode;

            for (int i = 0; i < branchLength && graph.nodes.Count < targetNodeCount; i++)
            {
                float t = (i + 1f) / (branchLength + 1f);
                float colF = Mathf.Lerp(lastBranchNode.position.x, toNode.position.x, t);

                // choose a random nearby row
                float rowOffset = Random.Range(-1, 2);
                float rowIndex = Mathf.Clamp(Mathf.RoundToInt((lastBranchNode.position.y / rowStep) + rowOffset), 0, rows - 1);

                Vector2 pos = new Vector2(colF, rowStep * rowIndex);

                NodeData branchNode = new NodeData
                {
                    id = idCounter++,
                    type = NodeType.Enemy,
                    position = pos
                };

                graph.nodes.Add(branchNode);

                if (!lastBranchNode.nextNodeIds.Contains(branchNode.id))
                {
                    lastBranchNode.nextNodeIds.Add(branchNode.id);
                }

                lastBranchNode = branchNode;
            }

            if (!lastBranchNode.nextNodeIds.Contains(toNode.id))
            {
                lastBranchNode.nextNodeIds.Add(toNode.id);
            }
        }
    }

    private void AssignNodeTypes(MapGraph graph)
    {
        if (graph.nodes.Count == 0)
        {
            return;
        }

        // Start and Exit
        NodeData startNode = graph.GetNodeById(graph.startNodeId);
        NodeData exitNode = graph.GetNodeById(graph.exitNodeId);

        if (startNode != null)
        {
            startNode.type = NodeType.Start;
        }

        if (exitNode != null)
        {
            exitNode.type = NodeType.Exit;
        }

        // collect candidate nodes for special types (exclude Start/Exit)
        List<NodeData> candidates = new List<NodeData>();
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            NodeData node = graph.nodes[i];
            if (node.id == graph.startNodeId || node.id == graph.exitNodeId)
            {
                continue;
            }

            candidates.Add(node);
        }

        Shuffle(candidates);

        int index = 0;

        // Shop
        if (shopNodes > 0 && index < candidates.Count)
        {
            candidates[index].type = NodeType.Shop;
            index++;
        }

        // Heal nodes
        for (int i = 0; i < healNodes && index < candidates.Count; i++, index++)
        {
            candidates[index].type = NodeType.Heal;
        }

        // Rest: Enemy
        for (int i = index; i < candidates.Count; i++)
        {
            candidates[i].type = NodeType.Enemy;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

