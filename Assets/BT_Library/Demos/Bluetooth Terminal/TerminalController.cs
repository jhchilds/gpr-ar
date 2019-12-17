using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TechTweaking.Bluetooth;
using UnityEngine.UI;

using System;
using System.IO;
//using Tango;

public class TerminalController : MonoBehaviour
{
	// Mauricio Tango
	public Text devicNameText;
	public Text status;

	//ScrollTerminalUI is a script used to control the ScrollView text
	public ScrollTerminalUI readDataText;

	public GameObject InfoCanvas;
	public GameObject DataCanvas;
	public GameObject MapCanvas;
	public BluetoothDevice device;
	public Text dataToSend;
  public bool m_isConnected = false;
  public bool m_post = true;

  void Awake ()
	{
		BluetoothAdapter.askEnableBluetooth ();//Ask user to enable Bluetooth

		BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;
		BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked; //To get what device the user picked out of the devices list

	}

	void HandleOnDeviceOff (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name))
			status.text = "Couldn't connect to " + dev.Name + ", device is OFF";
		else if (!string.IsNullOrEmpty (dev.Name)) {
			status.text = "Couldn't connect to " + dev.MacAddress + ", device is OFF";
		}
	}

	void HandleOnDevicePicked (BluetoothDevice device)//Called when device is Picked by user
	{
		this.device = device;//save a global reference to the device

		//this.device.UUID = UUID; //This is only required for Android to Android connection

		/*
		 * 10 equals the char '\n' which is a "new Line" in Ascci representation,
		 * so the read() method will retun a packet that was ended by the byte 10. simply read() will read lines.
		 */
		device.setEndByte (10);


		//Assign the 'Coroutine' that will handle your reading Functionality, this will improve your code style
		//Other way would be listening to the event Bt.OnReadingStarted, and starting the courotine from there
		device.ReadingCoroutine = ManageConnection;

		devicNameText.text = "Remote Device : " + device.Name;
	}


	//############### UI BUTTONS RELATED METHODS #####################
	public void showDevices ()
	{
		BluetoothAdapter.showDevices ();//show a list of all devices//any picked device will be sent to this.HandleOnDevicePicked()
		Debug.Log("Show Devices Clicked");
	}

	public void connect ()//Connect to the public global variable "device" if it's not null.
	{
		if (device != null)
		{
			device.connect ();
      m_isConnected = true;
			status.text = String.Format("Connected: {0}, Can Post: {1}", m_isConnected.ToString(), m_post.ToString());
		}
	}

	public void disconnect ()//Disconnect the public global variable "device" if it's not null.
	{
		// there was a diff here between codes
		if (device != null)
		{
			device.close ();
    	m_isConnected = false;
			status.text = m_isConnected.ToString();
		}
  }

  public void send()
  {
      if (device != null && !string.IsNullOrEmpty(dataToSend.text))
      {
          device.send(System.Text.Encoding.ASCII.GetBytes("A" + (char)10));//10 is our seperator Byte (sepration between packets)
      }
  }

  public void sendA(string index)
  {
      try
      {
          if (device != null)
          {
              device.send_Blocking(System.Text.Encoding.ASCII.GetBytes("A" + (char)10));//10 is our seperator Byte (separation between packets)
							m_post = false;
					}
      }
      catch (Exception ex)
      {
          string filepath = Application.persistentDataPath + "/debugA_terminal.txt";
          using (StreamWriter writer = new StreamWriter(filepath, true))
          {
              string message = String.Format("Send Exception: {0} Inner: {1}", ex.Message.ToString(), ex.InnerException.ToString());
              writer.WriteLine(message);
              writer.Flush();
          }
      }
  }

  //############### Reading Data  #####################
  //Please note that you don't have to use Couroutienes, you can just put your code in the Update() method
  IEnumerator  ManageConnection (BluetoothDevice device)
	{//Manage Reading Coroutine
		

		//Switch to Terminal View
		InfoCanvas.SetActive (false); 
		// MapCanvas.Map.SetActive(false);
		DataCanvas.SetActive (true);

		while (device.IsReading) {
			


			if (device.IsDataAvailable) {
				//because we called setEndByte(10)..read will always return a packet excluding the last byte 10. 10 equals '\n' so it will return lines.

				byte [] msg = device.read ();

				if (msg != null && msg.Length > 0) {
					string content = System.Text.ASCIIEncoding.ASCII.GetString(msg);
					readDataText.add (device.Name, content);

					string filepath = Application.persistentDataPath + "/read_remote.txt";

					if (content == "X\r")
					{
						m_post = true;
						using (StreamWriter writer = new StreamWriter(filepath, true))
	            {
                writer.WriteLine(String.Format("{0}) Setting post to {1}. Connected: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), m_post.ToString(), m_isConnected.ToString()));
                writer.Flush();
	            }
					}
				}
			}

			yield return null;
		}
		//Switch to Menue View after reading stoped
		DataCanvas.SetActive (false);
		// MapCanvas.Map.SetActive(true);
		InfoCanvas.SetActive (true);
	}


	//############### UnRegister Events  #####################
	void OnDestroy ()
	{
		BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
		BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
	}

}
