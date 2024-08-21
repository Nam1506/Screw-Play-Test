using UnityEngine;
using UnityEngine.UI;

public class BoxHole : Hole
{
    [SerializeField] private Image holeImage;
    [SerializeField] private HoleSO holeSO;

    public void SetHoleImage(Sprite sprite)
    {
        holeImage.sprite = sprite;
        holeImage.SetNativeSize();
    }

    public void SetNormalHole()
    {
        holeImage.sprite = holeSO.GetSprite(EHoleType.Normal, EScrewColor.None);
        holeImage.SetNativeSize();
    }
}
