using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RotrwCharcoal : FVRPhysicalObject, IFVRDamageable
	{
		public enum RendererState
		{
			UnburnedNoFluid,
			UnburnedWet,
			Burning,
			BurntOut
		}

		public bool IsFluided;

		public bool IsOnFire;

		public ParticleSystem FireSystem;

		private float m_fuelLeft = 1f;

		private float m_fuelBurnDownTime = 0.005f;

		public RendererState State;

		public Renderer Rend_Unburned_Unfluided;

		public Renderer Rend_Unburned_Fluided;

		public Renderer Rend_Burning;

		public Renderer Rend_BurntOut;

		private float m_timeTilFireCheck = 1f;

		public LayerMask LM_DamMask;

		public AudioEvent AudEvent_Ignite;

		private bool m_isPSystemBurning;

		protected override void Start()
		{
			base.Start();
			m_fuelBurnDownTime = Random.Range(m_fuelBurnDownTime * 1f, m_fuelBurnDownTime * 1.2f);
		}

		private void SetRendererState(RendererState s)
		{
			if (State != s)
			{
				switch (s)
				{
				case RendererState.UnburnedWet:
					Rend_Unburned_Unfluided.enabled = false;
					Rend_Unburned_Fluided.enabled = true;
					Rend_Burning.enabled = false;
					Rend_BurntOut.enabled = false;
					break;
				case RendererState.Burning:
					Rend_Unburned_Unfluided.enabled = false;
					Rend_Unburned_Fluided.enabled = false;
					Rend_Burning.enabled = true;
					Rend_BurntOut.enabled = false;
					break;
				case RendererState.BurntOut:
					Rend_Unburned_Unfluided.enabled = false;
					Rend_Unburned_Fluided.enabled = false;
					Rend_Burning.enabled = false;
					Rend_BurntOut.enabled = true;
					break;
				}
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (IsOnFire)
			{
				UpdateFireEmission(5f);
				m_fuelLeft -= Time.deltaTime * m_fuelBurnDownTime;
				if (m_timeTilFireCheck > 0f)
				{
					m_timeTilFireCheck -= Time.deltaTime;
				}
				else
				{
					m_timeTilFireCheck = Random.Range(1f, 1.5f);
					DamageBubble();
				}
				if (m_fuelLeft < 0.1f)
				{
					SetRendererState(RendererState.BurntOut);
				}
				if (m_fuelLeft <= 0f)
				{
					PutOut();
				}
			}
			else
			{
				UpdateFireEmission(0f);
			}
		}

		private void DamageBubble()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f, LM_DamMask, QueryTriggerInteraction.Collide);
			List<IFVRDamageable> list = new List<IFVRDamageable>();
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					IFVRDamageable component = array[i].gameObject.GetComponent<IFVRDamageable>();
					if (component != null && !list.Contains(component))
					{
						list.Add(component);
					}
					FVRIgnitable component2 = array[i].transform.gameObject.GetComponent<FVRIgnitable>();
					if (component2 == null && array[i].attachedRigidbody != null)
					{
						array[i].attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
					}
					if (component2 != null)
					{
						FXM.Ignite(component2, 1f);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if ((MonoBehaviour)list[num] != null)
				{
					Damage damage = new Damage();
					damage.Dam_Thermal = 50f;
					damage.Dam_TotalEnergetic = 50f;
					damage.Class = FistVR.Damage.DamageClass.Explosive;
					damage.damageSize = 0.1f;
					damage.hitNormal = Random.onUnitSphere;
					damage.point = base.transform.position;
					damage.strikeDir = -damage.hitNormal;
					list[num].Damage(damage);
				}
			}
		}

		public void FluidMe()
		{
			IsFluided = true;
			if (State == RendererState.UnburnedNoFluid)
			{
				SetRendererState(RendererState.UnburnedWet);
			}
		}

		private void OnParticleCollision(GameObject other)
		{
			if (!IsFluided && other.CompareTag("LighterFluid"))
			{
				FluidMe();
			}
		}

		public void Ignite()
		{
			if (!IsOnFire && IsFluided && !(m_fuelLeft <= 0f))
			{
				IsOnFire = true;
				SetRendererState(RendererState.Burning);
				ParticleSystem.EmissionModule emission = FireSystem.emission;
				emission.enabled = true;
				UpdateFireEmission(5f);
				SM.PlayGenericSound(AudEvent_Ignite, base.transform.position);
			}
		}

		public void PutOut()
		{
			IsOnFire = false;
			m_fuelLeft = 0f;
			ParticleSystem.EmissionModule emission = FireSystem.emission;
			emission.enabled = false;
			SetRendererState(RendererState.BurntOut);
		}

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 0f)
			{
				Ignite();
			}
			if (d.Dam_TotalKinetic > 500f && m_fuelLeft < 0.25f)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void UpdateFireEmission(float f)
		{
			ParticleSystem.EmissionModule emission = FireSystem.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.mode = ParticleSystemCurveMode.Constant;
			rateOverTime.constant = f;
			emission.rateOverTime = rateOverTime;
		}
	}
}
