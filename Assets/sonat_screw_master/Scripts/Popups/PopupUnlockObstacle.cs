using UnityEngine;
using UnityEngine.UI;

public class PopupUnlockObstacle : PopupForce
{
    [SerializeField] private Image obstacleIcon;
    [SerializeField] private Button continueBtn;
    [SerializeField] private ObstacleSO obstacleSO;

    private EObstacleType obstacleType;

    private void Awake()
    {
        continueBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_Obstacle", "Ingame", "close");

            Hide(PopupManager.Instance.ForceShowPopupActions.Count <= 1, false, false);
        });
    }

    public void SetObstalce(EObstacleType type)
    {
        obstacleType = type;

        foreach (var obstacle in obstacleSO.obstacles)
        {
            if (obstacle.type == type)
            {
                obstacleIcon.sprite = obstacle.icon;
                //obstacleIcon.SetNativeSize();

                return;
            }
        }
    }
}
