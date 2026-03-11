using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapView mapView;
    [SerializeField] private RectTransform pawnIcon;

    [Header("Enemies")]
    [SerializeField] private List<EnemyData> possibleEnemies = new List<EnemyData>();
    [SerializeField] private string battleSceneName = "Battle";

    [Header("Shop UI")]
    [SerializeField] private GameObject shopPanel;

    private MapGraph currentGraph;
    private int currentNodeId = -1;

    private void Start()
    {
        EnsureGameSession();
        GenerateAndRenderNewMap();
    }

    private void EnsureGameSession()
    {
        if (GameSession.Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("GameSession");
        go.AddComponent<GameSession>();
    }

    private void GenerateAndRenderNewMap()
    {
        if (mapGenerator == null || mapView == null)
        {
            Debug.LogError("OverworldController is missing MapGenerator or MapView reference.");
            return;
        }

        currentGraph = mapGenerator.Generate();
        currentNodeId = currentGraph != null ? currentGraph.startNodeId : -1;

        mapView.Render(currentGraph, this);
        UpdatePawnPosition();
    }

    public void OnNodeClicked(int nodeId)
    {
        if (currentGraph == null)
        {
            return;
        }

        NodeData currentNode = currentGraph.GetNodeById(currentNodeId);
        if (currentNode == null)
        {
            return;
        }

        if (!currentNode.nextNodeIds.Contains(nodeId))
        {
            // cannot move there
            return;
        }

        currentNodeId = nodeId;
        UpdatePawnPosition();

        NodeData node = currentGraph.GetNodeById(nodeId);
        if (node == null)
        {
            return;
        }

        switch (node.type)
        {
            case NodeType.Enemy:
                HandleEnemyNode();
                break;
            case NodeType.Heal:
                HandleHealNode();
                break;
            case NodeType.Shop:
                HandleShopNode();
                break;
            case NodeType.Exit:
                HandleExitNode();
                break;
        }
    }

    private void UpdatePawnPosition()
    {
        if (pawnIcon == null || mapView == null || currentGraph == null)
        {
            return;
        }

        NodeData node = currentGraph.GetNodeById(currentNodeId);
        if (node == null)
        {
            return;
        }

        RectTransform container = mapView.Container;
        Rect rect = container.rect;
        Vector2 size = rect.size;
        Vector2 origin = new Vector2(rect.xMin, rect.yMin);

        Vector2 localPos = new Vector2(
            origin.x + (node.position.x * size.x),
            origin.y + (node.position.y * size.y));

        pawnIcon.SetParent(container, false);
        pawnIcon.anchoredPosition = localPos;
    }

    private void HandleEnemyNode()
    {
        if (GameSession.Instance == null)
        {
            return;
        }

        EnemyData enemy = ChooseEnemy();
        if (enemy == null)
        {
            Debug.LogWarning("No enemies configured for overworld.");
            return;
        }

        GameSession.Instance.SelectEnemy(enemy.id);
        SceneManager.LoadScene(battleSceneName);
    }

    private EnemyData ChooseEnemy()
    {
        if (possibleEnemies == null || possibleEnemies.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, possibleEnemies.Count);
        return possibleEnemies[index];
    }

    private void HandleHealNode()
    {
        if (GameSession.Instance == null)
        {
            return;
        }

        GameSession.Instance.RestoreHPToMax();
        Debug.Log("Healed to full HP.");
    }

    private void HandleShopNode()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        Debug.Log("Shop opened (stub).");
    }

    private void HandleExitNode()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.floorIndex++;
            Debug.Log($"Floor advanced to {GameSession.Instance.floorIndex}");
        }

        GenerateAndRenderNewMap();
    }
}

