using UnityEngine;
using UnityEngine.UI;

public class WidgetStarter : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("starter_bundle", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");

            PopupManager.Instance.ShowStarterPack(false);
        });
    }
}
