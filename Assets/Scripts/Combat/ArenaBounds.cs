using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    [SerializeField] private BoxCollider2D arenaCollider;

    private void Reset()
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
            return new Rect(transform.position, Vector2.zero);
        }

        Bounds bounds = arenaCollider.bounds;
        return Rect.MinMaxRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
    }

    public bool Contains(Vector2 worldPos)
    {
        return GetWorldRect().Contains(worldPos);
    }
}
