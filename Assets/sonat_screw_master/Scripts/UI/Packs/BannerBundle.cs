using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

public class BannerBundle : UIShopPack
{
    [SerializeField] private PopupMoreBooster popupMoreBooster;
    [SerializeField] private SkeletonGraphic anim;

    private void OnEnable()
    {
        if (anim != null)
        {
            if (anim.Skeleton.Data.FindAnimation("Appear") != null)
            {
                anim.AnimationState.SetAnimation(0, "Appear", false);
                anim.AnimationState.AddAnimation(0, "Idle", true, 0);
            }
            else
            {
                anim.AnimationState.SetAnimation(0, "Idle", true);
            }
        }

        PopupReward.OnClaimRewardAction += PopupReward_OnClaimRewardAction;
    }

    private void OnDisable()
    {
        PopupReward.OnClaimRewardAction -= PopupReward_OnClaimRewardAction;
    }

    private void PopupReward_OnClaimRewardAction(object sender, EventArgs e)
    {
        if (!popupMoreBooster.IsPreBooster)
        {
            StartCoroutine(IeWaitForReward());
        }
    }

    private IEnumerator IeWaitForReward()
    {
        yield return new WaitForSeconds(0.2f);

        while (UIManager.Instance.IsReceivingRewards())
        {
            yield return null;
        }

        popupMoreBooster.Hide(true, false, true);
    }

    protected override void OnBuySuccess()
    {
        base.OnBuySuccess();

        if (key == ShopItemKey.ExclusiveDeal)
        {
            DataManager.Instance.playerData.timeBuyExclusive = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();

            UIManager.Instance.CheckWeekendSale();
        }
        else if (key == ShopItemKey.WeekendSale)
        {
            DataManager.Instance.playerData.timeBuyWeekendSale = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();

            UIManager.Instance.CheckWeekendSale();
        }
        else if (key == ShopItemKey.PlusOffer)
        {
            DataManager.Instance.playerData.timeBuyPlusOffer = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();

            UIManager.Instance.CheckPlusOffer();
        }

        if (key != ShopItemKey.BoosterOffer)
        {
            gameObject.SetActive(false);

            popupMoreBooster.ShowBundle(ShopItemKey.BoosterOffer);
        }
    }
}
