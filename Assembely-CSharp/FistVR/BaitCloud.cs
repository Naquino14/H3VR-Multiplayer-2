using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BaitCloud : MonoBehaviour
	{
		public enum PieEffect
		{
			Attractor,
			Repellant,
			Stun,
			Freeze,
			IFFScramble
		}

		public PieEffect Effect;

		public float EffectRadius = 10f;

		public LayerMask LM_Collide_AIBody;

		private float m_tickTilCheck = 0.02f;

		public float m_timeToRemoval = 30f;

		private HashSet<Sosig> m_baitedSosigs = new HashSet<Sosig>();

		private void Start()
		{
		}

		private void Update()
		{
			m_tickTilCheck -= Time.deltaTime;
			if (m_tickTilCheck <= 0f)
			{
				m_tickTilCheck = Random.Range(0.1f, 0.2f);
				Check();
			}
			m_timeToRemoval -= Time.deltaTime;
			if (m_timeToRemoval <= 0f)
			{
				Unbait();
				Object.Destroy(base.gameObject);
			}
		}

		private void Check()
		{
			if (Effect == PieEffect.Attractor)
			{
				m_baitedSosigs.Clear();
				Collider[] array = Physics.OverlapSphere(base.transform.position, EffectRadius, LM_Collide_AIBody, QueryTriggerInteraction.Collide);
				for (int i = 0; i < array.Length; i++)
				{
					if (!(array[i].attachedRigidbody == null))
					{
						SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (component.S.E.IFFCode != 0 && component.S.E.IFFCode != 2)
						{
							m_baitedSosigs.Add(component.S);
						}
					}
				}
				foreach (Sosig baitedSosig in m_baitedSosigs)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					onUnitSphere.y = 0f;
					baitedSosig.CommandAssaultPoint(base.transform.position + onUnitSphere * 1.2f);
					baitedSosig.SetCurrentOrder(Sosig.SosigOrder.Assault);
					baitedSosig.FallbackOrder = Sosig.SosigOrder.Assault;
					baitedSosig.SetAssaultPointOverrideDistance(0.1f);
				}
			}
			else if (Effect == PieEffect.Repellant)
			{
				Collider[] array2 = Physics.OverlapSphere(base.transform.position, EffectRadius, LM_Collide_AIBody, QueryTriggerInteraction.Collide);
				for (int j = 0; j < array2.Length; j++)
				{
					if (!(array2[j].attachedRigidbody == null))
					{
						SosigLink component2 = array2[j].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						component2.S.Blind(1f);
					}
				}
			}
			else if (Effect == PieEffect.Stun)
			{
				Collider[] array3 = Physics.OverlapSphere(base.transform.position, EffectRadius, LM_Collide_AIBody, QueryTriggerInteraction.Collide);
				for (int k = 0; k < array3.Length; k++)
				{
					if (!(array3[k].attachedRigidbody == null))
					{
						SosigLink component3 = array3[k].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						component3.S.Stun(2f);
						component3.S.Shudder(0.25f);
					}
				}
			}
			else if (Effect == PieEffect.Freeze)
			{
				Collider[] array4 = Physics.OverlapSphere(base.transform.position, EffectRadius, LM_Collide_AIBody, QueryTriggerInteraction.Collide);
				for (int l = 0; l < array4.Length; l++)
				{
					if (!(array4[l].attachedRigidbody == null))
					{
						SosigLink component4 = array4[l].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (component4.S != null)
						{
							component4.S.ActivatePower(PowerupType.ChillOut, PowerUpIntensity.High, PowerUpDuration.Blip, isPuke: false, isInverted: false);
						}
					}
				}
			}
			else
			{
				if (Effect != PieEffect.IFFScramble)
				{
					return;
				}
				Collider[] array5 = Physics.OverlapSphere(base.transform.position, EffectRadius, LM_Collide_AIBody, QueryTriggerInteraction.Collide);
				for (int m = 0; m < array5.Length; m++)
				{
					if (!(array5[m].attachedRigidbody == null))
					{
						SosigLink component5 = array5[m].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (component5.S != null && component5.S.BodyState != Sosig.SosigBodyState.Dead)
						{
							component5.S.E.IFFCode = Random.Range(3, 20);
						}
					}
				}
			}
		}

		private void Unbait()
		{
			if (Effect == PieEffect.Attractor || Effect == PieEffect.Repellant)
			{
				foreach (Sosig baitedSosig in m_baitedSosigs)
				{
					if (baitedSosig != null)
					{
						baitedSosig.SetAssaultPointOverrideDistance(50f);
						baitedSosig.SetCurrentOrder(Sosig.SosigOrder.Wander);
						baitedSosig.FallbackOrder = Sosig.SosigOrder.Wander;
					}
				}
			}
			m_baitedSosigs.Clear();
		}
	}
}
