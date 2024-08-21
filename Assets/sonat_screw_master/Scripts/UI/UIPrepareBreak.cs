using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrepareBreak : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private RectTransform rect;

    [SerializeField] private Transform close;
    [SerializeField] private TextMeshProUGUI guide;

    private Color preColor;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BoosterManager.Instance.boosterHammer.UnPrepare();
        });

        preColor = guide.color;
    }

    private void OnEnable()
    {
        rect.sizeDelta = new Vector2(210f, 180f);
        rect.DOSizeDelta(new Vector2(670f, 180f), 0.5f).OnKill(() =>
        {
            rect.sizeDelta = new Vector2(670f, 180f);
        });

        guide.color = preColor;
        guide.color.SetAlpha(0f);

        guide.DOColor(preColor, 0.5f).OnKill(() =>
        {
            guide.color = preColor;
        });

        close.localScale = Vector3.zero;
        close.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f).OnKill(() =>
        {
            close.localScale = Vector3.one;
        });
    }

    private void OnDisable()
    {
        rect.DOKill();
        guide.DOKill();
        close.DOKill();
    }
}
