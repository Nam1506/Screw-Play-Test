using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/LoseSO", fileName = "LoseSO")]
public class LoseSO : ScriptableObject
{
    public List<LoseInfo> loseInfos;

    public LoseInfo GetLoseInfo(LoseCause cause)
    {
        return loseInfos.Find(l => l.cause == cause);
    }
}

[Serializable]
public class LoseInfo
{
    public LoseCause cause;
    public Sprite sprite;
    public string description;
    public int coinRevive;
}
