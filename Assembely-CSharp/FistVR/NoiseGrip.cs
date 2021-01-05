using UnityEngine;

namespace FistVR
{
	public class NoiseGrip : MonoBehaviour
	{
		public AudioSource AudSource;

		private float m_volume;

		private float m_actualVolume;

		public LayerMask DetectMask;

		public float Radius = 1.25f;

		public Transform VolumeCenter;

		public void ProcessInput(FVRViveHand hand, FVRInteractiveObject o)
		{
			if (o.m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
			{
				if (AudSource.isPlaying)
				{
					AudSource.Stop();
				}
				AudSource.Play();
				Horn();
				int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
				GM.CurrentSceneSettings.OnPerceiveableSound(80f, 32f, base.transform.position, playerIFF);
			}
			if (o.m_hasTriggeredUpSinceBegin && hand.Input.TriggerPressed)
			{
				m_actualVolume = 1f;
			}
		}

		private void Update()
		{
			Vector3 from = GM.CurrentPlayerBody.Head.position - base.transform.position;
			float num = Vector3.Angle(from, base.transform.forward);
			float num2 = (180f - num) / 180f;
			m_volume = 0.25f + num2 * 0.75f;
			AudSource.volume = m_volume * m_actualVolume;
		}

		private void Horn()
		{
			Collider[] array = Physics.OverlapSphere(VolumeCenter.position, Radius, DetectMask, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].attachedRigidbody == null))
				{
					SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
					if (component != null && component.BodyPart == SosigLink.SosigBodyPart.Head)
					{
						Damage damage = new Damage();
						damage.Dam_Stunning = 10f;
						component.Damage(damage);
					}
				}
			}
		}
	}
}
