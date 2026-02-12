using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Image enemyImage;

    private Color originalColor = Color.white;
    private Coroutine embarrassCoroutine;

    private void Awake()
    {
        if (enemyImage == null)
        {
            enemyImage = GetComponent<Image>();
        }

        if (enemyImage != null)
        {
            originalColor = enemyImage.color;
        }
    }

    public void ShowEmbarrassed(float seconds)
    {
        if (enemyImage == null)
        {
            Debug.LogWarning("EnemyVisual: enemyImage is not assigned.");
            return;
        }

        if (embarrassCoroutine != null)
        {
            StopCoroutine(embarrassCoroutine);
        }

        embarrassCoroutine = StartCoroutine(EmbarrassRoutine(Mathf.Max(0f, seconds)));
    }

    private IEnumerator EmbarrassRoutine(float seconds)
    {
        enemyImage.color = Color.green;
        yield return new WaitForSeconds(seconds);
        enemyImage.color = originalColor;
        embarrassCoroutine = null;
    }
}
