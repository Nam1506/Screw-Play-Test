using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterBox : BoosterBase
{
    public override void Use()
    {
        if (!CanClick()) return;

        SoundManager.Instance.PlaySound(KeySound.AddBox);

        BoxController.Instance.AddOneMoreBox();

        base.Use();
    }

    public override void UseFree()
    {
        SoundManager.Instance.PlaySound(KeySound.AddBox);

        BoxController.Instance.AddOneMoreBox();

        base.UseFree();

        CheckToggleBG();
    }

    public override void ForceUse(bool isFree = false)
    {
        base.ForceUse();

        if (!CheckValid()) return;

        SoundManager.Instance.PlaySound(KeySound.AddBox);

        BoxController.Instance.AddOneMoreBox();

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
        return !GameplayManager.Instance.IsAddingBox && GameplayManager.Instance.GetActiveBoxes().Count > 0
            && GameplayManager.Instance.boxes.Count > 1;
    }

    public override void CheckOutOfBooster()
    {
        base.CheckOutOfBooster();

        CheckToggleBG();
    }

    public void CheckToggleBG()
    {
        if (IsLocking)
        {
            toggleBg.OnChanged(false);
        }
        else
        {
            if (GameplayManager.Instance.IsAddingBox || GameplayManager.Instance.boxes.Count == 1)
                toggleBg.OnChanged(false);
            else
                toggleBg.OnChanged(true);
        }
    }
}
