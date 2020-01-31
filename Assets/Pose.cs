using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class Pose : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    public static Pose Instance { set; get; }


    public float x;
    public float y;
    public float z;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        x = Frame.Pose.position.x;
        y = Frame.Pose.position.y;
        z = Frame.Pose.position.z;

    }

    public async void postPoseData()
    {
        var values = new Dictionary<string, string>{
            { "x", x.ToString("R") },
            { "y", y.ToString("R") },
            { "z", z.ToString("R") },
            { "lat", GPS.Instance.latitude.ToString("R")},
            { "lon", GPS.Instance.longitude.ToString("R")}
        };

        var content = new FormUrlEncodedContent(values);

        var response = await client.PostAsync("http://192.168.0.164:1142/stream", content);

        var responseString = await response.Content.ReadAsStringAsync();

        Debug.Log(responseString);
    }


}
