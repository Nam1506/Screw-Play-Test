using DG.Tweening;
using UnityEngine;

public class ActionDoPunch : ActionScript
{
    public Vector3 punch;
    public float duration;
    public int vibrato;
    public int elasticity;

    private Sequence sequence;
    private bool isPunching = false;

    protected override void DoAction()
    {
        base.DoAction();

        if (isPunching) return;

        isPunching = true;

        Vector3 preScale = transform.localScale;

        transform.DOPunchScale(punch, duration, vibrato, elasticity).OnComplete(() =>
        {
            isPunching = false;
            transform.localScale = preScale;
        }).OnKill(() =>
        {
            isPunching = false;
            transform.localScale = preScale;
        });
    }

    private void DoAction(int i)
    {
        base.DoAction();

        Vector3 preScale = transform.localScale;

        sequence = DOTween.Sequence();

        Tween tween = transform.DOPunchScale(punch, duration, vibrato, elasticity).SetLoops(2, LoopType.Restart).OnComplete(() =>
        {
            transform.localScale = preScale;
        }).OnKill(() =>
        {
            transform.localScale = preScale;
        });

        sequence.Append(tween);
        sequence.AppendInterval(1f);
        sequence.SetLoops(i);
    }

    public override void StartAction()
    {
        DoAction();
    }

    public override void StartAction(int i)
    {
        DoAction(i);
    }

    public override void StopAction()
    {
        sequence.Kill();
    }
}
