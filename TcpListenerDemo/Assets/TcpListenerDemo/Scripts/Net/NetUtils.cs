using UnityEngine;
using System.Collections;
using ProtoBuf;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class NetUtils {

	/// <summary>
	/// Gets the internal IP.
	/// </summary>
	/// <returns>The internal I.</returns>
	public static IPAddress GetInternalIP(){
		IPHostEntry host;
		IPAddress localIP = null;
		host = Dns.GetHostEntry(Dns.GetHostName());
	
		foreach(IPAddress ip in host.AddressList)
		{
			if(ip.AddressFamily.ToString() == "InterNetwork")
			{
				localIP = ip;
				break;
			}
		}
		Debug.Log (localIP);
		return localIP;
	}

	public static byte[] Serialize(IExtensible msg)
	{
		byte[] result;
		using(MemoryStream stream = new MemoryStream())
		{
			Serializer.Serialize(stream, msg);
			result = stream.ToArray();
		}
		return result;
	}
	public static IExtensible Deserialize<IExtensible>(byte[] message)
	{
		IExtensible result;
		using(MemoryStream stream = new MemoryStream(message)){
			result = Serializer.Deserialize<IExtensible>(stream);
		}
		return result;
	}
	public static bool ReceiveVarData(NetworkStream ns, out byte[] receiveData)
	{
		receiveData = null;
		if (null == ns)
			return false;

		int total = 0;
		int recv;
		byte[] datasize = new byte[4];

		try{
			recv = ns.Read(datasize, 0, 4);
		}catch(Exception err) {
			Debug.Log ("ReceiveVarData:"+err.Message);
			return false;
		}
		if (recv <= 0) {
			receiveData = null;
			return false;
		}
		int size = System.BitConverter.ToInt32(datasize, 0);
		int dataleft = size;
		byte[] data = new byte[size];

		while(total < size)
		{
			try{
				recv = ns.Read(data, total, dataleft);
			}
			catch(Exception err) {
				Debug.Log ("ReceiveVarData:"+err.Message);
				return false;
			}
			if(recv <= 0)
			{
				receiveData = null;
				return false;
			}
			total += recv;
			dataleft -= recv;
		}
		receiveData = data;

		return true;
	}
	public static bool SendVarData(NetworkStream ns, byte[] data)
	{
		if (ns == null || data == null)
			return false;
		byte[] datasize = new byte[4];
		datasize = System.BitConverter.GetBytes(data.Length);
		try{
			ns.Write(datasize, 0, datasize.Length);

			ns.Write(data, 0, data.Length);

			ns.Flush();
		}catch(Exception err) {
			Debug.Log ("SendVarData failed:"+err.Message);
			return false;
		}
		return true;
	}
}
