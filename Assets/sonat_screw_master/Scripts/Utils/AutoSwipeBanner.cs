using Coffee.UIEffects;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;

public class AutoSwipeBanner : MonoBehaviour
{
    [SerializeField] private SimpleScrollSnap bannerScroll;

    private float elapsedTime;

    private const float TIME_SWIPE = 3f;

    private void OnEnable()
    {
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > TIME_SWIPE)
        {
            bannerScroll.Custom_GoToNextPanel();

            int index = bannerScroll.SelectedPanel + 1;

            if (index > bannerScroll.Panels.Length - 1)
            {
                index = 0;
            }

            //UIShiny uIShiny = bannerScroll.Panels[index].GetComponent<UIShiny>();

            //uIShiny.Play();

            elapsedTime = 0;
        }
    }
}
