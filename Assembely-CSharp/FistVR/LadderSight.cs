using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class LadderSight : FVRInteractiveObject
	{
		public Transform Ladder;

		public Transform Bead;

		public List<string> RangeNames;

		public List<float> LadderRots;

		public List<Vector3> BeadLocalPoses;

		public int Setting = 1;

		public OpticUI UI;

		public Vector3 offset = new Vector3(0f, 0.02f, -0.01f);

		protected override void Start()
		{
			base.Start();
			UpdateRots();
			Vector3 vector = base.transform.up * offset.y + base.transform.forward * offset.z;
			GameObject gameObject = Object.Instantiate(ManagerSingleton<AM>.Instance.Prefab_OpticUI, base.transform.position + vector, base.transform.rotation);
			UI = gameObject.GetComponent<OpticUI>();
			UI.UpdateUI(this);
			gameObject.SetActive(value: false);
			UXGeo_Held = gameObject;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.IsHeld)
			{
				Vector3 vector = base.transform.up * offset.y + base.transform.forward * offset.z;
				UI.transform.position = base.transform.position + vector;
				UI.transform.rotation = base.transform.rotation;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					SettingDown();
				}
				if (hand.Input.AXButtonDown)
				{
					SettingUp();
				}
			}
			else if (hand.Input.TouchpadDown)
			{
				if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45f)
				{
					SettingDown();
				}
				else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45f)
				{
					SettingUp();
				}
			}
		}

		private void SettingDown()
		{
			Setting--;
			if (Setting < 0)
			{
				Setting = 0;
			}
			UpdateRots();
			UI.UpdateUI(this);
		}

		private void SettingUp()
		{
			Setting++;
			if (Setting >= LadderRots.Count)
			{
				Setting = LadderRots.Count - 1;
			}
			UpdateRots();
			UI.UpdateUI(this);
		}

		[ContextMenu("UpdateRots")]
		private void UpdateRots()
		{
			Ladder.localEulerAngles = new Vector3(LadderRots[Setting], 0f, 0f);
			Bead.localPosition = BeadLocalPoses[Setting];
		}
	}
}
