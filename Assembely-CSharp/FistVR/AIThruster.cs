using UnityEngine;

namespace FistVR
{
	public class AIThruster : FVRDestroyableObject
	{
		[Header("Thruster Params")]
		public ParticleSystem SmallBooster;

		public ParticleSystem LargeBooster;

		public Rigidbody BaseRB;

		private float m_ThrustMagnitude;

		private int framesTilNextBoost;

		public AIThrusterControlBox ControlBox;

		public float GetMagnitude()
		{
			return m_ThrustMagnitude;
		}

		public float Thrust()
		{
			BaseRB.AddForceAtPosition(-base.transform.forward * m_ThrustMagnitude * 4f, base.transform.position);
			if (m_ThrustMagnitude < 0.8f)
			{
				m_ThrustMagnitude += Time.deltaTime * 16f;
			}
			else
			{
				m_ThrustMagnitude += Time.deltaTime * 1f;
			}
			m_ThrustMagnitude = Mathf.Clamp(m_ThrustMagnitude, 0f, 1f);
			return m_ThrustMagnitude;
		}

		public void KillThrust()
		{
			m_ThrustMagnitude = 0f;
		}

		public override void DestroyEvent()
		{
			base.DestroyEvent();
		}

		public override void Update()
		{
			base.Update();
			if (framesTilNextBoost > 0)
			{
				framesTilNextBoost--;
			}
			if (m_ThrustMagnitude > 0.8f)
			{
				SetBooster(LargeBooster, 10f);
				SetBooster(SmallBooster, 0f);
				if (framesTilNextBoost <= 0)
				{
					framesTilNextBoost = Random.Range(4, 8);
					if (GM.CurrentSceneSettings.IsSceneLowLight)
					{
						FXM.InitiateMuzzleFlash(LargeBooster.transform.position, base.transform.forward, Random.Range(0.25f, 1.5f), new Color(1f, 0.8f, 0.5f), Random.Range(1f, 2f));
					}
					else
					{
						FXM.InitiateMuzzleFlash(LargeBooster.transform.position, base.transform.forward, Random.Range(0.1f, 0.4f), new Color(1f, 0.8f, 0.5f), Random.Range(0.5f, 2f));
					}
				}
			}
			else if (m_ThrustMagnitude > 0f)
			{
				SetBooster(LargeBooster, 0f);
				SetBooster(SmallBooster, 10f);
			}
			else
			{
				SetBooster(LargeBooster, 0f);
				SetBooster(SmallBooster, 0f);
			}
			m_ThrustMagnitude -= Time.deltaTime * 2f;
			m_ThrustMagnitude = Mathf.Clamp(m_ThrustMagnitude, 0f, 1f);
		}

		private void SetBooster(ParticleSystem pSystem, float rate)
		{
			if (pSystem != null)
			{
				ParticleSystem.EmissionModule emission = pSystem.emission;
				ParticleSystem.MinMaxCurve rate2 = emission.rate;
				rate2.mode = ParticleSystemCurveMode.Constant;
				rate2.constantMax = rate;
				rate2.constantMin = rate;
				emission.rate = rate2;
			}
		}
	}
}
