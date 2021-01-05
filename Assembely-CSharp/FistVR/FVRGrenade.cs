using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRGrenade : FVRPhysicalObject
	{
		public Transform FuseCylinder;

		private int m_fuseCylinderSetting = 2;

		private float m_fuseCurYRotation;

		private float m_fuseTarYRotation;

		public FVRGrenadePin Pin;

		private bool m_isPinPulled;

		public bool Uses2ndPin;

		public FVRGrenadePin Pin2;

		private bool m_isPinPulled2;

		public HingeJoint LeverJoint;

		private bool m_isLeverReleased;

		public GameObject FakeHandle;

		public GameObject RealHandle;

		private Dictionary<int, float> FuseTimings = new Dictionary<int, float>();

		private bool m_isFused;

		private float m_startFuseTime = 15f;

		private float m_fuseTime = 15f;

		public float DefaultFuse = 3.3f;

		private float m_fuse_tick;

		private float m_fuse_PitchStart = 1.8f;

		private float m_fuse_PitchEnd = 3.7f;

		private float m_fuse_StartRefire = 0.4f;

		private float m_fuse_EndRefire = 0.02f;

		public GameObject ExplosionFX;

		public GameObject ExplosionSoundFX;

		public AudioSource FuseAudio;

		public ParticleSystem FusePSystem;

		private Vector3 m_releasePoint;

		public bool FuseOnSpawn;

		public SmokeSolidEmitter SmokeEmitter;

		public AudioEvent AudioEvent_StrikerActivate;

		public AudioEvent AudioEvent_SafetyLeverRelease;

		public AudioEvent AudioEvent_Pinpull;

		private bool m_hasSploded;

		public int IFF;

		public override int GetTutorialState()
		{
			if (m_isPinPulled)
			{
				return 1;
			}
			return 0;
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			FVRGrenade component = gameObject.GetComponent<FVRGrenade>();
			component.SetFuseSetting(m_fuseCylinderSetting);
			return gameObject;
		}

		public void SetFuseSetting(int i)
		{
			m_fuseCylinderSetting = i;
		}

		private void IncreaseFuseSetting()
		{
			if (m_fuseCylinderSetting < 4)
			{
				m_fuseCylinderSetting++;
			}
			else
			{
				m_fuseCylinderSetting = 0;
			}
			m_fuseTarYRotation = (float)m_fuseCylinderSetting * 24f - 48f;
		}

		private void DecreaseFuseSetting()
		{
			if (m_fuseCylinderSetting > 0)
			{
				m_fuseCylinderSetting--;
			}
			m_fuseTarYRotation = (float)m_fuseCylinderSetting * 24f - 48f;
		}

		public void PullPin()
		{
			m_isPinPulled = true;
			SM.PlayGenericSound(AudioEvent_Pinpull, base.transform.position);
			if (!base.IsHeld && (!Uses2ndPin || m_isPinPulled2))
			{
				ReleaseLever();
			}
		}

		public void PullPin2()
		{
			m_isPinPulled2 = true;
			SM.PlayGenericSound(AudioEvent_Pinpull, base.transform.position);
			if (!base.IsHeld && m_isPinPulled)
			{
				ReleaseLever();
			}
		}

		public void ReleaseLever()
		{
			if (!m_isLeverReleased)
			{
				SM.PlayGenericSound(AudioEvent_StrikerActivate, base.transform.position);
				SM.PlayGenericSound(AudioEvent_SafetyLeverRelease, base.transform.position);
				m_isLeverReleased = true;
				FakeHandle.SetActive(value: false);
				RealHandle.SetActive(value: true);
				JointSpring spring = LeverJoint.spring;
				spring.targetPosition = -135f;
				spring.spring = 4f;
				LeverJoint.spring = spring;
				m_isFused = true;
				if (FuseCylinder != null)
				{
					m_startFuseTime = FuseTimings[m_fuseCylinderSetting];
					m_fuseTime = FuseTimings[m_fuseCylinderSetting];
				}
				else
				{
					m_startFuseTime = DefaultFuse;
					m_fuseTime = DefaultFuse;
				}
				m_releasePoint = base.transform.position;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			FuseTimings.Add(0, 12f);
			FuseTimings.Add(1, 8f);
			FuseTimings.Add(2, 5f);
			FuseTimings.Add(3, 3f);
			FuseTimings.Add(4, 2f);
			if (FuseOnSpawn)
			{
				Invoke("PullPin", 0.1f);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (FuseCylinder != null)
			{
				m_fuseCurYRotation = Mathf.Lerp(m_fuseCurYRotation, m_fuseTarYRotation, Time.deltaTime * 8f);
				FuseCylinder.localEulerAngles = new Vector3(0f, m_fuseCurYRotation, 0f);
			}
			if (LeverJoint != null && m_isLeverReleased && Mathf.Abs(LeverJoint.angle) > 130f)
			{
				LeverJoint.transform.SetParent(null);
				Object.Destroy(LeverJoint);
			}
			if (!m_isFused)
			{
				return;
			}
			m_fuseTime -= Time.deltaTime;
			if (FuseCylinder != null)
			{
				float f = Mathf.Clamp(1f - m_fuseTime / m_startFuseTime, 0f, 1f);
				f = Mathf.Pow(f, 2f);
				if (m_fuse_tick <= 0f)
				{
					m_fuse_tick = Mathf.Lerp(m_fuse_StartRefire, m_fuse_EndRefire, f);
					FuseAudio.pitch = Mathf.Lerp(m_fuse_PitchStart, m_fuse_PitchEnd, f);
					FuseAudio.PlayOneShot(FuseAudio.clip);
					FusePSystem.Emit(2);
				}
				else
				{
					m_fuse_tick -= Time.deltaTime;
				}
			}
			if (!(m_fuseTime <= 0.05f))
			{
				return;
			}
			if (!m_hasSploded)
			{
				m_hasSploded = true;
				if (ExplosionFX != null)
				{
					GameObject gameObject = Object.Instantiate(ExplosionFX, base.transform.position, Quaternion.identity);
					Explosion component = gameObject.GetComponent<Explosion>();
					if (component != null)
					{
						component.IFF = IFF;
					}
					ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
					if (component2 != null)
					{
						component2.IFF = IFF;
					}
				}
				if (ExplosionSoundFX != null)
				{
					GameObject gameObject2 = Object.Instantiate(ExplosionSoundFX, base.transform.position, Quaternion.identity);
					Explosion component3 = gameObject2.GetComponent<Explosion>();
					if (component3 != null)
					{
						component3.IFF = IFF;
					}
					ExplosionSound component4 = gameObject2.GetComponent<ExplosionSound>();
					if (component4 != null)
					{
						component4.IFF = IFF;
					}
				}
			}
			if (SmokeEmitter != null)
			{
				SmokeEmitter.Engaged = true;
				return;
			}
			if (base.IsHeld)
			{
				FVRViveHand hand = m_hand;
				m_hand.ForceSetInteractable(null);
				EndInteraction(hand);
			}
			Object.Destroy(base.gameObject);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			IFF = GM.CurrentPlayerBody.GetPlayerIFF();
			bool flag = false;
			if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
			{
				flag = true;
			}
			else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
			{
				flag = true;
			}
			if (FuseCylinder != null && !m_isPinPulled && flag)
			{
				IncreaseFuseSetting();
			}
			bool flag2 = false;
			if (Uses2ndPin)
			{
				if (m_isPinPulled && m_isPinPulled2 && !m_isLeverReleased)
				{
					flag2 = true;
				}
			}
			else if (m_isPinPulled && !m_isLeverReleased)
			{
				flag2 = true;
			}
			if (flag2 && flag)
			{
				ReleaseLever();
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			bool flag = false;
			if (Uses2ndPin)
			{
				if (m_isPinPulled && m_isPinPulled2 && !m_isLeverReleased)
				{
					flag = true;
				}
			}
			else if (m_isPinPulled && !m_isLeverReleased)
			{
				flag = true;
			}
			if (flag)
			{
				ReleaseLever();
			}
		}
	}
}
