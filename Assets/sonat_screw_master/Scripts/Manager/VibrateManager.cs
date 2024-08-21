using MoreMountains.NiceVibrations;

public class VibrateManager : SingletonBase<VibrateManager>
{
    private bool isOn;

    public bool IsOn => isOn;

    private void Start()
    {
        isOn = DataManager.Instance.playerData.settings.isVibrateOn;
    }

    public void ToggleVibrate()
    {
        isOn = !isOn;

        Save();
    }

    private void Save()
    {
        DataManager.Instance.playerData.settings.isVibrateOn = isOn;
        DataManager.Instance.Save();
    }

    public void Vibrate(HapticTypes type)
    {
        if (isOn)
        {
            MMVibrationManager.Haptic(type);
        }
    }
}
