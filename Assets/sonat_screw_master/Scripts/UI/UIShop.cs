using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public static event EventHandler OnOpenShopAction;
    public static event EventHandler OnCloseShopAction;

    [SerializeField] private Button backBtn;
    [SerializeField] private List<UIShopPack> uiShopPacks;
    [SerializeField] private ShopPackSO shopPackSO;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform coinTitle;
    [SerializeField] private RectTransform liveTitle;

    [SerializeField] private RectTransform barPos;

    private bool preStateCoin;
    private bool preStateLive;
    private float time;

    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            Active(false, preStateCoin, preStateLive);

            PopupManager.Instance.ShowPrePopup(null);
        });

        //UIManager.Instance.CheckLimitPacks();
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void OnEnable()
    {
        backBtn.gameObject.SetActive(GameManager.Instance.GameState == GameState.Ingame);

        if (GameManager.Instance.GameState == GameState.Home)
        {
            //HomeController.Instance.SetPanelBotLayer(1);

            barPos.anchoredPosition = Vector2.zero;
        }
        else
        {
            barPos.anchoredPosition = new Vector2(450f, 0f);
        }

        scroll.verticalNormalizedPosition = 1;

        OnOpenShopAction?.Invoke(this, EventArgs.Empty);

        time = 0f;
    }

    private void OnDisable()
    {
        //if (GameManager.Instance.GameState == GameState.Home)
        //{
        //    HomeController.Instance.SetPanelBotLayer(0);
        //}

        barPos.anchoredPosition = Vector2.zero;

        OnCloseShopAction?.Invoke(this, EventArgs.Empty);

        SonatTracking.LogSpentTime("shop", "shop", GameManager.Instance.eScreen.ToString(), time);
    }

    public void Active(bool isActive, bool preStateCoin = false, bool preStateLive = false)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
            this.preStateCoin = preStateCoin;
            this.preStateLive = preStateLive;
        }
        else
        {
            gameObject.SetActive(false);

            UIManager.Instance.HideCoin(false);
            UIManager.Instance.HideLive(false);

            if (!this.preStateCoin)
            {
                UIManager.Instance.HideCoin(false);
            }

            if (!this.preStateLive)
            {
                UIManager.Instance.HideLive(false);
            }

            if (!this.preStateCoin && !this.preStateLive)
            {
                GameplayManager.Instance.GameplayState = GameplayState.Playing;
            }
        }
    }

    public ShopPack GetShopPackData(ShopItemKey shopItemKey)
    {
        return shopPackSO.GetShopPack(shopItemKey);
    }

    public ShopPack GetShopPackData(int shopItemKey)
    {
        return shopPackSO.GetShopPack((ShopItemKey)shopItemKey);
    }

    public UIShopPack GetShopPack(ShopItemKey shopItemKey)
    {
        return uiShopPacks.Find(p => p.key == shopItemKey);
    }

    public void RemovePack(ShopItemKey shopItemKey)
    {
        foreach (UIShopPack pack in uiShopPacks)
        {
            if (pack.key == shopItemKey)
            {
                pack.gameObject.SetActive(false);
            }
        }
    }

    public void ScrollTo(EContent content)
    {
        if (content == EContent.Coin)
        {
            scroll.verticalNormalizedPosition = 1;

            scroll.DOVerticalNormalizedPos(0, 0.8f).SetEase(Ease.InOutQuad);
        }
        else if (content == EContent.Live)
        {
            scroll.content.DOAnchorPos(new Vector2(0, -liveTitle.anchoredPosition.y), 0.4f);
        }
    }

    public enum EContent
    {
        None,
        Coin,
        Live
    }
}
