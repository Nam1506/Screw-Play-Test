using UnityEngine;
using System;
using DG.Tweening;
using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public class UIManager : SingletonBase<UIManager>
{
    [SerializeField] private Transform rewardPrefab;
    [SerializeField] private Transform coinPrefab;
    [SerializeField] private UIIngame uIIngame;
    [SerializeField] private UIPrepareBreak uiPrepare;
    [SerializeField] private UICoin uiCoin;
    [SerializeField] private UILive uiLive;
    [SerializeField] private UIShop uiShop;
    [SerializeField] private UIHome uiHome;
    [SerializeField] private UICollection uiCollection;

    [SerializeField] private RewardSO rewardSO;

    [Header("No Ads")]
    [SerializeField] private GameObject[] noAds;

    [Header("Widget")]
    [SerializeField] private WidgetNoAds24h noAds24H;
    [SerializeField] private WidgetValueBundle valueBundle;
    [SerializeField] private WidgetBestValueBundle bestValueBundle;
    [SerializeField] private GameObject noAdsWidget;
    [SerializeField] private WidgetExclusive exclusiveDeal;
    [SerializeField] private WidgetWeekend weekendSale;
    [SerializeField] private WidgetStarter starterPack;
    [SerializeField] private WidgetPlusOffer plusOffer;
    [SerializeField] private WidgetLevelChest levelChestWidget;
    [SerializeField] private WidgetRace raceWidget;

    public List<Tween> tweens = new();

    public UIIngame UIIngame => uIIngame;
    public UIShop UIShop => uiShop;
    public UIHome UIHome => uiHome;
    public UILive UILive => uiLive;
    public UICoin UICoin => uiCoin;
    public UICollection UICollection => uiCollection;

    public WidgetLevelChest LevelChest => levelChestWidget;
    public WidgetRace Race => raceWidget;

    public bool IsShowingShop => uiShop.gameObject.activeInHierarchy;

    private bool isCollectingCoin;

    private void Start()
    {
        CheckNoAds();
        CheckLimitPacks();
    }

    public void CheckNoAds()
    {
        Debug.Log("check no ads");

        CheckNoAds24h();


    }

    public void CheckWidgets()
    {
        Debug.Log("check widgets");

       
        //CheckLevelChest();
    }

    public void CheckWeekendSale()
    {
        if (IsShowExclusive())
        {
            exclusiveDeal.gameObject.SetActive(true);
            weekendSale.gameObject.SetActive(false);
        }
        else if (IsShowWeekendSale())
        {
            exclusiveDeal.gameObject.SetActive(false);
            weekendSale.gameObject.SetActive(true);
        }
        else
        {
            exclusiveDeal.gameObject.SetActive(false);
            weekendSale.gameObject.SetActive(false);
        }

        uIIngame.CheckPacks();
    }

    private void CheckWidget24h()
    {
        if (!noAds24H.HasPurchased())
        {
            if (!noAds24H.IsEnded())
            {
                noAds24H.gameObject.SetActive(true);
                valueBundle.gameObject.SetActive(false);
                bestValueBundle.gameObject.SetActive(false);
                noAdsWidget.SetActive(false);
            }
            else
            {
                noAds24H.gameObject.SetActive(false);

                CheckWidgetValue();
            }
        }
        else
        {
            noAds24H.gameObject.SetActive(false);

            bool isNextDay = Helper.StringToDate(DataManager.Instance.playerData.timeBuyNoAds24h).AddDays(1).Date <= DateTime.Now.Date;

            if (isNextDay)
            {
                CheckWidgetValue();
            }
            else
            {
                valueBundle.gameObject.SetActive(false);
                bestValueBundle.gameObject.SetActive(false);
                noAdsWidget.SetActive(false);
            }
        }
    }

    private void CheckWidgetValue()
    {
        if (!valueBundle.HasPurchased())
        {
            if (!valueBundle.IsEnded())
            {
                valueBundle.gameObject.SetActive(true);
                bestValueBundle.gameObject.SetActive(false);
                noAdsWidget.SetActive(false);
            }
            else
            {
                CheckWidgetBestValue();
            }
        }
        else
        {
            valueBundle.gameObject.SetActive(false);

            bool isNextDay = Helper.StringToDate(DataManager.Instance.playerData.timeBuyNoAds24h).AddDays(1).Date <= DateTime.Now.Date;

            if (isNextDay)
            {
                CheckWidgetBestValue();
            }
            else
            {
                bestValueBundle.gameObject.SetActive(false);
            }
        }
    }

    private void CheckWidgetBestValue()
    {
        if (!bestValueBundle.HasPurchased())
        {
            if (!bestValueBundle.IsEnded())
            {
                bestValueBundle.gameObject.SetActive(true);
                noAdsWidget.SetActive(false);

                if (!string.IsNullOrEmpty(DataManager.Instance.playerData.timeEndBestValueBundle))
                {
                    DataManager.Instance.playerData.timeEndBestValueBundle = null;
                    DataManager.Instance.Save();
                }
            }
            else
            {
                bestValueBundle.gameObject.SetActive(false);

                if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeEndBestValueBundle))
                {
                    DataManager.Instance.playerData.timeEndBestValueBundle = Helper.DateTimeToString(DateTime.Now);
                    DataManager.Instance.Save();
                }
                else
                {
                    bool isNextDay = Helper.StringToDate(DataManager.Instance.playerData.timeEndBestValueBundle).AddDays(1).Date <= DateTime.Now.Date;

                    if (isNextDay)
                    {
                        Debug.Log("abc reset pack timer");

                        DataManager.Instance.ResetPackTimer();

                        CheckWidgetBestValue();
                    }
                }
            }
        }
        else
        {
            bestValueBundle.gameObject.SetActive(false);
            noAdsWidget.SetActive(false);
        }
    }

    public void CheckStarterPack()
    {
        if (IsShowStarterPack())
        {
            starterPack.gameObject.SetActive(true);
        }
        else
        {
            starterPack.gameObject.SetActive(false);
        }
    }

    public void CheckPlusOffer()
    {
        if (IsShowPlusOffer())
        {
            plusOffer.gameObject.SetActive(true);
        }
        else
        {
            plusOffer.gameObject.SetActive(false);

            uIIngame.CheckPacks();
        }
    }

    public bool IsShowNoAds24h()
    {
        return !noAds24H.HasPurchased() && !noAds24H.IsEnded();
    }

    public bool IsShowValueBundle()
    {
        bool isShowNoAds24h = IsShowNoAds24h();
        return !isShowNoAds24h && !valueBundle.IsEnded() && !valueBundle.HasPurchased();
    }

    public bool IsShowBestValueBundle()
    {
        //return (valueBundle.HasPurchased() || valueBundle.IsEnded()) && !bestValueBundle.IsEnded() && !bestValueBundle.HasPurchased();
        return /*(valueBundle.HasPurchased() || valueBundle.IsEnded()) &&*/ !bestValueBundle.IsEnded() && !bestValueBundle.HasPurchased();
    }

    public bool IsShowExclusive()
    {
        return !exclusiveDeal.IsEnded() && !exclusiveDeal.IsBuyThisWeek();
    }

    public bool IsShowWeekendSale()
    {
        return !weekendSale.IsEnded() && !weekendSale.IsBuyThisWeek();
    }

    public bool IsShowStarterPack()
    {
        

        List<int> restoredPacks = DataManager.Instance.playerData.restoredPacks;

        if (restoredPacks.Contains((int)ShopItemKey.StarterBundle))
        {
            return false;
        }
        else
        {
            return true;
        }

        //if (Kernel.Resolve<BasePurchaser>().CheckHasPurchasedProductId((int)ShopItemKey.StarterBundle))
        //{
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }

    public bool IsShowPlusOffer()
    {
        return !plusOffer.IsEnded() && !plusOffer.IsBuyThisWeek();
    }

    public void ClaimBooster(EBoosterType boosterType, int amount)
    {
        PopupManager.OnSetInteractableMask?.Invoke(true);

        BoosterInfo info = BoosterManager.Instance.boosterSO.boosters.Find(p => p.type == boosterType);

        if (info == null) return;

        UIReward spawned = PoolBoss.Spawn(rewardPrefab, Vector3.zero, Quaternion.identity, CanvasManager.Instance.canvaPopup.transform).GetComponent<UIReward>();

        spawned.Init(info.bigIcon, amount, false);

        SoundManager.Instance.PlaySound(KeySound.Booster_Appear);

        BoosterBase booster = BoosterManager.Instance.GetBooster(boosterType);

        spawned.PlayClaimEffect(booster.transform, () =>
        {
            booster.PlayUnlockEffect();

            booster.UnlockVisual();

            SoundManager.Instance.PlaySound(KeySound.Booster_Receive);

            PopupManager.Instance.ShowForceBooster(boosterType);

            //GameplayManager.Instance.GameplayState = GameplayState.Playing;

            //PopupManager.OnSetInteractableMask?.Invoke(false);
        });
    }

    public void ClaimReward(Reward reward, Transform from, Transform to, float delay = 0)
    {
        PopupManager.OnSetInteractableMask?.Invoke(true);

        UIReward spawned = PoolBoss.Spawn(rewardPrefab, from.position, Quaternion.identity, CanvasManager.Instance.canvaPopup.transform).GetComponent<UIReward>();
        spawned.transform.localScale = Vector3.zero;

        spawned.transform.DOScale(Vector3.one, 0.2f);

        if (reward.type == RewardType.booster || reward.type == RewardType.currency)
        {
            Debug.Log(reward.id);

            Sprite icon = rewardSO.GetRewardSprite(reward.id);

            if (reward.id == RewardID.unlimitedLives)
            {
                spawned.Init(icon, reward.time, false);
            }
            else
            {
                spawned.Init(icon, reward.amount, false);
            }
        }
        else return;

        SoundManager.Instance.PlaySound(KeySound.Booster_Appear);

        if (delay > 0)
        {
            bool preState = uiLive.gameObject.activeInHierarchy;

            if (reward.id == RewardID.unlimitedLives)
            {
                ShowLive(false);
            }

            DOVirtual.DelayedCall(delay, () =>
            {
                spawned.PlayClaimEffect(to, () =>
                {
                    SoundManager.Instance.PlaySound(KeySound.Booster_Receive);

                    EffectManager.Instance.PlayReceiveEffect(to.position);

                    if (tweens.Count == 0 && !isCollectingCoin)
                    {
                        PopupManager.OnSetInteractableMask?.Invoke(false);
                    }

                    if (reward.id == RewardID.unlimitedLives || reward.id == RewardID.live)
                    {
                        Debug.Log("Match live");
                        DataManager.Instance.MatchLives();

                        if (!preState)
                        {
                            HideLive();
                        }
                    }

                    if (reward.type == RewardType.booster)
                    {
                        BoosterManager.Instance.MatchValue();
                    }
                });
            });
        }
        else
        {
            bool preState = uiLive.gameObject.activeInHierarchy;

            if (reward.id == RewardID.unlimitedLives)
            {
                ShowLive(false);
            }

            spawned.PlayClaimEffect(to, () =>
            {
                SoundManager.Instance.PlaySound(KeySound.Booster_Receive);

                if (tweens.Count == 0 && !isCollectingCoin)
                {
                    PopupManager.OnSetInteractableMask?.Invoke(false);
                }

                if (reward.id == RewardID.unlimitedLives || reward.id == RewardID.live)
                {
                    Debug.Log("Match live");
                    DataManager.Instance.MatchLives();

                    if (!preState)
                    {
                        HideLive();
                    }
                }
            });
        }
    }

    public bool IsReceivingRewards()
    {
        return tweens.Count > 0 || isCollectingCoin;
    }

    public void PlayTransition()
    {
        CanvasManager.Instance.canvaTransition.gameObject.SetActive(true);
    }

    public void PrepareHammer()
    {
        uiPrepare.gameObject.SetActive(true);

        foreach (var booster in BoosterManager.Instance.boosterBases)
        {
            booster.transform.DOScale(Vector3.zero, 0.5f);
        }
    }

    public void UnPrepareHammer()
    {
        uiPrepare.gameObject.SetActive(false);

        foreach (var booster in BoosterManager.Instance.boosterBases)
        {
            booster.transform.DOScale(Vector3.one, 0.5f);
        }
    }

    public void ShowCoin(bool isFade = true)
    {
        Debug.Log("ShowCoin");
        uiCoin.Show(isFade);
    }

    public void HideCoin(bool isFade = true)
    {
        uiCoin.Hide(isFade);
    }

    public void ShowLive(bool isFade = true)
    {
        Debug.Log("namtt ShowLive");
        uiLive.Show(isFade);
    }

    public void HideLive(bool isFade = true)
    {
        Debug.Log("namtt HideLive");
        uiLive.Hide(isFade);
    }

    public void ShowShop()
    {
        if (IsShowingShop) return;

        SonatTracking.LogOpenShop();

        switch (GameManager.Instance.GameState)
        {
            case GameState.Ingame:
                GameplayManager.Instance.GameplayState = GameplayState.Pausing;

                uiShop.Active(true, uiCoin.gameObject.activeInHierarchy, uiLive.gameObject.activeInHierarchy);
                ShowCoin();
                ShowLive();
                break;
            case GameState.Home:
                uiShop.Active(true, true, true);
                break;
        }
    }

    public void CollectCoins(int num, Action callback, Vector3 startPos, bool isShowMask = true, float _timeMove = 0.8f)
    {
        StartCoroutine(IeCollectCoins(num, callback, startPos, isShowMask, _timeMove));
    }

    public float timeReady;
    public float timeMove;
    public float timeGap;
    public float valueRandom;

    public float randomX;
    public float randomY;

    public float timeTween;
    public float timePerCoin;

    public Vector3 scaleEnd;

    private IEnumerator IeCollectCoins(int num, Action callback, Vector3 startPos, bool isShowMask = true, float _timeMove = 0.8f)
    {
        bool preState = uiCoin.gameObject.activeSelf;

        var preSortingOrder = uiCoin.GetComponent<Canvas>().sortingOrder;

        ShowCoin();

        if (preSortingOrder <= 0)
        {
            uiCoin.ChangeSortingLayer(1);
        }

        if (isShowMask)
            PopupManager.OnFadeOutSecondBlackMask();

        PopupManager.OnSetInteractableMask?.Invoke(true);

        isCollectingCoin = true;

        int coinFly = 0;

        bool isTweeningCoin = false;

        bool isSounding = false;

        while (num-- > 0)
        {
            coinFly++;

            var coin = PoolBoss.Spawn(coinPrefab, startPos, Quaternion.identity, uiCoin.transform);
            coin.localScale = Vector3.one * 2f;

            coin.SetAsFirstSibling();

            var posRandom = coin.transform.position + new Vector3(UnityEngine.Random.Range(-valueRandom, valueRandom), UnityEngine.Random.Range(-valueRandom, valueRandom), 0);

            coin.DOMove(posRandom, timeTween);

            yield return new WaitForSeconds(timePerCoin);

            float randomTime = UnityEngine.Random.Range(randomX, randomY);

            DOVirtual.DelayedCall(randomTime, () =>
            {
                coin.DOScale(scaleEnd, _timeMove);

                coin.transform.DOMove(uiCoin.GetIconPos(), _timeMove).SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        if (!isTweeningCoin)
                        {
                            DataManager.Instance.MatchCoins();
                            isTweeningCoin = true;
                        }

                        if (!isSounding)
                        {
                            SoundManager.Instance.PlaySound(KeySound.CoinsPickup);
                            isSounding = true;
                        }

                        uiCoin.GetComponent<ActionScript>()?.StartAction();
                        uiCoin.GetComponentInChildren<ParticleSystem>()?.Play();

                        coinFly--;

                        VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);

                        PoolBoss.Despawn(coin);
                    }).SetEase(Ease.InQuad);
            });
        }

        while (coinFly != 0) yield return null;

        callback?.Invoke();

        if (!preState)
        {
            HideCoin();
        }

        if (preSortingOrder <= 0)
        {
            uiCoin.ChangeSortingLayer(preSortingOrder);
        }


        if (isShowMask)
            PopupManager.OnFadeInSecondBlackMask();

        if (!PopupManager.Instance.IsWinning)
        {
            PopupManager.OnSetInteractableMask?.Invoke(false);
        }

        isCollectingCoin = false;
    }

    public float GetTimeCollectCoin(int num = 10, float _timeMove = 0.8f)
    {
        return timePerCoin * num + randomX + _timeMove;
    }

    public void CheckLimitPacks()
    {

        List<int> restoredPacks = DataManager.Instance.playerData.restoredPacks;

        if (restoredPacks.Contains((int)ShopItemKey.StarterBundle))
        {
            UIShopPack starterPack = uiShop.GetShopPack(ShopItemKey.StarterBundle);

            starterPack.gameObject.SetActive(false);
        }
    }

    public void CheckNoAds24h()
    {
        if (DataManager.Instance.IsNoAds24hExpired() && !IAPManager.HasPurchasedNoAds())
        {
            Debug.Log("NoAds24h was Expired");
        }
        //else
        //{
        //    if (IAPManager.HasPurchasedNoAds24h() && !string.IsNullOrEmpty(DataManager.Instance.playerData.timeBuyNoAds24h))
        //    {
        //        Kernel.Resolve<AdsManager>().EnableNoAds();
        //    }
        //}
    }

    public void SetCurrencyLayer(int sortingLayer)
    {
        uiCoin.ChangeSortingLayer(sortingLayer);
        uiLive.ChangeSortingLayer(sortingLayer);
    }

    public void SendCurrencyToFront()
    {
        SetCurrencyLayer(5);
    }

    public void SetCoinLayer(int sortingLayer)
    {
        uiCoin.ChangeSortingLayer(sortingLayer);
    }

    public void SendCurrencyToBack(bool isNegative = true)
    {
        if (GameManager.Instance.GameState == GameState.Home)
        {
            if (IsShowingShop)
            {
                SetCurrencyLayer(1);
            }
            else
            {
                SetCurrencyLayer(isNegative ? -1 : 0);
                //SetCurrencyLayer(0);
            }
        }
        else
        {
            SetCurrencyLayer(1);
        }
    }

    public void MatchAllProgress()
    {
        Race.MatchOnly();
        LevelChest.MatchOnly();
    }
}
