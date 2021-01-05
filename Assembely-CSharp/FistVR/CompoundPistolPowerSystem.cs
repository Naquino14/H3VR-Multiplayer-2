using UnityEngine;

namespace FistVR
{
	public class CompoundPistolPowerSystem : MonoBehaviour
	{
		[GradientHDR]
		public Gradient colorGrad;

		public Renderer Rend;

		public Handgun Hangun;

		public AudioEvent AudEvent_Overheat;

		public AudioEvent AudEvent_AllSet;

		public ParticleSystem PSystem_Overheat;

		public ParticleSystem PSystem_Overheat2;

		private float m_heat;

		private float m_timeSinceLastShot = 1f;

		private bool m_isOverheating;

		private float m_coolTime = 3.5f;

		private void OnShotFired(FVRFireArm firearm)
		{
			if (firearm == Hangun)
			{
				AddHeat();
				PSystem_Overheat.Emit(5);
				PSystem_Overheat2.Emit(5);
			}
		}

		private void AddHeat()
		{
			m_heat += 0.1f;
			m_timeSinceLastShot = 0f;
			if (m_heat >= 1f && !m_isOverheating)
			{
				Overheat();
			}
			m_heat = Mathf.Clamp(m_heat, 0f, 1f);
		}

		private void Overheat()
		{
			m_isOverheating = true;
			Hangun.Magazine.ForceEmpty();
			m_coolTime = 3.5f;
			FVRPooledAudioSource fVRPooledAudioSource = Hangun.PlayAudioAsHandling(AudEvent_Overheat, base.transform.position);
			fVRPooledAudioSource.FollowThisTransform(base.transform);
		}

		private void Reset()
		{
			m_isOverheating = false;
			Hangun.Magazine.ForceFull();
			Hangun.DropSlideRelease();
			m_heat = 0f;
			FVRPooledAudioSource fVRPooledAudioSource = Hangun.PlayAudioAsHandling(AudEvent_AllSet, base.transform.position);
			fVRPooledAudioSource.FollowThisTransform(base.transform);
		}

		private void Update()
		{
			Hangun.IsSlideLockExternalHeldDown = false;
			if (!m_isOverheating)
			{
				if (m_timeSinceLastShot < 0.3f)
				{
					m_timeSinceLastShot += Time.deltaTime;
				}
				else if (m_heat > 0f)
				{
					m_heat -= Time.deltaTime;
				}
			}
			else
			{
				PSystem_Overheat.Emit(1);
				if (m_coolTime > 0f)
				{
					m_coolTime -= Time.deltaTime;
				}
				else
				{
					Reset();
				}
			}
			float y = Mathf.Lerp(0.5f, -0.5f, m_heat);
			Rend.material.SetColor("_EmissionColor", colorGrad.Evaluate(m_heat));
			Rend.material.SetTextureOffset("_IncandescenceMap", new Vector2(0f, y));
		}

		private void Start()
		{
			GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
		}

		private void OnDisable()
		{
			GM.CurrentSceneSettings.ShotFiredEvent -= OnShotFired;
		}
	}
}
