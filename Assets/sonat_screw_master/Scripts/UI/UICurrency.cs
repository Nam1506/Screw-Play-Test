using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI valueText;
    [SerializeField] protected GameObject moreIcon;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Canvas canvas;
    [SerializeField] private Button moreBtn;
    [SerializeField] private Image icon;

    private void Awake()
    {
        if (moreBtn != null)
        {
            moreBtn.onClick.AddListener(DoMoreAction);
        }
    }

    protected virtual void OnDisable()
    {
        UIShop.OnOpenShopAction -= UIShop_OnOpenShopAction;
        UIShop.OnCloseShopAction -= UIShop_OnCloseShopAction;
    }

    protected virtual void OnEnable()
    {
        if (UIManager.Instance.IsShowingShop)
            ActiveMoreIcon(false);

        UIShop.OnOpenShopAction += UIShop_OnOpenShopAction;
        UIShop.OnCloseShopAction += UIShop_OnCloseShopAction;
    }

    protected virtual void UIShop_OnCloseShopAction(object sender, System.EventArgs e)
    {
        ActiveMoreIcon(true);
    }

    protected virtual void UIShop_OnOpenShopAction(object sender, System.EventArgs e)
    {
        ActiveMoreIcon(false);
    }

    public void ActiveMoreIcon(bool isActive)
    {
        moreIcon.SetActive(isActive);
        moreBtn.interactable = isActive;
    }

    public Vector3 GetIconPos()
    {
        return icon.transform.position;
    }

    public virtual void SetText(int value)
    {
        if (value >= 0)
        {
            valueText.text = value.ToString();
        }
    }

    public virtual void TweenText(int currentValue, int finalValue, float duration)
    {
        StartCoroutine(IEIncreaseAnyReference(currentValue, finalValue, duration));
    }

    private IEnumerator IEIncreaseAnyReference( float currentValue, float finalValue, float duration)
    {
        float elapsed = 0f;
        float process;

        float initValue = currentValue;

        while (elapsed <= duration)
        {
            process = Mathf.Clamp01(elapsed / duration);
            currentValue = initValue + process * (finalValue - initValue);

            valueText.text = Mathf.RoundToInt(currentValue).ToString();

            elapsed += Time.deltaTime;
            yield return null;
        }

        valueText.text = Mathf.RoundToInt(finalValue).ToString();
    }

    protected virtual void DoMoreAction()
    {
        
    }

    public virtual void Hide(bool isFade = true)
    {
        if (isFade)
        {
            canvasGroup.DOFade(0, PopupConfig.TIME_FADE_MASK).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void Show(bool isFade = true)
    {
        if (gameObject.activeInHierarchy) return;

        gameObject.SetActive(true);

        if (isFade)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, PopupConfig.TIME_FADE_MASK);
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    public void ChangeSortingLayer(int layer)
    {
        canvas.sortingOrder = layer;
    }
}
