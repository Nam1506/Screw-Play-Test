using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterHole : BoosterBase
{
    public override void Use()
    {
        if (!CanClick()) return;

        HolesQueueController.Instance.AddOneMoreHole();

        base.Use();
    }

    public override void UseFree()
    {
        HolesQueueController.Instance.AddOneMoreHole();

        base.UseFree();
    }

    public override void ForceUse(bool isFree = false)
    {
        base.ForceUse();

        if (!CheckValid()) return;

        HolesQueueController.Instance.AddOneMoreHole();

        if (!isFree)
        {
            base.Use();
        }
    }

    protected override bool CanClick()
    {
        return base.CanClick() && CheckValid();
    }

    protected override bool CheckValid()
    {
        return !HolesQueueController.Instance.HasMovingScrewInQueue() && !HolesQueueController.Instance.IsMaxHole();
    }

    public override void CheckOutOfBooster()
    {
        base.CheckOutOfBooster();

        if (HolesQueueController.Instance.IsMaxHole())
        {
            toggleBg.OnChanged(false);
        }
    }
}
