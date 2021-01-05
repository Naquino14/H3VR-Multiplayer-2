using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TrapLever : FVRInteractiveObject
	{
		public float MaxRot = 30f;

		public Transform RefVector;

		public float ValvePos = 0.5f;

		public List<GameObject> MessageTargets;

		public AudioEvent AudEvent_Release;

		public Transform Lever;

		protected override void Awake()
		{
			base.Awake();
			ValvePos = 0f;
			Lever.localEulerAngles = new Vector3(0f, 0f - MaxRot, 0f);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - Lever.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, Lever.up);
			float a = Vector3.Angle(RefVector.forward, vector2);
			Vector3 vector3 = Vector3.RotateTowards(RefVector.forward, vector2, Mathf.Min(a, MaxRot) * 0.0174533f, 0f);
			Lever.rotation = Quaternion.LookRotation(vector3, RefVector.up);
			float num = Vector3.Angle(RefVector.right, vector3);
			ValvePos = (num - 90f) / (MaxRot * 2f) + 0.5f;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Release, base.transform.position);
			if (ValvePos < 0.5f)
			{
				ValvePos = 1f;
				Lever.localEulerAngles = new Vector3(0f, MaxRot, 0f);
				for (int i = 0; i < MessageTargets.Count; i++)
				{
					MessageTargets[i].BroadcastMessage("ON", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				ValvePos = 0f;
				Lever.localEulerAngles = new Vector3(0f, 0f - MaxRot, 0f);
				for (int j = 0; j < MessageTargets.Count; j++)
				{
					MessageTargets[j].BroadcastMessage("OFF", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
