using UnityEngine;
using System.Collections;
using System;

public class GameUtils {

	public static ulong GetUTCTimeStamp()
	{
		return (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
	}
}
