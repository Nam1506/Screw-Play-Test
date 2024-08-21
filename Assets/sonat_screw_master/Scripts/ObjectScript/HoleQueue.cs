using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoleQueue : Hole
{
    [SerializeField] private ParticleSystem addHoleEffect;

    [SerializeField] private Image img;

    [SerializeField] private Color redColor;

    public void PlayAddHoleEffect()
    {
        addHoleEffect.Play();
    }

    public void Warning()
    {
        img.DOKill();

        img.DOColor(redColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopWarning()
    {
        img.DOKill();

        img.color = Color.white;
    }
}
