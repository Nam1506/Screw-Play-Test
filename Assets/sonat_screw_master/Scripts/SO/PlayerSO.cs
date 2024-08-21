using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/PlayerSO", fileName = "PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public List<Avatar> avatars;

    public Sprite GetAvatar(int index)
    {
        return avatars[index].sprite;
    }
}

[Serializable]
public class Avatar
{
    public Sprite sprite;
}
