using UnityEngine;

namespace FistVR
{
	public class LAPD2019Cylinder : FVRInteractiveObject
	{
		[Header("Revolver Cylinder Config")]
		public LAPD2019 Gun;

		private float m_fakeAngularVel;

		public int numChambers = 5;

		protected override void Awake()
		{
			base.Awake();
		}

		public override bool IsInteractable()
		{
			if (Gun.isCylinderArmLocked)
			{
				return false;
			}
			return true;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = Gun.transform.InverseTransformDirection(hand.Input.VelLinearWorld);
			Gun.AddCylinderCloseVel((0f - vector.x) * 2800f);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_fakeAngularVel = (0f - Gun.transform.InverseTransformDirection(hand.Input.VelLinearWorld).y) * 120f;
			m_fakeAngularVel = Mathf.Clamp(m_fakeAngularVel, -360f, 360f);
			base.EndInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!Gun.isCylinderArmLocked)
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, base.transform.localEulerAngles.z + m_fakeAngularVel * 30f * Time.deltaTime);
				if (Mathf.Abs(m_fakeAngularVel) > 0f)
				{
					m_fakeAngularVel = Mathf.Lerp(m_fakeAngularVel, 0f, Time.deltaTime * 0.8f);
				}
				if (!(Mathf.Abs(m_fakeAngularVel) > 5f))
				{
				}
			}
			else
			{
				m_fakeAngularVel = 0f;
			}
		}

		public int GetClosestChamberIndex()
		{
			float num = 0f - base.transform.localEulerAngles.z;
			num += 360f / (float)numChambers * 0.5f;
			num = Mathf.Repeat(num, 360f);
			return Mathf.CeilToInt(num / (360f / (float)numChambers)) - 1;
		}

		public Quaternion GetLocalRotationFromCylinder(int cylinder)
		{
			float t = (float)cylinder * (360f / (float)numChambers) * -1f;
			t = Mathf.Repeat(t, 360f);
			return Quaternion.Euler(new Vector3(0f, 0f, t));
		}
	}
}
