using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ROFTester : MonoBehaviour, IFVRDamageable
	{
		public List<float> ShotTimes = new List<float>();

		public float RPM;

		public void Update()
		{
			if (Input.GetKey(KeyCode.R))
			{
				ShotTimes.Clear();
			}
			if (Input.GetKey(KeyCode.P))
			{
				Debug.Log("ROF: " + RPM);
			}
		}

		public void Damage(Damage dam)
		{
			ShotTimes.Add(Time.time);
			UpdateROF();
		}

		public void UpdateROF()
		{
			float num = ShotTimes[ShotTimes.Count - 1] - ShotTimes[0];
			float num2 = (float)ShotTimes.Count / num;
			RPM = num2 * 60f;
		}
	}
}
