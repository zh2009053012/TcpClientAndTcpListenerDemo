using UnityEngine;
using System.Collections;
using com.crownrace.msg;
using ProtoBuf;

public class Demo : MonoBehaviour {

	// Use this for initialization
	void Start () {

		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.LOGIN_ACK_CMD, LoginAck, "LoginAck");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq, "HeartbeatReq");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.LEAVE_GAME_NTF_CMD, PlayerLeaveNtf, "PlayerLeaveNtf");

		if(TcpClientHelper.Instance.Connect("192.168.1.173", 8000))
		{
			login_req req = new login_req();
			req.name = "zhangming";
			req.res_name = "123";
			TcpClientHelper.Instance.SendData<login_req>(NET_CMD.LOGIN_REQ_CMD, req);
		}
	}

	void LoginAck(byte[] data)
	{
		login_ack ack = NetUtils.Deserialize<login_ack>(data);
		GameGlobalData.PlayerID = ack.player_id;
		Debug.Log("LoginAck::player_id:"+ack.player_id);
	}

	void HeartbeatReq(byte[] data)
	{
		heartbeat_req hb = NetUtils.Deserialize<heartbeat_req> (data);
		Debug.Log ("HeartbeatReq:"+hb.server_timestamp);
		if (GameGlobalData.PlayerID >= 0) {
			heartbeat_ack ack = new heartbeat_ack ();
			ack.player_id = GameGlobalData.PlayerID;
			TcpClientHelper.Instance.SendData<heartbeat_ack> (NET_CMD.HEARTBEAT_ACK_CMD, ack);
		}
	}

	void PlayerLeaveNtf(byte[] data)
	{
		leave_game_ntf ntf = NetUtils.Deserialize<leave_game_ntf> (data);
		Debug.Log ("PlayerLeaveNtf:"+ntf.player_id);
		if (ntf.player_id == GameGlobalData.PlayerID) {
			TcpClientHelper.Instance.Close ();
		} else {
		
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
