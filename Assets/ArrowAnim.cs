using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnim : MonoBehaviour
{
    private float initPosY;

    public float offsetY = 30f;
    public float time = 0.2f;

    private void Awake()
    {
        initPosY = transform.localPosition.y;
    }

    private void OnEnable()
    {
        transform.DOKill();

        transform.localPosition = new Vector3(transform.localPosition.x, initPosY, 0f);

        transform.DOLocalMoveY(initPosY - offsetY, time).SetLoops(-1, LoopType.Yoyo);
    }
}
