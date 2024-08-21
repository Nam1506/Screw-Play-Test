using Crystal;
using UnityEngine;

public class ResizeCamera : SingletonBase<ResizeCamera>
{
    private float defaultSize = 15f;
    private float defaultRatio = 0.45f;

    public void Resize()
    {
        Camera.main.orthographicSize = defaultSize;

        //Camera cam = Camera.main;

        //float ratio = (float)Screen.width / Screen.height;

        //cam.orthographicSize = defaultSize;

        //LevelGenerator.Instance.Calculate();

        //Debug.Log("Ratio: " + ratio);

        //if (ratio < defaultRatio)
        //{
        //    cam.orthographicSize = defaultSize * (defaultRatio / ratio);
        //}
        //else
        //{
        //    cam.orthographicSize = defaultSize * (ratio / defaultRatio);
        //}

        //cam.orthographicSize /= UIManager.Instance.UIIngame.GetHeight() / (Screen.height / CanvasManager.Instance.canvaIngame.scaleFactor);

        //cam.orthographicSize = Mathf.Min(cam.orthographicSize, 18.2f);

        //var midY = UIManager.Instance.UIIngame.GetMidY();

        //cam.transform.position = new Vector3(0, 0.5f + midY / cam.PixelsPerUnit(), -10f);

        //#if UNITY_ANDROID
        //        if (ratio < defaultRatio)
        //        {
        //            cam.orthographicSize = defaultSize * (defaultRatio / ratio);
        //        }
        //        else
        //        {
        //            cam.orthographicSize = defaultSize * (ratio / defaultRatio);
        //        }

        //        cam.orthographicSize = Mathf.Min(cam.orthographicSize, 20f);

        //#elif UNITY_IOS
        //        if (ratio < 0.6f)
        //        {
        //            cam.orthographicSize = 16f;
        //        }
        //        else
        //        {
        //            cam.orthographicSize = defaultSize * (ratio / defaultRatio);
        //            cam.orthographicSize = Mathf.Min(cam.orthographicSize, 20f);
        //        }
        //#endif

    }
}
