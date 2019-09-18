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
	private List<float> xArray = new List<float>();
	private List<float> yArray = new List<float>();
	private List<float> zArray = new List<float>();
	private List<float> qxArray = new List<float>();
	private List<float> qyArray = new List<float>();
	private List<float> qzArray = new List<float>();
	private List<float> qwArray = new List<float>();

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
    	timer += Time.deltaTime;



    	x = Frame.Pose.position.x;
    	y = Frame.Pose.position.y;
    	z = Frame.Pose.position.z;
    	qx = Frame.Pose.rotation.x;
    	qy = Frame.Pose.rotation.y;
    	qz = Frame.Pose.rotation.z;
    	qw = Frame.Pose.rotation.w;

    	

    	
    	//Only send a post request after a timed period because 60 fps will cuase server overload.
    	if (timer > waitTime){

    		makePostRequest();
    		timer = timer - waitTime;

    	}
        
    }

    /// <summary>
    /// making a TCP Connection for possible web socket needs
    /// </summary>
    void makeTCPConnection(){


    	TcpClient tcpClient = new TcpClient ();
		IPAddress ipAddress = Dns.GetHostEntry ("www.jhchilds.w3.uvm.edu").AddressList[0];
		IPEndPoint ipEndPoint = new IPEndPoint (ipAddress, 5000);

		tcpClient.Connect (ipAddress, 5000);

    }

    async void makePostRequest(){

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

     void dataBuffer(){ //TODO: Build Data buffer 


     	xArray.Add(x);
    	yArray.Add(y);
    	zArray.Add(z);
    	qxArray.Add(qx);
    	qyArray.Add(qy);
    	qzArray.Add(qz);
    	qwArray.Add(qw);





     }



}
