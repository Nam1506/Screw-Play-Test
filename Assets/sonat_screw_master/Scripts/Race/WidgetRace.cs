using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WidgetRace : WidgetProgress
{
    [SerializeField] private Transform plusEffect;

    [SerializeField] private Vector2 initPosPlusEff = new Vector2(210f, 0f);

    protected override void Awake()
    {
        base.Awake();

        plusEffect.gameObject.SetActive(false);

        var progress = GetProgress();

        if (canChange)
        {
            valueTxt.text = $"{progress}/{RaceEvent.RACE_RANGE}";
            slider.value = progress * 1f / RaceEvent.RACE_RANGE;
        }
        else
        {
            var point = Mathf.Max(0, progress - 1);

            valueTxt.text = $"{point}/{RaceEvent.RACE_RANGE}";
            slider.value = point / RaceEvent.RACE_RANGE;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SetData();

        RaceEvent.OnStartEvent += SetData;
        RaceEvent.OnStartRace += SetData;
        RaceEvent.OnFinishEvent += SetData;
        RaceEvent.OnFinishRace += SetData;
    }

    private void OnDisable()
    {
        RaceEvent.OnStartEvent -= SetData;
        RaceEvent.OnStartRace -= SetData;
        RaceEvent.OnFinishEvent -= SetData;
        RaceEvent.OnFinishRace -= SetData;
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
        Debug.Log("OK: " + GetProgress());

        base.UpdateProgress(GetProgress(), RaceEvent.RACE_RANGE, isIncrease, 0.5f);
    }

    public override void MatchOnly()
    {
        base.MatchOnly();

        if (!RaceEvent.CheckRacing()) return;

        var progress = GetProgress();

        valueTxt.text = $"{progress}/{RaceEvent.RACE_RANGE}";
        slider.value = progress * 1f / RaceEvent.RACE_RANGE;

        canChange = true;
    }

    private void SetData()
    {
        if (!RaceEvent.raceData.isAvailable)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        button.onClick.RemoveAllListeners();

        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (RaceEvent.CheckCanStartRacing())
        {
            ResetProgress(false, RaceEvent.RACE_RANGE);

            valueTxt.text = "Join";

            button.onClick.AddListener(() =>
            {
                PopupManager.Instance.ShowRaceStart(false);
                SonatTracking.ClickIcon("race", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");
            });
        }
        else if (RaceEvent.CheckRacing())
        {
            Debug.Log("update racing progress");

            //MatchProgress();

            button.onClick.AddListener(() =>
            {
                PopupManager.Instance.ShowRace(false);
                SonatTracking.ClickIcon("race", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");
            });
        }
        else
        {
            valueTxt.text = "Race";
        }
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

    private float GetProgress()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        int progress = RaceEvent.RACE_RANGE - (RaceEvent.raceData.winLevelTarget - level);

        progress = Mathf.Min(progress, RaceEvent.RACE_RANGE);
        progress = Mathf.Max(0, progress);

        return Mathf.RoundToInt(progress);
    }

    public override void SetCanChange(bool state)
    {
        if (!RaceEvent.CheckRacing()) return;

        base.SetCanChange(state);
    }
}
