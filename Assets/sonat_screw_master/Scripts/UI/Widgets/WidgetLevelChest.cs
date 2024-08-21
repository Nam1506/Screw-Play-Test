using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WidgetLevelChest : WidgetProgress
{
    [SerializeField] private Transform plusEffect;

    [SerializeField] private Vector2 initPosPlusEff = new Vector2(210f, 0f);

    protected override void Awake()
    {
        base.Awake();

        plusEffect.gameObject.SetActive(false);

        if (canChange)
        {
            valueTxt.text = $"{LevelChestManager.data.point}/{LevelChestManager.END_POINT}";
            slider.value = LevelChestManager.data.point / LevelChestManager.END_POINT;
        }
        else
        {
            var point = Mathf.Max(0, LevelChestManager.data.point - 1);

            valueTxt.text = $"{point}/{LevelChestManager.END_POINT}";
            slider.value = point / LevelChestManager.END_POINT;
        }

        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("level_chest", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");

            PopupManager.Instance.ShowLevelChest();
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //MatchProgress(true);
    }

    public void ForceUpdateProgress()
    {
        PopupManager.Instance.canSetInteractable = false;

        PopupManager.OnSetInteractableMask?.Invoke(true);

        base.SetCanChange(true);

        PlayPlusEffect();
    }

    public void MatchProgress(bool isIncrease = true)
    {
        base.UpdateProgress(LevelChestManager.data.point, LevelChestManager.END_POINT, isIncrease, 0.5f, () => { OnSuccess(); });
    }

    public override void MatchOnly()
    {
        base.MatchOnly();

        valueTxt.text = $"{LevelChestManager.data.point}/{LevelChestManager.END_POINT}";
        slider.value = LevelChestManager.data.point / LevelChestManager.END_POINT;

        canChange = true;
    }

    public void OnSuccess()
    {
        if (!LevelChestManager.IsCompleted())
        {
            PopupManager.OnSetInteractableMask(false);
            PopupManager.Instance.CompleteForceAction();
            return;
        }

        LevelChestManager.OnSuccess();

        ResetProgress(false, LevelChestManager.END_POINT);

        var rewards = LevelChestManager.GenerateRewards();

        IAPManager.Claim(rewards, "level_chest");

        PopupManager.Instance.ShowRewardLevelChest(rewards);

        PopupRewardLevelChest.OnClaimRewardAction += PopupRewardLevelChest_OnClaimRewardAction;
    }

    private void PopupRewardLevelChest_OnClaimRewardAction(object sender, System.EventArgs e)
    {
        DOVirtual.DelayedCall(PopupManager.Instance.TimeDelayRewardLevelChest, () =>
        {
            PopupManager.Instance.CompleteForceAction();
            PopupReward.OnClaimRewardAction -= PopupRewardLevelChest_OnClaimRewardAction;
        });
    }

    public void PlayPlusEffect()
    {
        StartCoroutine(IEPlayEffect());
    }

    private IEnumerator IEPlayEffect()
    {
        plusEffect.localScale = Vector3.zero;
        plusEffect.localPosition = initPosPlusEff;

        plusEffect.gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(KeySound.Booster_Appear);

        plusEffect.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        plusEffect.DOMove(transform.position, 0.2f).SetEase(Ease.InBack).SetDelay(0.4f);
        plusEffect.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetDelay(0.4f)
            .OnComplete(() =>
            {
                plusEffect.gameObject.SetActive(false);

                EffectManager.Instance.PlayReceiveEffect(transform.position, 0.5f);
                SoundManager.Instance.PlaySound(KeySound.Booster_Receive);

                PopupManager.Instance.canSetInteractable = true;
            });

        yield return new WaitForSeconds(0.6f);

        MatchProgress();
    }
}
