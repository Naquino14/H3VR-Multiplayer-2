using UnityEngine;

namespace FistVR
{
	public class AIThrusterControlBox : FVRDestroyableObject
	{
		public AIThruster[] Thrusters;

		public bool Thrust(Vector3 thrust, ref float magnitude)
		{
			bool result = false;
			for (int i = 0; i < Thrusters.Length; i++)
			{
				if (Thrusters[i] != null)
				{
					if (Vector3.Dot(-Thrusters[i].transform.forward, thrust.normalized) > 0.5f)
					{
						result = true;
						magnitude += Thrusters[i].Thrust();
					}
					else
					{
						Thrusters[i].KillThrust();
					}
				}
			}
			return result;
		}

		public override void DestroyEvent()
		{
			for (int i = 0; i < Thrusters.Length; i++)
			{
				if (Thrusters[i] != null)
				{
				}
			}
			base.DestroyEvent();
		}
	}
}
