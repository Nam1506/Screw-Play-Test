using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRate : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button rateBtn;
    [SerializeField] private List<Button> stars;

    private int starCount;

    private const string AndroidRatingURI = "http://play.google.com/store/apps/details?id=";

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });

        foreach (var star in stars)
        {
            star.onClick.AddListener(() =>
            {
                foreach (var star2 in stars)
                {
                    if (stars.IndexOf(star2) <= stars.IndexOf(star))
                    {
                        star2.GetComponent<ToggleScript>().OnChanged(true);
                    }
                    else
                    {
                        star2.GetComponent<ToggleScript>().OnChanged(false);
                    }
                }

                starCount = stars.IndexOf(star) + 1;
            });
        }

        rateBtn.onClick.AddListener(() =>
        {
            Rate();

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);

            PopupManager.Instance.ShowNotiAlert("Thanks for your rating!");
        });
    }

    private void OnEnable()
    {
        starCount = 5;

        foreach (var star in stars)
        {
            star.GetComponent<ToggleScript>().OnChanged(true);
        }
    }

    private void Rate()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (starCount < 4)
        {
            SonatTracking.LogShowUI("auto", level, "rate", "Ingame", "other_star");
        }
        else if (starCount == 4)
        {
            SonatTracking.LogShowUI("auto", level, "rate", "Ingame", "4_star");
            OpenStore();
        }
        else if (starCount == 5)
        {
            SonatTracking.LogShowUI("auto", level, "rate", "Ingame", "5_star");
            OpenStore();
        }

        SonatTracking.LogShowUI("auto", level, "rate", "Ingame", "rate_now");
    }

    private void OpenStore()
    {
#if UNITY_IPHONE
        Application.OpenURL("https://itunes.apple.com/app/id6504008582");
#else
        Application.OpenURL(AndroidRatingURI + Application.identifier);
#endif
    }
}
