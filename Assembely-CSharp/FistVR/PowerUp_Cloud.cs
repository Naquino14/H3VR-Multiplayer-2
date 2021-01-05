using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PowerUp_Cloud : MonoBehaviour
	{
		public LayerMask LM_Collide;

		public PowerupType PType;

		public PowerUpIntensity PIntensity;

		public PowerUpDuration PDuration;

		private bool PIsPuke;

		public bool PIsInverted;

		public float CloudRadius = 5f;

		private float m_tickTilCheck = 0.02f;

		private float timeTilGone = 1f;

		private bool m_hasChecked;

		public void SetParams(PowerupType t, PowerUpIntensity i, PowerUpDuration d, bool puke, bool inverted)
		{
			PType = t;
			PIntensity = i;
			PDuration = d;
			PIsPuke = puke;
			PIsInverted = inverted;
		}

		private void Update()
		{
			m_tickTilCheck -= Time.deltaTime;
			if (m_tickTilCheck <= 0f)
			{
				Check();
			}
			timeTilGone -= Time.deltaTime;
			if (timeTilGone <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void Check()
		{
			if (m_hasChecked)
			{
				return;
			}
			m_hasChecked = true;
			Collider[] array = Physics.OverlapSphere(base.transform.position, CloudRadius, LM_Collide, QueryTriggerInteraction.Ignore);
			HashSet<Sosig> hashSet = new HashSet<Sosig>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].attachedRigidbody == null))
				{
					SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
					if (component != null)
					{
						hashSet.Add(component.S);
					}
				}
			}
			foreach (Sosig item in hashSet)
			{
				item.ActivatePower(PType, PIntensity, PDuration, PIsPuke, PIsInverted);
			}
			if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) < CloudRadius)
			{
				GM.CurrentPlayerBody.ActivatePower(PType, PIntensity, PDuration, PIsPuke, PIsInverted);
			}
		}
	}
}
