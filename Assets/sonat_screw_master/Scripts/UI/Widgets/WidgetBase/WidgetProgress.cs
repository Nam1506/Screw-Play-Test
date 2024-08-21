using DG.Tweening;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WidgetProgress : MonoBehaviour
{
    [SerializeField] protected Button button;

    [SerializeField] protected Slider slider;

    [SerializeField] protected TMP_Text valueTxt;

    protected bool canChange = true;

    public bool CanChange => canChange;

    protected virtual void Awake()
    {
        slider.value = 0f;
    }

    protected virtual void OnEnable()
    {
        Debug.Log("abc " + name + " enable");
    }


    protected virtual void UpdateProgress(float value, float endValue, bool isIncrease, float duration = 0.5f, Action callback = null)
    {
        if (!canChange) return;

        if (isIncrease)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(slider.DOValue(value / endValue, duration));

            if (valueTxt != null)
            {
                if (!valueTxt.text.Contains('/'))
                {
                    valueTxt.text = "0/" + endValue;
                }

                float currentValue = float.Parse(valueTxt.text.Split('/')[0]);

                sequence.Join(DOTween.To(() => currentValue, x =>
                {
                    currentValue = x;

                    valueTxt.text = $"{Mathf.Round(currentValue)}/{Mathf.Round(endValue)}";
                }, value, duration));
            }

            sequence.Append(DOVirtual.DelayedCall(duration, () =>
            {
                callback?.Invoke();
            }));


            sequence.Play();
        }
        else
        {
            slider.value = value;

            if (valueTxt != null)
            {
                valueTxt.text = $"{Mathf.Round(value)}/{Mathf.Round(endValue)}";
            }

            callback?.Invoke();
        }
    }

    public virtual void MatchOnly()
    {

    }


    public virtual void SetCanChange(bool state)
    {
        canChange = state;
    }

    public virtual void ResetProgress(bool isIncrease, float endValue, float duration = 0.5f)
    {
        if (isIncrease)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(slider.DOValue(0, duration));

            if (valueTxt != null)
            {
                float currentValue = float.Parse(valueTxt.text.Split('/')[0]);

                sequence.Join(DOTween.To(() => currentValue, x =>
                {
                    currentValue = x;

                    valueTxt.text = $"{Mathf.Round(currentValue)}/{Mathf.Round(endValue)}";
                }, 0, 0.5f));
            }

            sequence.Play();
        }
        else
        {
            slider.value = 0;

            if (valueTxt != null)
            {
                valueTxt.text = $"{Mathf.Round(0)}/{Mathf.Round(endValue)}";
            }

        }
    }

}
