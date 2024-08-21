using Spine.Unity;
using UnityEngine;
using UnityEngine.Purchasing;

public class LoadingAnim : MonoBehaviour
{
    public bool isIap;

    [SerializeField] private SkeletonGraphic anim;

    private void OnEnable()
    {
        if (!isIap)
        {
            anim.AnimationState.SetAnimation(0, "animation", false);
            anim.AnimationState.AddAnimation(0, "animation2", true, 0);
        }
        else
        {
            anim.Initialize(true);

            anim.AnimationState.SetAnimation(0, "animation", true);
        }

    }

    private void OnDisable()
    {
        anim.AnimationState.ClearTracks();
    }
}
