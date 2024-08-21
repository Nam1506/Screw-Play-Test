using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelChest : PopupBase
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button continueBtn;

    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueTxt;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "level_chest", GameManager.Instance.eScreen.ToString(), "close");

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });

        continueBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "level_chest", GameManager.Instance.eScreen.ToString(), "close");

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });
    }

    private void OnEnable()
    {
        var value = LevelChestManager.data.point;
        var endValue = LevelChestManager.END_POINT;

        Sequence sequence = DOTween.Sequence();

        slider.DOKill();

        slider.value = 0f;
        valueTxt.text = $"0/{endValue}";

        sequence.Append(slider.DOValue(value / endValue, 0.5f));

        if (valueTxt != null)
        {
            float currentValue = float.Parse(valueTxt.text.Split('/')[0]);

            sequence.Join(DOTween.To(() => currentValue, x =>
            {
                currentValue = x;

                valueTxt.text = $"{Mathf.Round(currentValue)}/{Mathf.Round(endValue)}";
            }, value, 0.5f));
        }

        sequence.Play();

    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);
    }
}
