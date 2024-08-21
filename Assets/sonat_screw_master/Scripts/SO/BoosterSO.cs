using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BoosterSO", fileName = "BoosterSO")]
public class BoosterSO : ScriptableObject
{
    public List<BoosterInfo> boosters;
}

[System.Serializable]
public class BoosterInfo
{
    public EBoosterType type;
    public string name;
    public string description;
    public Sprite icon;
    public Sprite bigIcon;
    public Sprite titleIcon;
    public int cost;
    public int levelUnlock;

    [Header("Tut")]

    public Sprite tutIcon;
    public Sprite tutRibbon;
    public string nameSkin;
}
