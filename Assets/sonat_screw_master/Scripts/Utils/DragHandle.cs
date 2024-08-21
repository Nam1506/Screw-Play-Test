using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandle : MonoBehaviour, IBeginDragHandler
{
    [SerializeField] private ScrollRect kill;
    public void OnBeginDrag(PointerEventData eventData)
    {
        kill.DOKill();
    }
}
