using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    [SerializeField] private List<GameObject> screws;
    [SerializeField] private SkeletonGraphic logoAnim;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private RotateScrew rotateScrew;

    [SerializeField] private RectTransform bg;

    private float currentValue;

    private const int INGAME_SCENE = 1;

    private bool canSkip = false;

    private void Awake()
    {
        if (Screen.width * 1f / Screen.height >= 0.8f)
        {
            bg.localScale = Vector3.one * 1.12f;
        }
    }

    private void Start()
    {
        ResetHole();

        TimeManager.Instance.StartGetRealDateTime();

        progressSlider.value = 0f;

        Loading(INGAME_SCENE);
    }

    private void ResetHole()
    {
        foreach (var screw in screws)
        {
            screw.SetActive(false);
        }
    }

    private void CheckProgressHole(int value)
    {
        int maxI = (value / 20);

        if (value > 90)
        {
            maxI += 1;
        }

        if (maxI == 4)
        {
            canSkip = true;
            return;
        }

        for (int i = 0; i < maxI - 1; i++)
        {
            if (!screws[i].activeSelf)
            {
                screws[i].gameObject.SetActive(true);

                screws[i].transform.localScale = Vector3.zero;

                screws[i].transform.DOScale(1.17f, 0.25f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (i == 4)
                        {
                            Debug.Log("OK");
                            canSkip = true;
                        };
                    });
            }
        }
    }

    private void Loading(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));

        logoAnim.AnimationState.SetAnimation(0, "Appear", false);
        logoAnim.AnimationState.AddAnimation(0, "Idle", true, 0);
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        operation.allowSceneActivation = false;

        float timeWait = 0f;

        while (!operation.isDone && currentValue < 1 /* || !TimeManager.Instance.isTimeLoaded*/)
        {
            if (currentValue >= 1/* && TimeManager.Instance.isTimeLoaded*/)
            {
                if (Helper.IsPurchaserInitFailed())
                {
                    while (timeWait < 1.1f)
                    {
                        timeWait += Time.deltaTime;
                        yield return null;
                    }

                    Debug.Log("Purchaser init: " + Helper.IsPurchaserInitFailed());
                    break;
                }
            }

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            currentValue = Mathf.MoveTowards(currentValue, progress, 0.7f * Time.deltaTime);

            progressSlider.value = currentValue;

            yield return null;
        }

#if UNITY_IOS
        if (Kernel.Resolve<BasePurchaser>().IsInitialized())
        {
            IAPManager.GetListProductFromStore();
        }
#endif

        //yield return new WaitForSeconds(0.25f);

        operation.allowSceneActivation = true;

        rotateScrew.canRotate = false;
    }
}