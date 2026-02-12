using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private Color embarrassedTint = Color.green;

    private Color originalColor = Color.white;
    private Coroutine tintRoutine;

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
            return;
        }

        if (tintRoutine != null)
        {
            StopCoroutine(tintRoutine);
        }

        tintRoutine = StartCoroutine(EmbarrassedRoutine(seconds));
    }

    private IEnumerator EmbarrassedRoutine(float seconds)
    {
        enemyImage.color = embarrassedTint;
        yield return new WaitForSeconds(Mathf.Max(0f, seconds));
        enemyImage.color = originalColor;
        tintRoutine = null;
    }
}
