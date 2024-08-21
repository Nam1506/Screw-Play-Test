using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionReceive : ActionScript
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI parentAmountText;
    [SerializeField] private Image icon;
    [SerializeField] private Image parentIcon;

    public float timeMove = 0.5f;
    public float timeFade = 0.5f;
    public float moveY = 0.7f;

    private void Awake()
    {
        gameObject.SetActive(false);

        amountText.text = parentAmountText.text;
        icon.sprite = parentIcon.sprite;
        icon.SetNativeSize();
    }

    protected override void DoAction()
    {
        base.DoAction();

        Vector3 prePos = transform.position;

        gameObject.SetActive(true);

        canvasGroup.alpha = 1.0f;

        canvasGroup.DOFade(0f, timeFade).SetEase(Ease.Linear).SetDelay(timeMove / 2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.position = prePos;
        });

        transform.DOMoveY(transform.position.y + moveY, timeMove).SetEase(Ease.Linear);
    }
}
