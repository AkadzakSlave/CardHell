using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform container;
    [SerializeField] private NodeView nodeButtonPrefab;
    [SerializeField] private Image linePrefab;

    private readonly List<NodeView> activeNodes = new List<NodeView>();
    private readonly List<Image> activeLines = new List<Image>();

    public RectTransform Container => container;

    public void Clear()
    {
        for (int i = 0; i < activeNodes.Count; i++)
        {
            if (activeNodes[i] != null)
            {
                Destroy(activeNodes[i].gameObject);
            }
        }

        for (int i = 0; i < activeLines.Count; i++)
        {
            if (activeLines[i] != null)
            {
                Destroy(activeLines[i].gameObject);
            }
        }

        activeNodes.Clear();
        activeLines.Clear();
    }

    public void Render(MapGraph graph, OverworldController controller)
    {
        if (container == null || nodeButtonPrefab == null || linePrefab == null || graph == null)
        {
            return;
        }

        Clear();

        // Create node views
        Dictionary<int, NodeView> nodeViews = new Dictionary<int, NodeView>();

        Rect rect = container.rect;
        Vector2 size = rect.size;
        Vector2 origin = new Vector2(rect.xMin, rect.yMin);

        for (int i = 0; i < graph.nodes.Count; i++)
        {
            NodeData node = graph.nodes[i];
            NodeView view = Instantiate(nodeButtonPrefab, container);
            activeNodes.Add(view);

            // positions are normalized 0..1, map to container local coordinates
            Vector2 localPos = new Vector2(
                origin.x + (node.position.x * size.x),
                origin.y + (node.position.y * size.y));

            RectTransform rt = view.GetComponent<RectTransform>();
            rt.anchoredPosition = localPos;

            view.Initialize(node.id, node.type, controller);

            nodeViews[node.id] = view;
        }

        // Create lines between nodes
        foreach (NodeData node in graph.nodes)
        {
            if (!nodeViews.TryGetValue(node.id, out NodeView fromView))
            {
                continue;
            }

            RectTransform fromRt = fromView.GetComponent<RectTransform>();
            Vector2 fromPos = fromRt.anchoredPosition;

            for (int i = 0; i < node.nextNodeIds.Count; i++)
            {
                int targetId = node.nextNodeIds[i];
                if (!nodeViews.TryGetValue(targetId, out NodeView toView))
                {
                    continue;
                }

                RectTransform toRt = toView.GetComponent<RectTransform>();
                Vector2 toPos = toRt.anchoredPosition;

                Image line = Instantiate(linePrefab, container);
                activeLines.Add(line);

                RectTransform lineRt = line.rectTransform;

                Vector2 midPoint = (fromPos + toPos) * 0.5f;
                lineRt.anchoredPosition = midPoint;

                Vector2 dir = (toPos - fromPos);
                float length = dir.magnitude;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                lineRt.sizeDelta = new Vector2(length, lineRt.sizeDelta.y);
                lineRt.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}

