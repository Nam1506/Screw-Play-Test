using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILeaderboard : MonoBehaviour
{
    private void OnEnable()
    {
        //HomeController.Instance.SetPanelBotLayer(1);

        //UIManager.Instance.SendCurrencyToBack();
    }

    private void OnDisable()
    {
        //HomeController.Instance.SetPanelBotLayer(0);

        //UIManager.Instance.SendCurrencyToFront();
    }
}
