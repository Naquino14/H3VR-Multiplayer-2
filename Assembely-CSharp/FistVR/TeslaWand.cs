using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TeslaWand : FVRMeleeWeapon
	{
		public List<GameObject> LightningFX;

		public Transform LightningOrigin;

		public LayerMask LM_SosigDetect;

		public LayerMask LM_EnvBlock;

		public float range;

		public AudioEvent AudEvent_Lightning;

		public GameObject SplodeOut;

		private int m_charges = 10;

		protected override void Start()
		{
			base.Start();
			m_charges = Random.Range(20, 500);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (base.AltGrip != null && !IsAltHeld && hand.Input.TriggerDown)
			{
				Blast();
			}
		}

		private void Blast()
		{
			Debug.Log("b");
			Vector3 position = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.forward * 8f;
			Collider[] array = Physics.OverlapSphere(position, 10f, LM_SosigDetect, QueryTriggerInteraction.Collide);
			Debug.Log(array.Length);
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].attachedRigidbody == null)
				{
					continue;
				}
				SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
				Vector3 pos = Vector3.zero;
				if (component != null && component.S != null)
				{
					if (component.S.BodyState == Sosig.SosigBodyState.Dead || Physics.Linecast(LightningOrigin.position, component.transform.position, LM_EnvBlock))
					{
						continue;
					}
					component.S.Shudder(2f);
					component.R.AddForce(Random.onUnitSphere * Random.Range(5f, 8f), ForceMode.Impulse);
					Vector3 forward = component.transform.position - LightningOrigin.position;
					GameObject gameObject = Object.Instantiate(LightningFX[Random.Range(0, LightningFX.Count)], LightningOrigin.position, Quaternion.LookRotation(forward, Random.onUnitSphere));
					float magnitude = forward.magnitude;
					FVRIgnitable component2 = component.gameObject.GetComponent<FVRIgnitable>();
					if (component2 != null)
					{
						FXM.Ignite(component2, 1f);
					}
					gameObject.transform.localScale = new Vector3(magnitude * 1.2f, magnitude * 1.2f, magnitude * 1.2f);
					flag = true;
					pos = component.transform.position;
				}
				if (flag)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Lightning, LightningOrigin.position);
					m_charges--;
					if (m_charges < 0)
					{
						Object.Instantiate(SplodeOut, base.transform.position, base.transform.rotation);
						ForceBreakInteraction();
						Object.Destroy(base.gameObject);
					}
					FXM.InitiateMuzzleFlash(pos, Random.onUnitSphere, 10f, Color.yellow, Random.Range(1f, 5f));
				}
				break;
			}
		}
	}
}
