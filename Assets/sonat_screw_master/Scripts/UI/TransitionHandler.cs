using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI tips;
    [SerializeField] private SkeletonGraphic logoAnim;
    [SerializeField] private SkeletonGraphic bgAnim;

    [SerializeField] private List<string> logoQueues;
    [SerializeField] private List<string> tipQueues;

    private string[] TIPS = { "Use Hammer booster to reach deeper screws",
        "The next box are shown on the left side, plan for the next screw",
        "Coin can be used to buy booster",
        "Use booster to pass level faster"};

    private int index = 0;

    private void OnEnable()
    {
        StartLoading();
    }

    private void StartLoading()
    {
        StopAllCoroutines();

        //StartCoroutine(IELoading());

        DOVirtual.DelayedCall(GameDefine.TRANSITION_DURATION_IOS, () =>
        {
            gameObject.SetActive(false);
        });

        if (++index >= logoQueues.Count)
        {
            index = 0;
        }

        logoAnim.initialSkinName = logoQueues[index];

        logoAnim.Initialize(true);

        logoAnim.AnimationState.SetAnimation(0, "Appear", false);
        logoAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        tips.text = tipQueues[index];

        //icon.sprite = logoQueues[index];
        //icon.SetNativeSize();

        //StartCoroutine(nameof(IeSetRandomTips));
    }

    private IEnumerator IELoading()
    {
        bgAnim.gameObject.SetActive(true);

        logoAnim.Initialize(true);
        bgAnim.Initialize(true);

        logoAnim.AnimationState.SetAnimation(0, "In", false);
        logoAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        bgAnim.AnimationState.SetAnimation(0, "In", false);
        bgAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        yield return new WaitForSeconds(Helper.GetTimeAnim(logoAnim, "In") + 1f);

        logoAnim.AnimationState.SetAnimation(0, "Out", false);
        bgAnim.AnimationState.SetAnimation(0, "Out", false);

        yield return new WaitForSeconds(Helper.GetTimeAnim(logoAnim, "Out"));

        gameObject.SetActive(false);

        bgAnim.gameObject.SetActive(false);
    }

    private IEnumerator IeSetRandomTips()
    {
        System.Random random = new();

        List<string> list = TIPS.OrderBy(t => random.Next()).ToList();

        foreach (string str in list)
        {
            tips.text = str;

            tips.DOColor(new Color(1, 1, 1, 1), 0.3f);

            yield return new WaitForSeconds(1f);

            tips.DOColor(new Color(1, 1, 1, 0), 0.3f);
        }
    }

    private int GetRandom()
    {
        return Random.Range(0, 3);
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(IeSetRandomTips));

        tips.DOKill();
    }
}
