using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/HoleSO", fileName = "HoleSO")]
public class HoleSO : ScriptableObject
{
    public List<HoleSprite> holeSprites;

    public Sprite GetSprite(EHoleType type, EScrewColor color)
    {
        HoleSprite holeSprite;

        if (type == EHoleType.Normal)
        {
            holeSprite = holeSprites.Find(x => x.type == type);
        }
        else
        {
            holeSprite = holeSprites.Find(x => x.type == type && x.color == color);    
        }

        if (holeSprite == null)
        {
            Debug.LogError($"Not found hole sprite with type {type} and color {color}");
            return null;
        }
        else
        {
            return holeSprite.sprite;
        }
    }
}

[Serializable]
public class HoleSprite
{
    public Sprite sprite;
    public EHoleType type;
    public EScrewColor color;
}
