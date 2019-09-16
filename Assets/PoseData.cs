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
    /// Collect Pose data evey frame and send to Web Application hosted on the UVM Silk Server
    /// </summary>
    void Update()
    {

    	x = Frame.Pose.position.x;
    	y = Frame.Pose.position.y;
    	z = Frame.Pose.position.z;
    	qx = Frame.Pose.rotation.x;
    	qy = Frame.Pose.rotation.y;
    	qz = Frame.Pose.rotation.z;
    	qw = Frame.Pose.rotation.w;

    	makePostRequest(x,y,z,qx,qy,qz,qw);
    	// makeGetRequest();
        
    }


    void makeTCPConnection(){


    	TcpClient tcpClient = new TcpClient ();
		IPAddress ipAddress = Dns.GetHostEntry ("www.jhchilds.w3.uvm.edu").AddressList[0];
		IPEndPoint ipEndPoint = new IPEndPoint (ipAddress, 5000);

		tcpClient.Connect (ipAddress, 5000);

    }

    async void makePostRequest(float x,float y,float z,float qx,float qy,float qz,float qw){

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

		var response = await client.PostAsync("http://jhchilds.w3.uvm.edu/", content);

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
