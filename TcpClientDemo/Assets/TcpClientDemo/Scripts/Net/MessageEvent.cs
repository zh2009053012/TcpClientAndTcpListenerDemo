using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

public class MessageEvent {
	public delegate void NetMessageEvent(byte[] data);
	public string methodName;
	public NetMessageEvent method;
	public MessageEvent(){}
	public MessageEvent(NetMessageEvent method, string methodName="")
	{
		this.method = method;
		this.methodName = methodName;
	}
}
