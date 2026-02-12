using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    public BoxCollider2D arenaCollider;

    private void Awake()
    {
        if (arenaCollider == null)
        {
            arenaCollider = GetComponent<BoxCollider2D>();
        }
    }

    public Rect GetWorldRect()
    {
        if (arenaCollider == null)
        {
            Debug.LogWarning("ArenaBounds: arenaCollider is not assigned.");
            return new Rect();
        }

        Bounds b = arenaCollider.bounds;
        return Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
    }

    public bool Contains(Vector2 worldPos)
    {
        return GetWorldRect().Contains(worldPos);
    }
}
