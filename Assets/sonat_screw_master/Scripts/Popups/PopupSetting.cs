using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupBase
{
    [SerializeField] private RectTransform background1;
    [SerializeField] private RectTransform background2;

    [SerializeField] private Button closeBtn;
    [SerializeField] private Button noadsBtn;
    [SerializeField] private Button restoreBtn;
    [SerializeField] private Button policyBtn;
    [SerializeField] private Button rateBtn;

    [SerializeField] private Button homeButton;
    [SerializeField] private Button replayButton;

    [Header("Toggle")]
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button vibrateBtn;
    [SerializeField] private Button notiBtn;

    [SerializeField] private ToggleScript[] toggleMusic;
    [SerializeField] private ToggleScript[] toggleSound;
    [SerializeField] private ToggleScript[] toggleVibrate;
    [SerializeField] private ToggleScript[] toggleNoti;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

            SonatTracking.ClickIcon("exit_settings", level, "Home", () =>
            {
                SonatTracking.LogShowUI("user", level, "settings", "Home", "close");

                Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
            });
        });

        homeButton.onClick.AddListener(() =>
        {
            Hide(false, true, false);

            PopupManager.Instance.ShowRestart(ERestartFrom.SettingHome);

            PopupManager.Instance.PrePopups.Add(this);
        });

        replayButton.onClick.AddListener(() =>
        {
            Hide(false, true, false);

            PopupManager.Instance.ShowRestart(ERestartFrom.SettingReplay);

            PopupManager.Instance.PrePopups.Add(this);
        });

        noadsBtn.onClick.AddListener(() =>
        {
           
        });

        restoreBtn.onClick.AddListener(() =>
        {
            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);

            IAPManager.Restore();
        });

        policyBtn.onClick.AddListener(() =>
        {
            Hide(false, true, false);

            PopupManager.Instance.ShowPolicy();
        });

        rateBtn.onClick.AddListener(() =>
        {
            Hide(false, true, false);

            PopupManager.Instance.ShowRate(false);
        });

        musicBtn.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon(DataManager.Instance.playerData.settings.isMusicOn ? "music_off" : "music_on", DataManager.Instance.playerData.saveLevelData.currentLevel,
                GameManager.Instance.eScreen.ToString(), () =>
                {
                    //SoundManager.Instance.ToggleVolume();

                    DataManager.Instance.playerData.settings.isMusicOn = !DataManager.Instance.playerData.settings.isMusicOn;

                    DataManager.Instance.Save();

                    toggleMusic.OnChanged(DataManager.Instance.playerData.settings.isMusicOn);
                });
        });

        soundBtn.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon(SoundManager.Instance.IsVolumeOn ? "sound_off" : "sound_on", DataManager.Instance.playerData.saveLevelData.currentLevel,
                GameManager.Instance.eScreen.ToString(), () =>
                {
                    SoundManager.Instance.ToggleVolume();
                    toggleSound.OnChanged(!SoundManager.Instance.IsVolumeOn);
                });
        });

        vibrateBtn.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon(VibrateManager.Instance.IsOn ? "vibrate_off" : "vibrate_on", DataManager.Instance.playerData.saveLevelData.currentLevel,
                GameManager.Instance.eScreen.ToString(), () =>
                {
                    VibrateManager.Instance.ToggleVibrate();
                    toggleVibrate.OnChanged(!VibrateManager.Instance.IsOn);
                });
        });

        notiBtn.onClick.AddListener(() =>
        {
          
        });
    }

    private void OnEnable()
    {
        LoadSetting();
    }

    private void LoadSetting()
    {
        toggleMusic.OnChanged(!DataManager.Instance.playerData.settings.isMusicOn);
        toggleSound.OnChanged(!DataManager.Instance.playerData.settings.isSoundOn);
        toggleVibrate.OnChanged(!VibrateManager.Instance.IsOn);

        noadsBtn.gameObject.SetActive(GameManager.Instance.GameState == GameState.Home);

        homeButton.gameObject.SetActive(GameManager.Instance.GameState != GameState.Home);
        replayButton.gameObject.SetActive(GameManager.Instance.GameState != GameState.Home);

        restoreBtn.gameObject.SetActive(GameManager.Instance.GameState == GameState.Home);

        if (GameManager.Instance.GameState == GameState.Home)
        {
            background1.sizeDelta = new Vector2(background1.sizeDelta.x, 758.9f);
            background2.sizeDelta = new Vector2(background2.sizeDelta.x, 533.67f);
        }
        else
        {
            background1.sizeDelta = new Vector2(background1.sizeDelta.x, 907.03f);
            background2.sizeDelta = new Vector2(background2.sizeDelta.x, 675.63f);
        }
    }
}
