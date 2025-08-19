using UnityEngine;

public class AutoDisableUI : MonoBehaviour
{
    public float activeTime = 4f;       // How long the object stays active
    public float floatSpeed = 30f;      // Pixels per second to move upwards
    public Vector3 worldFloatOffset = new Vector3(0, 1f, 0); // For world-space UI

    private RectTransform rectTransform;
    private Vector3 initialPos;   // Where it should reset to each time

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
            initialPos = rectTransform.anchoredPosition; // Save UI start position
        else
            initialPos = transform.position;             // Save world-space start pos
    }

    private void OnEnable()
    {
        // Reset position back to the initial starting point
        if (rectTransform != null)
            rectTransform.anchoredPosition = initialPos;
        else
            transform.position = initialPos;

        StartCoroutine(FloatAndDisable());
    }

    private System.Collections.IEnumerator FloatAndDisable()
    {
        float elapsed = 0f;

        while (elapsed < activeTime)
        {
            elapsed += Time.deltaTime;

            if (rectTransform != null) // UI (Canvas space)
            {
                rectTransform.anchoredPosition = initialPos + Vector3.up * floatSpeed * elapsed;
            }
            else // World-space object
            {
                transform.position = initialPos + worldFloatOffset * (elapsed / activeTime);
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
