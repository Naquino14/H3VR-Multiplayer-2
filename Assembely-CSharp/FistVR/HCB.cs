using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HCB : FVRFireArm
	{
		public enum SledState
		{
			Forward,
			Winding,
			Rear
		}

		[Header("Component Connections")]
		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Vector2 TriggerRotRange;

		private Renderer ProxyRend;

		private float m_cookedAmount;

		private float m_cookSpeed = 1f;

		[Header("Sled String Part")]
		public Transform Sled;

		public List<Transform> Strings;

		public List<Transform> StringTargets;

		public Transform BoltHolder;

		public Transform SledPos_Forward;

		public Transform SledPos_Rearward;

		public Vector2 HolderRotRange;

		private bool m_isLoaded;

		[Header("Roller Parts")]
		public List<Transform> Rollers;

		private bool m_isPowered;

		private float m_rollingRot;

		private float m_shotCooldown = 0.3f;

		public GameObject BoltPrefab;

		private SledState m_sledState;

		private float m_sledLerp;

		public float SledLerpSpeed = 3f;

		private float m_rollingSpeedMag = 1f;

		[Header("AudioEvents")]
		public AudioEvent AudEvent_SledStart;

		public AudioEvent AudEvent_SledComplete;

		public AudioEvent AudEvent_PowerOn;

		public AudioEvent AudEvent_PowerOut;

		public AudioEvent AudEvent_Char;

		public AudioEvent AudEvent_Fire;

		[Header("VFX")]
		public ParticleSystem PSystem;

		public AudioSource AudSource_Sizzle;

		public ParticleSystem PSystem_Sparks;

		public AudioEvent AudEvent_Spark;

		private float m_timeTilSpark = 0.04f;

		private bool m_isCooking;

		protected override void Start()
		{
			base.Start();
			Sled.localPosition = SledPos_Forward.localPosition;
			UpdateStrings();
			ProxyRend = Chamber.ProxyRenderer;
		}

		private void UpdateStrings()
		{
			for (int i = 0; i < Strings.Count; i++)
			{
				Vector3 forward = StringTargets[i].position - Strings[i].position;
				Strings[i].rotation = Quaternion.LookRotation(forward, base.transform.up);
				Strings[i].localScale = new Vector3(1f, 1f, forward.magnitude);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			bool isPowered = m_isPowered;
			if (Magazine == null || !Magazine.HasFuel())
			{
				m_isPowered = false;
			}
			else
			{
				m_isPowered = true;
			}
			if (m_isPowered && !isPowered)
			{
				m_shotCooldown = 0.3f;
				PlayAudioAsHandling(AudEvent_PowerOn, Sled.transform.position);
			}
			else if (!m_isPowered && isPowered)
			{
				PlayAudioAsHandling(AudEvent_PowerOut, Sled.transform.position);
			}
			if (m_isPowered)
			{
				m_rollingRot += Time.deltaTime * 90f * m_rollingSpeedMag;
				m_rollingRot = Mathf.Repeat(m_rollingRot, 360f);
				for (int i = 0; i < Rollers.Count; i++)
				{
					Rollers[i].localEulerAngles = new Vector3(0f, 0f, m_rollingRot);
				}
				Chamber.ProxyMesh.transform.localEulerAngles = new Vector3(0f, 0f, m_rollingRot);
			}
			if (m_isPowered)
			{
				Magazine.DrainFuel(Time.deltaTime * 0.05f);
			}
			if (base.IsHeld && m_shotCooldown > 0f)
			{
				m_shotCooldown -= Time.deltaTime;
			}
			switch (m_sledState)
			{
			case SledState.Forward:
				Chamber.IsAccessible = false;
				if (m_isPowered && m_shotCooldown <= 0f && base.IsHeld && !IsAltHeld)
				{
					m_sledLerp = 0f;
					m_sledState = SledState.Winding;
					PlayAudioAsHandling(AudEvent_SledStart, Sled.transform.position);
				}
				break;
			case SledState.Winding:
				Chamber.IsAccessible = false;
				m_shotCooldown = 0.3f;
				if (m_isPowered)
				{
					m_sledLerp += SledLerpSpeed * Time.deltaTime;
					Sled.localPosition = Vector3.Lerp(SledPos_Forward.localPosition, SledPos_Rearward.localPosition, m_sledLerp);
					UpdateStrings();
					if (m_sledLerp > 1f)
					{
						m_sledLerp = 1f;
						m_sledState = SledState.Rear;
						PlayAudioAsHandling(AudEvent_SledComplete, Sled.transform.position);
					}
				}
				if (m_isPowered && !isPowered)
				{
					PlayAudioAsHandling(AudEvent_SledStart, Sled.transform.position);
				}
				break;
			case SledState.Rear:
				Chamber.IsAccessible = true;
				m_shotCooldown = 0.3f;
				break;
			}
			bool isLoaded = m_isLoaded;
			if (Chamber.IsFull)
			{
				m_isLoaded = true;
			}
			else
			{
				m_isLoaded = false;
			}
			if (m_isLoaded && !isLoaded)
			{
				SetAnimatedComponent(BoltHolder, HolderRotRange.y, InterpStyle.Rotation, Axis.X);
			}
			else if (!m_isLoaded && isLoaded)
			{
				SetAnimatedComponent(BoltHolder, HolderRotRange.x, InterpStyle.Rotation, Axis.X);
			}
			if (m_isCooking)
			{
				m_rollingSpeedMag = m_cookedAmount * 5f;
				if (!Chamber.IsFull)
				{
					return;
				}
				PSystem.Emit(4);
				if (!AudSource_Sizzle.isPlaying)
				{
					AudSource_Sizzle.Play();
				}
				if (m_cookedAmount < 1f)
				{
					AddCookedAmount(Time.deltaTime * 0.5f);
					m_timeTilSpark -= Time.deltaTime;
					if (m_timeTilSpark <= 0f)
					{
						m_timeTilSpark = Random.Range(0.04f, 0.3f);
						PSystem_Sparks.Emit(4);
						PlayAudioAsHandling(AudEvent_Spark, PSystem_Sparks.transform.position);
					}
				}
			}
			else
			{
				m_rollingSpeedMag = 1f;
				if (AudSource_Sizzle.isPlaying)
				{
					AudSource_Sizzle.Stop();
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			bool isCooking = false;
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin && m_sledState == SledState.Rear)
			{
				if (hand.Input.TriggerPressed)
				{
					isCooking = true;
				}
				if (m_isCooking && hand.Input.TriggerUp)
				{
					ReleaseSled();
				}
			}
			m_isCooking = isCooking;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (m_isCooking)
			{
				ReleaseSled();
			}
			base.EndInteraction(hand);
			m_isCooking = false;
		}

		private void SetCookedAmount(float f)
		{
			m_cookedAmount = 0f;
			m_cookedAmount = Mathf.Clamp(m_cookedAmount, 0f, 1f);
			ProxyRend.material.SetFloat("_BlendScale", m_cookedAmount);
		}

		private void AddCookedAmount(float f)
		{
			m_cookedAmount += f;
			m_cookedAmount = Mathf.Clamp(m_cookedAmount, 0f, 1f);
			ProxyRend.material.SetFloat("_BlendScale", m_cookedAmount);
		}

		private void ReleaseSled()
		{
			m_sledState = SledState.Forward;
			Sled.localPosition = SledPos_Forward.localPosition;
			UpdateStrings();
			if (Chamber.IsFull)
			{
				GameObject gameObject = Object.Instantiate(BoltPrefab, MuzzlePos.position, MuzzlePos.rotation);
				HCBBolt component = gameObject.GetComponent<HCBBolt>();
				component.Fire(MuzzlePos.forward, MuzzlePos.position, 1f);
				component.SetCookedAmount(m_cookedAmount);
				Chamber.SetRound(null);
			}
			SetCookedAmount(0f);
			PlayAudioAsHandling(AudEvent_Fire, Sled.transform.position);
		}
	}
}
