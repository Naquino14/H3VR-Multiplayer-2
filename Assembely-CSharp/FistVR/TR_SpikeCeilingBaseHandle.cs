using UnityEngine;

namespace FistVR
{
	public class TR_SpikeCeilingBaseHandle : FVRInteractiveObject
	{
		private float m_curRot;

		public GameObject JackBase;

		public Transform HandleBase;

		private IMG_HandlePumpable Pumpable;

		public AudioSource audsource;

		private float curVolume;

		private float tarVolume;

		public bool SendsPositiveDelta = true;

		protected override void Awake()
		{
			base.Awake();
			Pumpable = JackBase.GetComponent<IMG_HandlePumpable>();
			m_curRot = Random.Range(-33f, 33f);
			base.transform.localEulerAngles = new Vector3(m_curRot, 0f, 0f);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - HandleBase.position;
			vector = Vector3.ProjectOnPlane(vector, HandleBase.right);
			float num = Vector3.Angle(vector, HandleBase.forward);
			if (Vector3.Dot(Vector3.up, vector.normalized) > 0f)
			{
				num = 0f - num;
			}
			num = Mathf.Clamp(num, -33f, 33f);
			float num2 = Mathf.Abs(num - m_curRot);
			Pump(num2);
			if (SendsPositiveDelta)
			{
				Pumpable.Pump(num2);
			}
			else
			{
				Pumpable.Pump(0f - num2);
			}
			m_curRot = num;
			base.transform.localEulerAngles = new Vector3(m_curRot, 0f, 0f);
		}

		private void Pump(float delta)
		{
			tarVolume += delta * 0.02f;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			tarVolume -= Time.deltaTime * 5f;
			audsource.volume = tarVolume;
			if (tarVolume <= 0f)
			{
				tarVolume = 0f;
				if (audsource.isPlaying)
				{
					audsource.Stop();
				}
			}
			else if (!audsource.isPlaying)
			{
				audsource.Play();
			}
		}
	}
}
