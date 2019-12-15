using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using GoogleARCore;
using GoogleARCore.Examples.Common;



public class PoseData : MonoBehaviour
{

	private static readonly HttpClient client = new HttpClient();

	private float timer = 0.0f;
	private float waitTime = .1f;

	private	float x;
	private float y;
	private float z;
	private float qx;
	private float qy;
	private float qz;
	private float qw;

    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    /// <summary>
    /// Collect Pose data evey frame and send to Web Application 
    /// </summary>
    void Update()
    {

    	// setTimer();
    	

    	// setPosePositions();

    	// timedPostPoseData();
    	
    	
        
    }

    /// <summary>
    /// Set Timer for POST REQUEST
    /// </summary>
    void setTimer(){

    	timer += Time.deltaTime;

    }

    /// <summary>
    /// Set POSE positions to position of the Frame before POST REQUEST
    /// </summary>
    void setPosePositions(){
    	x = Frame.Pose.position.x;
    	y = Frame.Pose.position.y;
    	z = Frame.Pose.position.z;
    	qx = Frame.Pose.rotation.x;
    	qy = Frame.Pose.rotation.y;
    	qz = Frame.Pose.rotation.z;
    	qw = Frame.Pose.rotation.w;
    }

    /// <summary>
    /// Make Timed HTTP Request so the server does not overload
    /// </summary>
    void timedPostPoseData(){

    	//Only send a post request after a timed period because 60 fps will cuase server overload.
    	if (timer > waitTime){

    		postPoseData();
    		timer = timer - waitTime;

    	}

    }





    async void postPoseData(){

    	var values = new Dictionary<string, string>{
			{ "x", x.ToString("R") },
			{ "y", y.ToString("R") },
			{ "z", z.ToString("R") },
			{ "qx", qx.ToString("R") },
			{ "qy", qy.ToString("R") },
			{ "qz", qz.ToString("R") },
			{ "qw", qw.ToString("R") }

		};

		

		var content = new FormUrlEncodedContent(values);

		var response = await client.PostAsync("http://10.245.201.158:1142/stream", content);

		var responseString = await response.Content.ReadAsStringAsync();

		Debug.Log(responseString);

    }


     async void makeGetRequest(){

     	HttpResponseMessage response = await client.GetAsync("http://jhchilds.w3.uvm.edu");
    	response.EnsureSuccessStatusCode();
     	string responseBody = await response.Content.ReadAsStringAsync();
     	Debug.Log(responseBody);


     }

     



}
