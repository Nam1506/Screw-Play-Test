using UnityEngine;
using UnityEngine.UI;

public class PopupPolicy : PopupBase
{
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            //SonatTracking.LogShowUI("user", PlayerData.Instance.savePlayerData.currentLevel, "policy", "Home", "close");

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });
    }
}
