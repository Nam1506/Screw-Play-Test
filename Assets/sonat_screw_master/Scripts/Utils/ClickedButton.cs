using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickedButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button button;
    [SerializeField] private KeySound key;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(key);
    }
}
