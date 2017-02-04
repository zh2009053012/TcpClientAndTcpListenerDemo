using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using ProtoBuf;
using com.crownrace.msg;

public class ServerDealSystem {

	public static void DoClientReq(TcpListenerHelper.ReceiveClientData data)
	{
		if (data == null) {
			return;
		}
		packet pack = NetUtils.Deserialize<packet> (data.data);
		Debug.Log ("DoClientReq:"+data.player_id+","+pack.cmd);
		switch (pack.cmd) {
		case NET_CMD.HEARTBEAT_ACK_CMD:
			DoHeartbeatAck (data.player_id, pack.payload);
			break;
		case NET_CMD.LOGIN_REQ_CMD:
			DoLoginReq (data.player_id, pack.payload);
			break;
		}
	}

	static void DoHeartbeatAck(int player_id, byte[] data)
	{
		
		heartbeat_ack hb = NetUtils.Deserialize<heartbeat_ack> (data);
		Debug.Log ("server:DoHeartbearAck:"+hb.player_id);
		if (player_id == hb.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SetClientTimeStamp(player_id, GameUtils.GetUTCTimeStamp ());
		}
	}

	static void DoLoginReq(int player_id, byte[] data)
	{
		login_req req = NetUtils.Deserialize<login_req> (data);
		Debug.Log ("server:receive login req:"+req.name+","+req.res_name);
		login_ack ack = new login_ack ();
		ack.player_id = player_id;
		//cd.SendData<login_ack> (NET_CMD.LOGIN_ACK_CMD, ack);
		TcpListenerHelper.Instance.clientsContainer.SendToClient<login_ack>(player_id, NET_CMD.LOGIN_ACK_CMD, ack);
	}

	public static void NotifyClientLeave(int player_id)
	{
		Debug.Log ("Notify client leave."+player_id);
		leave_game_ntf ntf = new leave_game_ntf ();
		ntf.player_id = player_id;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<leave_game_ntf> (NET_CMD.LEAVE_GAME_NTF_CMD, ntf);
	}
}
