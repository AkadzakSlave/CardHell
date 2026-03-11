using UnityEngine;
using UnityEngine.UI;

public class NodeView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    [Header("Visuals")]
    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color healColor = Color.cyan;
    [SerializeField] private Color shopColor = Color.yellow;
    [SerializeField] private Color exitColor = Color.magenta;

    private int nodeId;
    private NodeType nodeType;
    private OverworldController controller;

    public int NodeId => nodeId;
    public NodeType NodeType => nodeType;

    public void Initialize(int id, NodeType type, OverworldController overworldController)
    {
        nodeId = id;
        nodeType = type;
        controller = overworldController;

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }

        ApplyColor(type);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    private void ApplyColor(NodeType type)
    {
        if (iconImage == null)
        {
            return;
        }

        switch (type)
        {
            case NodeType.Start:
                iconImage.color = startColor;
                break;
            case NodeType.Enemy:
                iconImage.color = enemyColor;
                break;
            case NodeType.Heal:
                iconImage.color = healColor;
                break;
            case NodeType.Shop:
                iconImage.color = shopColor;
                break;
            case NodeType.Exit:
                iconImage.color = exitColor;
                break;
        }
    }

    private void OnClicked()
    {
        if (controller != null)
        {
            controller.OnNodeClicked(nodeId);
        }
    }
}

