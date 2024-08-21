using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArea : MonoBehaviour
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
