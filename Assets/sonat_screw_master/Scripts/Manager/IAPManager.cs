using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class IAPManager
{
    public static ShopItemKey packClicked;
    public static ShopItemKey packBuySuccess = ShopItemKey.None;

    public static Action OnPurchasedFailedAction;

    public static void Buy(ShopItemKey shopItemKey, Action onSuccess, EPlacement ePlacement = EPlacement.None)
    {
        packClicked = shopItemKey;

       
    }

    public static void Restore(Action callback = null)
    {
#if UNITY_ANDROID      
        MyRestore();

        callback?.Invoke();

#elif UNITY_IOS
        Kernel.Resolve<BasePurchaser>().RestorePurchasesIOS(() =>
        {
            MyRestore();

            callback?.Invoke();
        });
#endif
    }

    public static void GetListProductFromStore()
    {
    }

    private static void Claim(ShopItemKey shopItemKey, bool isRestore = false, EPlacement ePlacement = EPlacement.None)
    {
        
    }

    public static void Claim(List<Reward> rewards, string source)
    {
       
    }

    private static void MyRestore()
    {
       
    }

    private static bool CheckFirstBuy(RewardID type)
    {


        return true;
    }

    public static bool HasPurchasedPack(ShopItemKey key)
    {
#if UNITY_EDITOR

            return false;

#else
        List<int> restoredPacks = DataManager.Instance.playerData.restoredPacks;

        if (restoredPacks.Contains((int)key) || packBuySuccess == key)
        {
            Debug.Log(key + " has been purchased");
            return true;
        }
        else
        {
            Debug.Log(key + " hasn't been purchased");
            return false;
        }
#endif
    }

    public static bool HasPurchasedPack(int key)
    {
#if UNITY_EDITOR

            return false;

#else
        List<int> restoredPacks = DataManager.Instance.playerData.restoredPacks;

        if (restoredPacks.Contains(key) || packBuySuccess == (ShopItemKey)key)
        {
            Debug.Log(key + " has been purchased");
            return true;
        }
        else
        {
            Debug.Log(key + " hasn't been purchased");
            return false;
        }
#endif
    }

    public static bool HasPurchasedNoAds()
    {
        return false;
    }
}

public enum ShopItemKey
{
    None = -1,
    NoAds = 0,
    NoAds24h = 1,
    NoAdsJustFun = 17,
    StarterBundle = 2,
    MiniBundle = 3,
    ExtraBundle = 4,
    ProBundle = 5,
    GiantBundle = 6,
    LegendaryBundle = 7,
    SafetyBundle = 15,
    HardLevelOffer = 16,
    Coin250 = 8,
    Coin750 = 9,
    Coin1600 = 10,
    Coin5000 = 11,
    Coin12500 = 12,
    Coin30000 = 13,
    LiveOffer = 14,
    ValueBundle = 18,
    BoosterOffer = 19,
    BestValueBundle = 20,
    ExclusiveDeal = 21,
    WeekendSale = 22,
    PlusOffer = 23,
    StandardBundle = 24
}

public enum EPlacement
{
    None = -1,
    Widget = 0,
    Popup = 1,
    Shop = 2,
}