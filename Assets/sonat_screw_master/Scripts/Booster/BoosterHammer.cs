using UnityEngine;

public class BoosterHammer : BoosterBase
{
    public bool isFree;

    public override void Use()
    {
        BoosterManager.Instance.CheckCancelHammer();

        if (!isFree)
        {
            base.Use();
        }
        else
        {
            isFree = false;
        }
    }

    public override void ForceUse(bool isFree = false)
    {
        BoosterManager.Instance.CheckCancelHammer();

        if (!isFree)
        {
            base.Use();
        }
    }
}
