using Newtonsoft.Json;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : SingletonBase<TimeManager>
{
    [HideInInspector] public bool isTimeLoaded = false;
    [HideInInspector] public bool isLoadedSuccessfully = false;

    private DateTime currentDateTime = DateTime.Now;

    //private const string API_URL = "https://worldtimeapi.org/api/ip";
    private const string API_URL = "https://www.google.com";

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public DateTime GetCurrentDateTime()
    {
        return Helper.AddDuration(currentDateTime, Time.realtimeSinceStartup);
    }

    public void StartGetRealDateTime()
    {
        StartCoroutine(GetRealDateTimeFromAPI());
    }

    private IEnumerator GetRealDateTimeFromAPI()
    {

        isTimeLoaded = true;
        isLoadedSuccessfully = false;

        UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);
        Debug.Log("Getting real datetime...");

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + webRequest.error);

            isTimeLoaded = true;
            isLoadedSuccessfully = false;
        }
        else
        {
            //TimeData timeData = JsonConvert.DeserializeObject<TimeData>(webRequest.downloadHandler.text);
            //currentDateTime = Helper.StringToDate(timeData.dateTime);

            string respone = webRequest.GetResponseHeader("date");

            currentDateTime = DateTime.ParseExact(respone, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);

            isTimeLoaded = true;
            isLoadedSuccessfully = true;

            Debug.Log("Getting real datetime successfully");
        }
    }
}

public class TimeData
{
    // public string clientIP;

    public string dateTime;
}
