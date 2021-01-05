using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class LightFluid : FVRPhysicalObject, IFVRDamageable
	{
		public ParticleSystem FluidSystem;

		public AudioEvent AudEvent_Gush;

		private float fluidGush;

		public List<GameObject> SpawnsOnDestroy;

		private bool m_isExploded;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.Input.TriggerDown)
			{
				fluidGush = 20f;
				SM.PlayGenericSound(AudEvent_Gush, base.transform.position);
			}
		}

		public void Damage(Damage d)
		{
			if (d.Dam_TotalKinetic > 500f || d.Dam_TotalEnergetic > 20f)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				for (int i = 0; i < SpawnsOnDestroy.Count; i++)
				{
					Object.Instantiate(SpawnsOnDestroy[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (fluidGush > 0f)
			{
				fluidGush -= Time.deltaTime * 40f;
				fluidGush = Mathf.Clamp(fluidGush, 0f, fluidGush);
				ParticleSystem.EmissionModule emission = FluidSystem.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.mode = ParticleSystemCurveMode.Constant;
				rateOverTime.constant = fluidGush;
				emission.rateOverTime = rateOverTime;
			}
		}
	}
}
