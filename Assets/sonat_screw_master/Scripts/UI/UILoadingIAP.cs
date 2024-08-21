using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadingIAP : MonoBehaviour
{
    [SerializeField] private List<Transform> circles;

    [SerializeField] private float initScale = 0.5f;

    [SerializeField] private float targetScale = 1f;

    [SerializeField] private float timeScale = 0.1f;

    [SerializeField] private float timeGap = 0.05f;

    private void OnEnable()
    {
        foreach (var c in circles)
        {
            c.localScale = Vector3.one * initScale;
        }

        StartCoroutine(IEPlay());
    }

    private IEnumerator IEPlay()
    {
        while (true)
        {
            foreach (var c in circles)
            {
                c.DOKill();

                c.DOScale(targetScale, timeScale).SetLoops(2, LoopType.Yoyo);

                yield return new WaitForSeconds(timeGap);
            }
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
