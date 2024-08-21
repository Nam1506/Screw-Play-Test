using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject hard;
    [SerializeField] private GameObject superHard;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button shopBtn;
    [SerializeField] private Button closeNoAds;
    [SerializeField] private GameObject plusOffer;
    [SerializeField] private GameObject exclusiveDeal;
    [SerializeField] private GameObject weekendSale;

    [SerializeField] private List<RectTransform> topTranforms;
    [SerializeField] private List<RectTransform> botTranforms;
    [SerializeField] private List<RectTransform> leftTranforms;

    [SerializeField] private CanvasGroup boxParent;

    private float duration = 0.3f;

    private void Awake()
    {
        settingBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.ShowSetting();
        });
        //noAdsBtn.onClick.AddListener(() =>
        //{
        //    PopupManager.Instance.ShowNoAds();
        //});
        shopBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowShop();
        });
        closeNoAds.onClick.AddListener(() =>
        {
            //PopupManager.Instance.ShowNoAds();
            PopupManager.Instance.ShowJustFun(false);
        });
    }

    private void Start()
    {
        CheckBannerHeight();
    }

    private void OnEnable()
    {
        CheckPacks();
    }

    public void CheckPacks()
    {
        plusOffer.SetActive(false);
        exclusiveDeal.SetActive(false);
        weekendSale.SetActive(false);

        //if (UIManager.Instance.IsShowPlusOffer())
        //{
        //    plusOffer.SetActive(true);
        //}
        //else if (UIManager.Instance.IsShowExclusive())
        //{
        //    exclusiveDeal.SetActive(true);
        //}
        //else if (UIManager.Instance.IsShowWeekendSale())
        //{
        //    weekendSale.SetActive(true);
        //}
    }

    public void ShowUI()
    {
        SlideIn();
    }

    public void HideUI(bool isSlide)
    {
        if (isSlide)
        {
            SlideOut();
        }
        else
        {
            topTranforms.SetActiveAll(false);
            leftTranforms.SetActiveAll(false);
            botTranforms.SetActiveAll(false);
            boxParent.gameObject.SetActive(false);
        }
    }

    private void SlideIn()
    {
        boxParent.DOKill();
        boxParent.alpha = 0f;
        boxParent.gameObject.SetActive(true);
        boxParent.DOFade(1f, duration);

        foreach (RectTransform transform in topTranforms)
        {
            transform.DOKill();

            transform.gameObject.SetActive(true);

            transform.anchoredPosition = new Vector2(0, transform.rect.height);
            transform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack);
        }

        foreach (RectTransform transform in botTranforms)
        {
            transform.DOKill();
            transform.gameObject.SetActive(true);

            transform.anchoredPosition = new Vector2(0, -(transform.rect.height));
            transform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutQuart);
        }

        foreach (RectTransform transform in leftTranforms)
        {
            //if (Kernel.Resolve<AdsManager>().IsNoAds()) return;

            transform.DOKill();
            transform.gameObject.SetActive(true);

            float preX = transform.anchoredPosition.x;

            transform.anchoredPosition = new Vector2(-preX, transform.anchoredPosition.y);
            transform.DOAnchorPos(new Vector2(preX, transform.anchoredPosition.y), duration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                //noAdsAnim.AnimationState.SetAnimation(0, NOADS_ANIM_NAME, true);
            });
        }
    }

    private void SlideOut()
    {
        boxParent.DOKill();
        boxParent.DOFade(0f, duration);

        foreach (RectTransform transform in topTranforms)
        {
            transform.DOKill();
            transform.anchoredPosition = Vector2.zero;
            transform.DOAnchorPos(new Vector2(0, transform.rect.height + 175f), duration).OnComplete(() =>
            {
                transform.anchoredPosition = Vector2.zero;
                transform.gameObject.SetActive(false);
            });
        }

        foreach (RectTransform transform in botTranforms)
        {
            transform.DOKill();
            transform.anchoredPosition = Vector2.zero;
            transform.DOAnchorPos(new Vector2(0, -(transform.rect.height + 50f)), duration).OnComplete(() =>
            {
                transform.anchoredPosition = Vector2.zero;
                transform.gameObject.SetActive(false);
            });
        }

        foreach (RectTransform transform in leftTranforms)
        {
            transform.DOKill();
            float preX = transform.anchoredPosition.x;

            transform.anchoredPosition = new Vector2(preX, transform.anchoredPosition.y);
            transform.DOAnchorPos(new Vector2(-preX, transform.anchoredPosition.y), duration).OnComplete(() =>
            {
                transform.anchoredPosition = new Vector2(preX, transform.anchoredPosition.y);
                transform.gameObject.SetActive(false);
            });
        }
    }

    public void UpdateLevelText()
    {
        levelText.text = "LEVEL " + DataManager.Instance.playerData.saveLevelData.currentLevel;

        EDifficulty difficulty = GameplayManager.Instance.difficulty;

        switch (difficulty)
        {
            case EDifficulty.Normal:
                hard.SetActive(false);
                superHard.SetActive(false);
                break;
            case EDifficulty.Hard:
                hard.SetActive(true);
                superHard.SetActive(false);
                break;
            case EDifficulty.Super_Hard:
                hard.SetActive(false);
                superHard.SetActive(true);
                break;
        }
    }

    private void CheckBannerHeight()
    {
        float ratio = (float)Screen.width / Screen.height;

        Debug.Log("ratio: " + ratio);

        if (ratio > 0.6)
        {
            botTranforms[0].sizeDelta = new Vector2(0, 460f);
        }
        else
        {
            botTranforms[0].sizeDelta = new Vector2(0, 430f);
        }
    }

    public void ActiveCloseNoAds(bool isActive)
    {
        closeNoAds.transform.parent.gameObject.SetActive(isActive);
    }
}
