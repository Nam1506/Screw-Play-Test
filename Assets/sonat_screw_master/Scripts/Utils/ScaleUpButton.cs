using DG.Tweening;
using UnityEngine;

public class ScaleUpButton : MonoBehaviour
{
    public float duration;
    public float delay;
    public Ease ease = Ease.OutBack;
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;

    private void OnEnable()
    {
        transform.localScale = startScale;
        transform.DOScale(endScale, duration).SetDelay(delay).SetEase(ease).OnKill(() =>
        {
            transform.localScale = endScale;
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
