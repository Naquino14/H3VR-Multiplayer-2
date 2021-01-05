using UnityEngine;

namespace FistVR
{
	public class RevolverCylinder : FVRInteractiveObject
	{
		[Header("Revolver Cylinder Config")]
		public Revolver Revolver;

		private float m_fakeAngularVel;

		public int numChambers = 6;

		public float CartridgeLength = 0.04f;

		protected override void Awake()
		{
			base.Awake();
		}

		public override bool IsInteractable()
		{
			if (Revolver.isCylinderArmLocked)
			{
				return false;
			}
			return true;
		}

		public void LoadFromSpeedLoader(Speedloader loader)
		{
			bool flag = false;
			for (int i = 0; i < loader.Chambers.Count; i++)
			{
				if (i < Revolver.Chambers.Length && loader.Chambers[i].IsLoaded && !Revolver.Chambers[i].IsFull)
				{
					Revolver.Chambers[i].Autochamber(loader.Chambers[i].Unload());
					flag = true;
				}
			}
			if (flag)
			{
				Revolver.PlayAudioEvent(FirearmAudioEventType.MagazineIn);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = Revolver.transform.InverseTransformDirection(hand.Input.VelLinearWorld);
			Revolver.AddCylinderCloseVel((0f - vector.x) * 2800f);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_fakeAngularVel = (0f - Revolver.transform.InverseTransformDirection(hand.Input.VelLinearWorld).y) * 120f;
			m_fakeAngularVel = Mathf.Clamp(m_fakeAngularVel, -360f, 360f);
			base.EndInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!Revolver.isCylinderArmLocked)
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, base.transform.localEulerAngles.z + m_fakeAngularVel);
				if (Mathf.Abs(m_fakeAngularVel) > 0f)
				{
					m_fakeAngularVel = Mathf.Lerp(m_fakeAngularVel, 0f, Time.deltaTime * 0.8f);
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
			if (Revolver.isChiappa)
			{
				num += 180f;
			}
			num += 360f / (float)numChambers * 0.5f;
			num = Mathf.Repeat(num, 360f);
			return Mathf.CeilToInt(num / (360f / (float)numChambers)) - 1;
		}

		public Quaternion GetLocalRotationFromCylinder(int cylinder)
		{
			float num = (float)cylinder * (360f / (float)numChambers) * -1f;
			if (Revolver.isChiappa)
			{
				num += 180f;
			}
			num = Mathf.Repeat(num, 360f);
			return Quaternion.Euler(new Vector3(0f, 0f, num));
		}
	}
}
