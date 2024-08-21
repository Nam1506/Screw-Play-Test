using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ObstacleSO", fileName = "ObstacleSO")]
public class ObstacleSO : ScriptableObject
{
    public List<ObstacleInfo> obstacles;
}

[Serializable]
public class ObstacleInfo
{
    public Sprite icon;
    public EObstacleType type;
    public int levelStart;
    public int levelUnlock;
}
