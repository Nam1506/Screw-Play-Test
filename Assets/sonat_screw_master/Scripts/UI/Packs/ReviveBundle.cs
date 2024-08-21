using System.Collections;
using TMPro;
using UnityEngine;

public class ReviveBundle : UIShopPack
{
    [SerializeField] private PopupLose popupLose;
    [SerializeField] private TextMeshProUGUI bonusText;

    private bool canRevive;
    private bool isBuySuccess;

    private void OnDisable()
    {
        //PopupReward.OnClaimRewardAction -= PopupReward_OnClaimRewardAction;
    }

    private void OnEnable()
    {
        canRevive = !HolesQueueController.Instance.IsMaxHoleLose();

        bonusText.gameObject.SetActive(canRevive);

        //PopupReward.OnClaimRewardAction += PopupReward_OnClaimRewardAction;
    }

    private void PopupReward_OnClaimRewardAction(object sender, System.EventArgs e)
    {
        if (isBuySuccess)
        {
            PopupManager.Instance.StartWaitForReward(canRevive);
            isBuySuccess = false;
            PopupReward.OnClaimRewardAction -= PopupReward_OnClaimRewardAction;
        }
    }

    protected override void OnBuySuccess()
    {
        base.OnBuySuccess();

        isBuySuccess = true;

        PopupReward.OnClaimRewardAction += PopupReward_OnClaimRewardAction;
    }

    private IEnumerator IeWaitForReward()
    {
        yield return new WaitForSeconds(0.2f);

        while (UIManager.Instance.IsReceivingRewards())
        {
            yield return null;
        }

        if (canRevive)
        {
            popupLose.Hide(true, false, true);
            popupLose.Revive(false);

            PopupManager.Instance.CloseAllPopupFadeAll();
        }
    }

    public void SetBonusText(string text)
    {
        bonusText.text = text;
    }
}
