using System;
using UnityEngine;

namespace FistVR
{
	public class OpenBoltRPMFireSelector : MonoBehaviour
	{
		[Serializable]
		public class BoltSetting
		{
			public float ForwardSpeed;

			public float RearwardSpeed;

			public float Stiffness;
		}

		public OpenBoltReceiver Receiver;

		public OpenBoltReceiverBolt Bolt;

		public BoltSetting[] Settings;

		private void Update()
		{
			Bolt.BoltSpeed_Forward = Settings[Receiver.FireSelectorModeIndex].ForwardSpeed;
			Bolt.BoltSpeed_Rearward = Settings[Receiver.FireSelectorModeIndex].RearwardSpeed;
			Bolt.BoltSpringStiffness = Settings[Receiver.FireSelectorModeIndex].Stiffness;
		}
	}
}
