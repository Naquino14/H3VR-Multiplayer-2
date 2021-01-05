using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GBeamer : FVRFireArm
	{
		public enum BeamerStatusLight
		{
			Red,
			Yellow,
			Green
		}

		public enum BeamerPowerState
		{
			Off,
			On
		}

		[Header("Beamer")]
		public GBeamerModeLever Lever;

		public Renderer Light0;

		public Renderer Light1;

		public Renderer Light2;

		public Material MatLightGreen;

		public Material MatLightYellow;

		public Material MatLightRed;

		public FVRBeamerSwitch BatterySwitch;

		public FVRBeamerSwitch CapacitorSwitch;

		public FVRBeamerSwitch MotorSwitch;

		private bool m_isBatterySwitchedOn;

		private bool m_isCapacitorSwitchedOn;

		private bool m_isMotorSwitchedOn;

		private float m_capacitorCharge;

		private BeamerStatusLight m_batteryLight;

		private BeamerStatusLight m_capacitorLight;

		private BeamerStatusLight m_motorLight;

		private float curMotorSpeed;

		private float tarMotorSpeed;

		private List<float> m_rots = new List<float>
		{
			0f,
			0f,
			0f
		};

		private BeamerPowerState m_powerState;

		[Header("Transforms")]
		public Transform Aperture;

		public Transform[] SpinnyParts;

		[Header("VFX")]
		public ParticleSystem RotorParticles;

		public ParticleSystem WindParticles;

		public List<Transform> Flaps;

		[Header("Audio")]
		public AudioEvent AudEvent_Capacitor;

		public AudioSource AudioMotor;

		public AudioSource AudioDrone;

		private bool m_isManipulating;

		private bool m_hasObject;

		private FVRPhysicalObject m_obj;

		[Header("ObjectSearch")]
		public LayerMask LM_ObjectHunt;

		public LayerMask LM_Blockers;

		public GameObject ShuntSplosion;

		public Transform ShuntSplosionPos;

		public Transform Locus;

		private RaycastHit m_hit;

		private HashSet<FVRPhysicalObject> pos = new HashSet<FVRPhysicalObject>();

		private float m_timeSinceLastSearch = 0.1f;

		private bool m_isObjSpinner;

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn && m_isMotorSwitchedOn)
			{
				m_powerState = BeamerPowerState.On;
			}
			else
			{
				m_powerState = BeamerPowerState.Off;
			}
			if (m_powerState == BeamerPowerState.Off)
			{
				tarMotorSpeed = 0f;
				m_hasObject = false;
				m_isManipulating = false;
			}
			else if (m_isManipulating)
			{
				tarMotorSpeed = 1f;
			}
			else
			{
				tarMotorSpeed = 0.3f;
			}
			if (m_capacitorCharge < 1f)
			{
				m_capacitorCharge += Time.deltaTime;
			}
			curMotorSpeed = Mathf.MoveTowards(curMotorSpeed, tarMotorSpeed, Time.deltaTime * 3f);
			for (int i = 0; i < SpinnyParts.Length; i++)
			{
				float num = curMotorSpeed * 2000f;
				m_rots[i] += num * Time.deltaTime;
				m_rots[i] = Mathf.Repeat(m_rots[i], 360f);
				if (i == 1)
				{
					SpinnyParts[i].localEulerAngles = new Vector3(0f, 0f, 0f - m_rots[i]);
				}
				else
				{
					SpinnyParts[i].localEulerAngles = new Vector3(0f, 0f, m_rots[i]);
				}
			}
			if (curMotorSpeed < 0.05f)
			{
				if (AudioMotor.isPlaying)
				{
					AudioMotor.Stop();
				}
			}
			else
			{
				if (!AudioMotor.isPlaying)
				{
					AudioMotor.Play();
				}
				AudioMotor.volume = (curMotorSpeed * 0.3f - 0.05f) * 0.4f;
				AudioMotor.pitch = curMotorSpeed * 0.3f + 0.3f;
			}
			if (curMotorSpeed < 0.3f)
			{
				if (AudioDrone.isPlaying)
				{
					AudioDrone.Stop();
				}
			}
			else
			{
				if (!AudioDrone.isPlaying)
				{
					AudioDrone.Play();
				}
				if (m_hasObject)
				{
					AudioDrone.volume = (curMotorSpeed * 0.4f - 0.05f) * 1f;
					AudioDrone.pitch = curMotorSpeed * 0.5f + 0.8f;
				}
				else
				{
					AudioDrone.volume = (curMotorSpeed * 0.4f - 0.05f) * 0.7f;
					AudioDrone.pitch = curMotorSpeed * 0.3f + 0.7f;
				}
			}
			if (curMotorSpeed > 0.4f)
			{
				ParticleSystem.EmissionModule emission = RotorParticles.emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.constantMax = (curMotorSpeed - 0.5f) * 20f;
				emission.rate = rate;
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = RotorParticles.emission;
				ParticleSystem.MinMaxCurve rate2 = emission2.rate;
				rate2.constantMax = 0f;
				emission2.rate = rate2;
			}
			if (m_isManipulating)
			{
				if (m_obj == null)
				{
					m_hasObject = false;
				}
				if (!m_hasObject)
				{
					ObjectSearch();
					Locus.gameObject.SetActive(value: false);
				}
				if (m_hasObject)
				{
					Locus.gameObject.SetActive(value: true);
					Locus.position = m_obj.RootRigidbody.worldCenterOfMass;
					Vector3 vector = Aperture.position + Aperture.forward * 0.75f;
					Quaternion rotation = Aperture.rotation;
					Vector3 worldCenterOfMass = m_obj.RootRigidbody.worldCenterOfMass;
					Quaternion quaternion = m_obj.RootRigidbody.rotation;
					m_isObjSpinner = false;
					if (m_obj.MP.CanNewStab)
					{
						quaternion = Quaternion.LookRotation(m_obj.MP.StabDirection.forward, m_obj.transform.right);
					}
					else if (m_obj.MP.HighDamageVectors.Count > 0)
					{
						m_isObjSpinner = true;
						quaternion = Quaternion.LookRotation(m_obj.MP.HighDamageVectors[0].forward, m_obj.transform.right);
					}
					Vector3 vector2 = worldCenterOfMass;
					Quaternion rotation2 = quaternion;
					Vector3 vector3 = vector;
					Quaternion quaternion2 = rotation;
					Vector3 vector4 = vector3 - vector2;
					Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(rotation2);
					float deltaTime = Time.deltaTime;
					quaternion3.ToAngleAxis(out var angle, out var axis);
					if (angle > 180f)
					{
						angle -= 360f;
					}
					if (angle != 0f)
					{
						Vector3 target = deltaTime * angle * axis * 6f;
						m_obj.RootRigidbody.angularVelocity = Vector3.MoveTowards(m_obj.RootRigidbody.angularVelocity, target, 50f * Time.fixedDeltaTime);
					}
					Vector3 target2 = vector4 * 450f * deltaTime;
					m_obj.RootRigidbody.velocity = Vector3.MoveTowards(m_obj.RootRigidbody.velocity, target2, 50f * deltaTime);
				}
			}
			else
			{
				Locus.gameObject.SetActive(value: false);
			}
			UpdateBolts();
			if (m_obj != null && !m_obj.IsDistantGrabbable())
			{
				m_obj = null;
				m_hasObject = false;
			}
		}

		private void UpdateBolts()
		{
			if (m_isManipulating)
			{
				if (m_hasObject)
				{
					for (int i = 0; i < Flaps.Count; i++)
					{
						Flaps[i].localEulerAngles = new Vector3(0f, -70f + Random.Range(0f, 3f), 0f);
					}
				}
				else
				{
					for (int j = 0; j < Flaps.Count; j++)
					{
						Flaps[j].localEulerAngles = new Vector3(0f, -45f + Random.Range(0f, 10f), 0f);
					}
				}
			}
			else
			{
				for (int k = 0; k < Flaps.Count; k++)
				{
					Flaps[k].localEulerAngles = new Vector3(0f, 17.7f, 0f);
				}
			}
		}

		private void ObjectSearch()
		{
			if (m_timeSinceLastSearch > 0f)
			{
				m_timeSinceLastSearch -= Time.deltaTime;
				return;
			}
			m_timeSinceLastSearch = Random.Range(0.1f, 0.2f);
			pos.Clear();
			Collider[] array = Physics.OverlapCapsule(Aperture.position + Aperture.forward * 0.5f, Aperture.position + Aperture.forward * 4.5f, 0.5f, LM_ObjectHunt, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].attachedRigidbody == null))
				{
					FVRPhysicalObject component = array[i].attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
					if (component != null && component.IsDistantGrabbable() && component != this)
					{
						pos.Add(component);
					}
				}
			}
			float num = 45f;
			float num2 = 10f;
			FVRPhysicalObject fVRPhysicalObject = null;
			foreach (FVRPhysicalObject po in pos)
			{
				Vector3 from = po.transform.position - Aperture.position;
				float magnitude = from.magnitude;
				float num3 = Vector3.Angle(from, Aperture.forward);
				if (num3 < num && magnitude < num2)
				{
					num = num3;
					num2 = magnitude;
					fVRPhysicalObject = po;
				}
			}
			if (fVRPhysicalObject != null)
			{
				m_hasObject = true;
				m_obj = fVRPhysicalObject;
			}
		}

		private void ShuntHeldObject()
		{
			if (m_hasObject)
			{
				if (m_obj.IsHeld)
				{
					m_obj.ForceBreakInteraction();
				}
				m_obj.RootRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				m_obj.RootRigidbody.velocity = Aperture.forward * 40f;
				if (m_isObjSpinner)
				{
				}
				m_obj = null;
				m_hasObject = false;
				PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.SuppressedLarge, FVRTailSoundClass.SuppressedLarge, SM.GetSoundEnvironment(base.transform.position));
				Recoil(twoHandStabilized: true, foregripStabilized: true, shoulderStabilized: false);
				WindParticles.Emit(20);
				RotorParticles.Emit(20);
				m_capacitorCharge = 0f;
				PlayAudioAsHandling(AudEvent_Capacitor, base.transform.position);
			}
		}

		private void WideShunt()
		{
			pos.Clear();
			Collider[] array = Physics.OverlapCapsule(Aperture.position + Aperture.forward * 0.5f, Aperture.position + Aperture.forward * 4.5f, 0.5f, LM_ObjectHunt, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].attachedRigidbody == null))
				{
					FVRPhysicalObject component = array[i].attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
					if (component != null && component.IsDistantGrabbable() && component != this)
					{
						pos.Add(component);
					}
				}
			}
			foreach (FVRPhysicalObject po in pos)
			{
				Vector3 from = po.transform.position - Aperture.position;
				float magnitude = from.magnitude;
				float num = Vector3.Angle(from, Aperture.forward);
				float num2 = Mathf.Lerp(1f, 0.1f, magnitude / 5f);
				float num3 = Mathf.Lerp(1f, 0.4f, num / 75f);
				po.RootRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				po.RootRigidbody.velocity = Aperture.forward * 40f * num2 * num3;
			}
			m_obj = null;
			m_hasObject = false;
			PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.SuppressedLarge, FVRTailSoundClass.SuppressedLarge, SM.GetSoundEnvironment(base.transform.position));
			Recoil(twoHandStabilized: true, foregripStabilized: true, shoulderStabilized: false);
			WindParticles.Emit(20);
			RotorParticles.Emit(20);
			m_capacitorCharge = 0f;
			PlayAudioAsHandling(AudEvent_Capacitor, base.transform.position);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!m_hasTriggeredUpSinceBegin || IsAltHeld)
			{
				return;
			}
			if (Lever.Mode == GBeamerModeLever.LeverMode.Rearward)
			{
				if (hand.Input.TriggerPressed && m_capacitorCharge >= 1f)
				{
					m_isManipulating = true;
				}
			}
			else if (Lever.Mode == GBeamerModeLever.LeverMode.Forward && hand.Input.TriggerDown && m_capacitorCharge >= 1f)
			{
				Object.Instantiate(ShuntSplosion, ShuntSplosionPos.position, ShuntSplosionPos.rotation);
				Invoke("WideShunt", 0.08f);
			}
			if (hand.Input.TriggerUp)
			{
				ShuntHeldObject();
				m_isManipulating = false;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_isManipulating = false;
		}

		public void SetSwitchState(int index, bool b)
		{
			switch (index)
			{
			case 0:
				m_isBatterySwitchedOn = b;
				break;
			case 1:
				m_isCapacitorSwitchedOn = b;
				if (b)
				{
					PlayAudioAsHandling(AudEvent_Capacitor, base.transform.position);
				}
				break;
			case 2:
				m_isMotorSwitchedOn = b;
				break;
			}
			UpdateStatusLights();
		}

		private void UpdateStatusLights()
		{
			if (m_isBatterySwitchedOn)
			{
				m_batteryLight = BeamerStatusLight.Green;
			}
			else
			{
				m_batteryLight = BeamerStatusLight.Red;
			}
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn && m_capacitorCharge >= 1f)
			{
				m_capacitorLight = BeamerStatusLight.Green;
			}
			else if (m_isCapacitorSwitchedOn)
			{
				m_capacitorLight = BeamerStatusLight.Yellow;
			}
			else
			{
				m_capacitorLight = BeamerStatusLight.Red;
			}
			if (m_isBatterySwitchedOn && m_isCapacitorSwitchedOn && m_isMotorSwitchedOn)
			{
				m_motorLight = BeamerStatusLight.Green;
			}
			else if (m_isMotorSwitchedOn)
			{
				m_motorLight = BeamerStatusLight.Yellow;
			}
			else
			{
				m_motorLight = BeamerStatusLight.Red;
			}
			UpdateLightDisplay(Light0, m_batteryLight);
			UpdateLightDisplay(Light1, m_capacitorLight);
			UpdateLightDisplay(Light2, m_motorLight);
		}

		private void UpdateLightDisplay(Renderer rend, BeamerStatusLight lightStatus)
		{
			switch (lightStatus)
			{
			case BeamerStatusLight.Green:
				rend.material = MatLightGreen;
				break;
			case BeamerStatusLight.Yellow:
				rend.material = MatLightYellow;
				break;
			case BeamerStatusLight.Red:
				rend.material = MatLightRed;
				break;
			}
		}
	}
}
