using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using com.crownrace.msg;
using ProtoBuf;
using System;
using System.Threading;

public class TcpClientHelper : MonoBehaviour {

	private static TcpClientHelper instance;
	private static object lockHelper = new object();
	public static TcpClientHelper Instance{
		get{
			if(instance == null)
			{
				lock(lockHelper)
				{
					if(instance == null)
					{
						GameObject go = new GameObject("TcpClientHelper");
						instance = go.AddComponent<TcpClientHelper>();
					}
				}
			}
			return instance;
		}
	}

	TcpClient client;
	NetworkStream stream;

	#region receive message
	Thread receiveThread;

	private Dictionary<NET_CMD, List<MessageEvent>> map = new Dictionary<NET_CMD, List<MessageEvent>>();
	private object lockMap = new object();
	public bool UnregisterNetMsg(NET_CMD cmd, MessageEvent.NetMessageEvent method)
	{
		lock(lockMap)
		{
			List<MessageEvent> list;
			if(map.TryGetValue(cmd, out list))
			{
				for(int i=0; i<list.Count; i++)
				{
					if(list[i].method.Equals(method))
					{
						list.RemoveAt(i);
						return true;
					}
				}
			}
		}
		return false;
	}
	public bool RegisterNetMsg(NET_CMD cmd, MessageEvent.NetMessageEvent method, string methodName="")
	{
		if(null == method)
			return false;
		MessageEvent me = new MessageEvent(method, methodName);
		lock(lockMap)
		{
			List<MessageEvent> list;
			if(map.TryGetValue(cmd, out list))
			{
				for(int i=0; i<list.Count; i++)
				{
					//already register
					if(list[i].method.Equals(method))
					{
						return true;
					}
				}
				//not register
				list.Add(me);
				return true;
			}
			list = new List<MessageEvent>();
			list.Add(me);
			map.Add(cmd, list);
		}
		return true;
	}
	//
	private LinkedList<byte[]> receiveDataBufferList = new LinkedList<byte[]>();
	private LinkedList<byte[]> receiveDataProcessList = new LinkedList<byte[]>();
	private object receiveDataLock = new object();
	void Receive()
	{
		while(true)
		{
			Debug.Log ("do receive");
			if (stream.CanRead) {
				byte[] data;
				if (NetUtils.ReceiveVarData (stream, out data)) {
					lock (receiveDataLock) {
						receiveDataBufferList.AddLast (data);
					}
				} else {
					//disconnect
					Debug.Log("disconnect");
					Close();
				}
			}
		}
	}
	//should be called by main thread
	void ReceiveData()
	{
		if(receiveDataBufferList.Count > 0)
		{
			lock(receiveDataLock)
			{
				foreach(byte[] item in receiveDataBufferList)
				{
					receiveDataProcessList.AddLast(item);
				}
				receiveDataBufferList.Clear();
			}
			foreach(byte[] item in receiveDataProcessList){
				packet package = (packet)NetUtils.Deserialize<packet>(item);
				List<MessageEvent> list;
				if(map.TryGetValue(package.cmd, out list))
				{
					foreach(MessageEvent me in list)
					{
						Debug.Log("receive:"+package.cmd+",call func:"+me.methodName);
						me.method.Invoke(package.payload);
					}
				}
			}
			receiveDataProcessList.Clear();
		}
	}
	#endregion
	//
	#region send message
	Thread sendThread;
	private LinkedList<byte[]> sendDataBufferList = new LinkedList<byte[]>();
	private LinkedList<byte[]> sendDataProcessList = new LinkedList<byte[]>();
	private object sendDataLock = new object();

	public void SendData<T>(NET_CMD cmd, T message)where T:ProtoBuf.IExtensible{

		packet pack = new packet();
		pack.cmd = cmd; 
		pack.payload = NetUtils.Serialize(message);

		lock(sendDataLock)
		{
			sendDataBufferList.AddLast(NetUtils.Serialize(pack));
		}

	}

	void SendToServer(){
		while(true)
		{
			if (sendDataBufferList.Count > 0) {
				lock (sendDataLock) {
					foreach (byte[] item in sendDataBufferList) {
						sendDataProcessList.AddLast (item);
					}
					sendDataBufferList.Clear ();
				}
				foreach (byte[] item in sendDataProcessList) {
					if (!NetUtils.SendVarData (stream, item)) {
						//disconnect
						Debug.Log("SendToServer:disconnect");
						Close ();
					}
				}
				sendDataProcessList.Clear ();
			}
			Thread.Sleep(100);
		}
	}
	#endregion

	public bool Connect(string hostName, int port)
	{
		try{
			client = new TcpClient();
			client.Connect(hostName, port);
		}catch(SocketException err){
			Debug.Log("connect server failed.");
			return false;
		}
		stream = client.GetStream();
		receiveThread = new Thread(new ThreadStart(Receive));
		receiveThread.Start();
		sendThread = new Thread(new ThreadStart(SendToServer));
		sendThread.Start();
		return true;
	}

	public void Close()
	{
		Debug.Log ("close");
		if (null != map)
			map.Clear ();
		if(null != receiveThread)
			receiveThread.Abort();
		if(null != sendThread)
			sendThread.Abort();
		if(null != stream)
			stream.Close();
		if(null != client)
			client.Close();
	}


	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		ReceiveData ();
	}

	void OnApplicationQuit()
	{
		Close ();
	}
}
