using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_HistoryStep
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 velocity;

		public Vector3 angularVelocity;

		public long timeInTicks = -1L;
	}
}
