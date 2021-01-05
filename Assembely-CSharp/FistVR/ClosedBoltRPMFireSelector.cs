using System;
using UnityEngine;

namespace FistVR
{
	public class ClosedBoltRPMFireSelector : MonoBehaviour
	{
		[Serializable]
		public class BoltSetting
		{
			public float ForwardSpeed;

			public float RearwardSpeed;

			public float Stiffness;
		}

		public ClosedBoltWeapon Receiver;

		public ClosedBolt Bolt;

		public BoltSetting[] Settings;

		private void Update()
		{
			Bolt.Speed_Forward = Settings[Receiver.FireSelectorModeIndex].ForwardSpeed;
			Bolt.Speed_Rearward = Settings[Receiver.FireSelectorModeIndex].RearwardSpeed;
			Bolt.SpringStiffness = Settings[Receiver.FireSelectorModeIndex].Stiffness;
		}
	}
}
