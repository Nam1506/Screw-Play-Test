using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI valueText;

    public float timeMove;

    public void Init(Sprite icon, int amount, bool isShowAmount = true, bool isShowPrefix = true)
    {
        if (icon != null)
        {
            this.icon.sprite = icon;
            this.icon.SetNativeSize();
        }

        if (!isShowAmount)
        {
            valueText.gameObject.SetActive(false);
            return;
        }

        if (amount > 0)
        {
            if (isShowPrefix)
            {
                valueText.text = "X" + amount;
            }
            else
            {
                valueText.text = amount.ToString();
            }

            valueText.gameObject.SetActive(true);
        }       
    }

    public void Init(Sprite icon, float time, bool isShowAmount = true)
    {
        if (icon != null)
        {
            this.icon.sprite = icon;
            this.icon.SetNativeSize();
        }

        if (!isShowAmount)
        {
            valueText.gameObject.SetActive(false);
            return;
        }

        if (time > 0)
        {
            int hours = Mathf.FloorToInt(time / 3600);
            int minutes = Mathf.RoundToInt(time % 3600 / 60);

            if (hours > 0)
            {
                valueText.text = minutes > 0 ? string.Format("{0}H{1}M", hours, minutes) : string.Format("{0}H", hours);
            }
            else
            {
                valueText.text = string.Format("{0}M", minutes);
            }
        }
    }

    public void PlayClaimEffect(Transform to, Action callback)
    {
        Tween t = null;

        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.one * 0.5f, timeMove).SetDelay(0.3f).OnComplete(() =>
        {
            if (transform != null)
                transform.localScale = Vector3.one;
        });

        t = transform.DOMove(to.position, timeMove).SetEase(Ease.InBack).OnComplete(() =>
        {
            UIManager.Instance.tweens.Remove(t);

            callback?.Invoke();

            PoolBoss.Despawn(transform);
        });

        UIManager.Instance.tweens.Add(t);
    }
}
