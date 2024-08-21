using UnityEngine;
using UnityEngine.UI;
using System;

public class TopUserInfo : MonoBehaviour
{
    [SerializeField] private Button toggleBtn;
    [SerializeField] private GameObject userInfo;
    [SerializeField] private GameObject rewardInfo;

    public void ToggleInfo()
    {
        userInfo.SetActive(!userInfo.activeSelf);
        rewardInfo.SetActive(!rewardInfo.activeSelf);
    }

    public void ShowRewardInfo()
    {
        userInfo.SetActive(false);
        rewardInfo.SetActive(true);
    }

    public void HideRewardInfo()
    {
        userInfo.SetActive(true);
        rewardInfo.SetActive(false);
    }

    public void AddAction(Action action)
    {
        toggleBtn.onClick.RemoveAllListeners();

        toggleBtn.onClick.AddListener(() => action?.Invoke());
    }

    public void SetCanClick(bool state)
    {
        toggleBtn.interactable = state;
    }
}
