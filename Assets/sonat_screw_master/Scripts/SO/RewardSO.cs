using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/RewardSO", fileName = "RewardSO")]
public class RewardSO : ScriptableObject
{
    public List<RewardSprite> sprites;

    public Sprite GetRewardSprite(RewardID id)
    {
        return sprites.Find(x => x.id == id).sprite;
    }
}

[Serializable]
public class RewardSprite
{
    public RewardID id;
    public Sprite sprite;
}
