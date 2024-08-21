using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMiniShop : PopupBase
{
    [SerializeField] private Button closeButton;

    private float time;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            if (PopupManager.Instance.ShowPrePopup(null))
            {
                Hide(false, true, true);
            }
            else
            {
                Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
            }

        });
    }

    private void OnEnable()
    {
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void OnDisable()
    {
        SonatTracking.LogSpentTime("popup", "mini_shop", GameManager.Instance.eScreen.ToString(), time);
    }
}