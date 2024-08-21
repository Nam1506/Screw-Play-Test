using DG.Tweening;
using Spine.Unity;
using System.Collections;
using UnityEngine;

public class UITakeBreak : MonoBehaviour
{
    [SerializeField] private ToggleScript[] fadeObj;
    [SerializeField] private SkeletonGraphic anim;

    private bool isReceiveCoin;

    private bool isPlaying = false;

    private void OnEnable()
    {
        StartCoroutine(Show());
    }

    private void OnDisable()
    {
        if (isReceiveCoin)
        {
            DataManager.Instance.ChangeCoins(2, false);

            UIManager.Instance.UICoin.SetKeepCoin();

            UIManager.Instance.UICoin.SetForce(int.Parse(UIManager.Instance.UICoin.CurrentCoinText), 2);

            DOVirtual.DelayedCall(0.5f, () =>
            {
                UIManager.Instance.CollectCoins(10, () =>
                {
                    GameplayManager.Instance.GameplayState = isPlaying ? GameplayState.Playing : GameplayState.Pausing;

                }, Vector3.zero);
            });

            isReceiveCoin = false;
        }
    }

    private IEnumerator Show()
    {
        isPlaying = GameplayManager.Instance.GameplayState == GameplayState.Playing ? true : false;

        GameplayManager.Instance.GameplayState = GameplayState.Pausing;

        fadeObj.SetActiveAll(false);

        anim.Initialize(true);

        anim.AnimationState.SetAnimation(0, "animation", false);

        yield return new WaitForSeconds(0.3f);

        fadeObj.OnChanged(true);

        isReceiveCoin = true;

        //yield return new WaitForSeconds(0.675f);

        //fadeObj.OnChanged(false);
    }
}
