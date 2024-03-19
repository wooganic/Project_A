using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using static UnityEditor.FilePathAttribute;

public class GPSManager : MonoBehaviour
{
    public Text text_ui;
    public GameObject welcome_popUp;
    public bool isFirst = false;

    public double[] lats;
    public double[] longs;
    public int distanceToGoal = 100;

    IEnumerator Start()
    {

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        }
        if (!Input.location.isEnabledByUser)
        {
            text_ui.text = "Please turn on GPS";
            yield break;
        }
        Input.location.Start(10, 1);
        text_ui.text = "Location Services started";

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Initializing Location Services... Time remaining:" + maxWait);
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            text_ui.text = "initialization timeout";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            text_ui.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            Debug.Log("Location Services is up and running. Start real-time location updates");
            while (true)
            {
                text_ui.text = Input.location.lastData.latitude + " / " + Input.location.lastData.longitude;
                yield return null;
            }
        }
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;

            double remainDistance = distance(myLat, myLong, lats[0], longs[0]);

            if (remainDistance <= distanceToGoal)
            {
                if (!isFirst)
                {
                    isFirst = true;
                    welcome_popUp.SetActive(true);
                }
            }
        }
    }

    // 지표면 거리 계산 공식(하버사인 공식)
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;

        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        dist = Math.Acos(dist);

        dist = Rad2Deg(dist);

        dist = dist * 60 * 1.1515;

        dist = dist * 1609.344; // 미터 변환

        return dist;
    }

    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }
}