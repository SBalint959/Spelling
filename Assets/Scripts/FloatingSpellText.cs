using UnityEngine;
using TMPro;

public class FloatingSpellText : MonoBehaviour
{
    public float floatSpeed = 20f;
    public float duration = 2f;
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
    }

    public void SetText(string text, Color color)
    {
        if (textComponent == null) textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = color;
    }
}
