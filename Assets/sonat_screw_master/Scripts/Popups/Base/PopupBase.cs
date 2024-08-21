using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup canvasGroup;

    //public EScreen eScreen;

    public virtual void Show(bool isFadeMask, bool isSetPause = true)
    {
        canvasGroup.DOKill();

        if (isSetPause) GameplayManager.Instance.GameplayState = GameplayState.Pausing;

        PopupManager.OnSetInteractableMask?.Invoke(true);

        if (isFadeMask)
        {
            PopupManager.OnFadeOutBlackMask?.Invoke();
        }

        canvasGroup.gameObject.SetActive(true);

        canvasGroup.alpha = 0;

        canvasGroup.DOFade(1, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                PopupManager.OnSetInteractableMask?.Invoke(false);
            });

        //PopupManager.Instance.CheckPrePopup(this);
    }

    public virtual void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        PopupManager.OnSetInteractableMask?.Invoke(true);

        canvasGroup.DOKill();

        if (!isKeepingMask)
        {
            PopupManager.OnFadeInBlackMask?.Invoke();
        }

        if (isFade)
        {
            canvasGroup.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);

                PopupManager.OnSetInteractableMask?.Invoke(false);

                if (isSetPlaying)
                {
                    GameplayManager.Instance.GameplayState = GameplayState.Playing;
                }
            });
        }
        else
        {
            gameObject.SetActive(false);

            DOVirtual.DelayedCall(PopupConfig.TIME_FADE_MASK, () =>
            {
                PopupManager.OnSetInteractableMask?.Invoke(false);

                if (isSetPlaying)
                {
                    GameplayManager.Instance.GameplayState = GameplayState.Playing;
                }
            });
        }

    }

    public virtual void HideOriginal(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        PopupManager.OnSetInteractableMask?.Invoke(true);

        canvasGroup.DOKill();

        if (!isKeepingMask)
        {
            PopupManager.OnFadeInBlackMask?.Invoke();
        }

        if (isFade)
        {
            canvasGroup.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);

                PopupManager.OnSetInteractableMask?.Invoke(false);

                if (isSetPlaying)
                {
                    GameplayManager.Instance.GameplayState = GameplayState.Playing;
                }
            });
        }
        else
        {
            gameObject.SetActive(false);

            DOVirtual.DelayedCall(PopupConfig.TIME_FADE_MASK, () =>
            {
                PopupManager.OnSetInteractableMask?.Invoke(false);

                if (isSetPlaying)
                {
                    GameplayManager.Instance.GameplayState = GameplayState.Playing;
                }
            });
        }

}

    public virtual void HideImmediate()
    {
        gameObject.SetActive(false);

        PopupManager.OnSetBlackMask?.Invoke(false);
    }
}
