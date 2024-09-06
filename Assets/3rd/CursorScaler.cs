using UnityEngine;
using UnityEngine.UI;

public class CursorScaler : MonoBehaviour
{
    public Image customCursor;
    private Vector3 originalScale;

    public float clickScale;

    void Start()
    {
        originalScale = GetComponent<RectTransform>().localScale;
    }

    void Update()
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }

        Vector3 cursorPosition = Input.mousePosition;
        Vector2 screenPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            customCursor.canvas.transform as RectTransform, cursorPosition,
            customCursor.canvas.worldCamera, out screenPosition
        );

        customCursor.rectTransform.anchoredPosition = screenPosition;

        if (Input.GetMouseButton(0))
        {
            customCursor.rectTransform.localScale = originalScale * clickScale;
        }
        else
        {
            customCursor.rectTransform.localScale = originalScale;
        }
    }
}
