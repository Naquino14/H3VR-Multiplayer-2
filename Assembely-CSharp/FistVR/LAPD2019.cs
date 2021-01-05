using UnityEngine;

namespace FistVR
{
	public class LAPD2019 : FVRFireArm
	{
		[Header("LAPD2019 Components Config")]
		public Transform Hammer;

		private float m_hammerForwardRot;

		private float m_hammerBackwardRot = -49f;

		private float m_hammerCurrentRot;

		public Transform Trigger1;

		public Transform Trigger2;

		private float m_triggerForwardRot;

		private float m_triggerBackwardRot = 30f;

		private float m_triggerCurrentRot1;

		private float m_triggerCurrentRot2;

		private bool m_hasTriggerCycled;

		public Transform CylinderReleaseButton;

		public Transform CylinderReleaseButtonForwardPos;

		public Transform CylinderReleaseButtonRearPos;

		private bool m_isCylinderReleasePressed;

		private float m_cylinderReleaseButtonLerp;

		public LAPD2019BoltHandle BoltHandle;

		[Header("LAPD2019 TransformPoints")]
		public Transform BoltForward;

		public Transform BoltRearward;

		[Header("Trigger Config")]
		public float TriggerFireThreshold = 0.75f;

		public float TriggerResetThreshold = 0.25f;

		private float m_triggerFloat;

		private float m_mechanicalCycleLerp;

		[Header("Cylinder Config")]
		public Transform CylinderArm;

		private bool m_isCylinderArmLocked = true;

		private bool m_wasCylinderArmLocked = true;

		private float CylinderArmRot;

		public Vector2 CylinderRotRange = new Vector2(0f, 105f);

		public bool GravityRotsCylinderPositive = true;

		public LAPD2019Cylinder Cylinder;

		private int m_curChamber;

		private float m_tarChamberLerp;

		private float m_curChamberLerp;

		[Header("Chambers Config")]
		public FVRFireArmChamber[] Chambers;

		[Header("Spinning Config")]
		public Transform PoseSpinHolder;

		public bool CanSpin = true;

		private bool m_isSpinning;

		[Header("Audio Config")]
		public AudioEvent ThermalClip_In;

		public AudioEvent ThermalClip_Eject;

		public AudioEvent CapacitorCharge;

		public AudioEvent Chirp_FullyCharged;

		public AudioEvent Chirp_LowBattery;

		public AudioEvent Chirp_FreshBattery;

		public AudioEvent Chirp_DeadBattery;

		[Header("Pose Config")]
		public bool UsesAltPoseSwitch = true;

		public Transform Pose_Main;

		public Transform Pose_Reloading;

		private bool m_isInMainpose = true;

		[Header("Electrical System Config")]
		public GameObject FauxBattery;

		public Renderer LED_Rear;

		public Renderer LED_FauxBattery_Side;

		public Renderer LED_FauxBattery_Under;

		public LAPD2019Laser LaserSystem;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Emissive_Red;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Emissive_Green;

		private bool m_isAutoChargeEnabled;

		private float m_capacitorCharge;

		private bool m_isCapacitorCharging;

		private bool m_isCapacitorCharged;

		public LAPD2019BatteryTriggerWell BatteryWellTrigger;

		private bool m_hasBattery;

		private float m_batteryCharge;

		[Header("Thermal Clip Config")]
		public GameObject ThermalClipProxy;

		public GameObject ThermalClipLoadTrigger;

		public ParticleSystem PSystem_ThermalSteam;

		public ParticleSystem PSystem_ThermalSteam2;

		public ParticleSystem PSystem_SparksShot1;

		public ParticleSystem PSystem_SparksShot2;

		public Transform ThermalClipEjectPos;

		private ParticleSystem.EmissionModule m_pSystem_ThermalSteam_Emission;

		private ParticleSystem.EmissionModule m_pSystem_ThermalSteam_Emission2;

		private bool m_hasThermalClip = true;

		public Renderer[] GlowingParts;

		public Renderer ProxyClipRenderer;

		private float m_thermalClipLoadTriggerEnableTick;

		private float m_heatThermalClip;

		private float m_heatSystem;

		private float m_barrelHeatDamage;

		public FVRObject BatteryPrefab;

		public FVRObject ThermalClipPrefab;

		public GameObject[] ProxMalfunctionPrefab;

		private float xSpinRot;

		private float xSpinVel;

		private float m_CylCloseVel;

		public bool isCylinderArmLocked => m_isCylinderArmLocked;

		public int CurChamber
		{
			get
			{
				return m_curChamber;
			}
			set
			{
				m_curChamber = value % Cylinder.numChambers;
			}
		}

		public bool HasBattery => m_hasBattery;

		protected override void Awake()
		{
			base.Awake();
			FauxBattery.SetActive(value: false);
			if (PoseOverride_Touch != null)
			{
				Pose_Main.localPosition = PoseOverride_Touch.localPosition;
				Pose_Main.localRotation = PoseOverride_Touch.localRotation;
			}
			m_pSystem_ThermalSteam_Emission = PSystem_ThermalSteam.emission;
			m_pSystem_ThermalSteam_Emission.rateOverTimeMultiplier = 0f;
			m_pSystem_ThermalSteam_Emission2 = PSystem_ThermalSteam2.emission;
			m_pSystem_ThermalSteam_Emission2.rateOverTimeMultiplier = 0f;
			ThermalClipLoadTrigger.SetActive(value: false);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateElectricalSystem();
			UpdateThermalSystem();
		}

		private void UpdateThermalSystem()
		{
			float num = 0f;
			if (m_heatSystem > 0f)
			{
				num = Mathf.Min(m_heatSystem, Time.deltaTime * 0.2f);
			}
			if (m_hasThermalClip && m_heatThermalClip < 1f)
			{
				m_heatThermalClip += num * 0.4f;
				m_heatSystem -= num;
			}
			if (m_heatSystem > 0f)
			{
				m_heatSystem -= Time.deltaTime * 0.005f;
			}
			m_heatSystem = Mathf.Clamp(m_heatSystem, 0f, 2f);
			for (int i = 0; i < GlowingParts.Length; i++)
			{
				GlowingParts[i].material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(m_heatSystem, 2.5f), 0f, 1f));
			}
			if (m_hasThermalClip)
			{
				ProxyClipRenderer.material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(m_heatThermalClip, 1.5f), 0f, 1f));
			}
			bool flag = false;
			if (BoltHandle.HandleRot != LAPD2019BoltHandle.BoltActionHandleRot.Down)
			{
				flag = true;
			}
			float num2 = 0f;
			num2 = (flag ? ((!m_hasThermalClip) ? m_heatSystem : Mathf.Max(m_heatThermalClip, m_heatSystem)) : ((!m_hasThermalClip) ? Mathf.Clamp((m_heatSystem - 0.5f) * 2f, 0f, 1f) : Mathf.Clamp((Mathf.Max(m_heatThermalClip, m_heatSystem) - 0.5f) * 2f, 0f, 1f)));
			m_pSystem_ThermalSteam_Emission.rateOverTimeMultiplier = num2 * 25f;
			m_pSystem_ThermalSteam_Emission2.rateOverTimeMultiplier = Mathf.Clamp(Mathf.Pow((m_heatSystem - 0.5f) * 2f, 2.5f), 0f, 1f) * 35f;
			if (!m_hasThermalClip && m_thermalClipLoadTriggerEnableTick > 0f)
			{
				m_thermalClipLoadTriggerEnableTick -= Time.deltaTime;
			}
			bool flag2 = false;
			if (!m_hasThermalClip && m_thermalClipLoadTriggerEnableTick < 0f && BoltHandle.HandleRot == LAPD2019BoltHandle.BoltActionHandleRot.Up)
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (!ThermalClipLoadTrigger.activeSelf)
				{
					ThermalClipLoadTrigger.SetActive(value: true);
				}
			}
			else if (ThermalClipLoadTrigger.activeSelf)
			{
				ThermalClipLoadTrigger.SetActive(value: false);
			}
		}

		public bool LoadThermalClip(float heat)
		{
			if (!m_hasThermalClip)
			{
				m_hasThermalClip = true;
				ThermalClipProxy.SetActive(value: true);
				m_heatThermalClip = heat;
				m_pool_handling.PlayClip(ThermalClip_In, base.transform.position);
				return true;
			}
			return false;
		}

		public void EjectThermalClip()
		{
			if (m_hasThermalClip)
			{
				m_hasThermalClip = false;
				ThermalClipProxy.SetActive(value: false);
				GameObject gameObject = Object.Instantiate(ThermalClipPrefab.GetGameObject(), ThermalClipEjectPos.position, ThermalClipEjectPos.rotation);
				gameObject.GetComponent<Rigidbody>().velocity = ThermalClipEjectPos.up * 2.5f;
				gameObject.GetComponent<LAPD2019ThermalClip>().SetHeat(m_heatThermalClip);
				m_pool_handling.PlayClip(ThermalClip_Eject, base.transform.position);
				m_heatThermalClip = 0f;
				m_thermalClipLoadTriggerEnableTick = 0.5f;
			}
		}

		private void ChargeCapacitor()
		{
			if (m_hasBattery && !(m_batteryCharge <= 0f) && !m_isCapacitorCharging && !m_isCapacitorCharged)
			{
				m_isCapacitorCharging = true;
				m_pool_handling.PlayClip(CapacitorCharge, base.transform.position);
			}
		}

		private void UpdateElectricalSystem()
		{
			if (!m_hasBattery)
			{
				m_batteryCharge = 0f;
				m_isCapacitorCharging = false;
			}
			if (m_hasBattery)
			{
				LED_FauxBattery_Side.material.SetColor("_Color", Color.Lerp(Color_Emissive_Red, Color_Emissive_Green, m_batteryCharge));
			}
			LED_Rear.material.SetColor("_Color", Color.Lerp(Color_Emissive_Red, Color_Emissive_Green, m_capacitorCharge));
			if (!m_isCapacitorCharging || !m_hasBattery)
			{
				return;
			}
			if (m_batteryCharge > 0f)
			{
				if (m_capacitorCharge < 1f)
				{
					float batteryCharge = m_batteryCharge;
					float num = 1f + m_heatSystem;
					m_batteryCharge -= Time.deltaTime * 0.05f * num;
					m_capacitorCharge += Time.deltaTime * 0.75f;
					if (batteryCharge > 0.2f && m_batteryCharge <= 0.2f)
					{
						m_pool_handling.PlayClip(Chirp_LowBattery, base.transform.position);
					}
				}
				else
				{
					m_capacitorCharge = 1f;
					m_isCapacitorCharging = false;
					m_isCapacitorCharged = true;
					m_pool_handling.PlayClip(Chirp_FullyCharged, base.transform.position);
				}
			}
			else
			{
				m_isCapacitorCharging = false;
				m_batteryCharge = 0f;
				m_pool_handling.PlayClip(Chirp_DeadBattery, base.transform.position);
			}
		}

		public bool LoadBattery(LAPD2019Battery battery)
		{
			if (m_hasBattery)
			{
				return false;
			}
			FauxBattery.SetActive(value: true);
			BatteryWellTrigger.gameObject.SetActive(value: false);
			m_hasBattery = true;
			m_batteryCharge = battery.GetEnergy();
			PlayAudioEvent(FirearmAudioEventType.MagazineIn);
			if (m_batteryCharge > 0.2f)
			{
				m_pool_handling.PlayClip(Chirp_FreshBattery, base.transform.position);
			}
			else
			{
				m_pool_handling.PlayClip(Chirp_LowBattery, base.transform.position);
			}
			return true;
		}

		public float ExtractBattery(FVRViveHand hand)
		{
			if (m_hasBattery)
			{
				FauxBattery.SetActive(value: false);
				m_hasBattery = false;
				float batteryCharge = m_batteryCharge;
				m_batteryCharge = 0f;
				PlayAudioEvent(FirearmAudioEventType.MagazineOut);
				Invoke("EnableWellTrigger", 1f);
				return batteryCharge;
			}
			return -1f;
		}

		private void EnableWellTrigger()
		{
			BatteryWellTrigger.gameObject.SetActive(value: true);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (!IsAltHeld)
			{
				if (m_isInMainpose)
				{
					PoseOverride.localPosition = Pose_Main.localPosition;
					PoseOverride.localRotation = Pose_Main.localRotation;
					m_grabPointTransform.localPosition = Pose_Main.localPosition;
					m_grabPointTransform.localRotation = Pose_Main.localRotation;
				}
				else
				{
					PoseOverride.localPosition = Pose_Reloading.localPosition;
					PoseOverride.localRotation = Pose_Reloading.localRotation;
					m_grabPointTransform.localPosition = Pose_Reloading.localPosition;
					m_grabPointTransform.localRotation = Pose_Reloading.localRotation;
				}
			}
			LaserSystem.TurnOn();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (IsAltHeld && !m_isInMainpose)
			{
				m_isInMainpose = true;
				PoseOverride.localPosition = Pose_Main.localPosition;
				PoseOverride.localRotation = Pose_Main.localRotation;
				m_grabPointTransform.localPosition = Pose_Main.localPosition;
				m_grabPointTransform.localRotation = Pose_Main.localRotation;
			}
			m_isSpinning = false;
			if (m_hand.IsInStreamlinedMode)
			{
				if (hand.Input.AXButtonDown)
				{
					ToggleChargeMode();
				}
			}
			else
			{
				if (hand.Input.TouchpadPressed && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45f)
				{
					m_isSpinning = true;
				}
				if (hand.Input.TouchpadDown)
				{
					if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45f && UsesAltPoseSwitch)
					{
						m_isInMainpose = !m_isInMainpose;
						if (m_isInMainpose)
						{
							PoseOverride.localPosition = Pose_Main.localPosition;
							PoseOverride.localRotation = Pose_Main.localRotation;
							m_grabPointTransform.localPosition = Pose_Main.localPosition;
							m_grabPointTransform.localRotation = Pose_Main.localRotation;
						}
						else
						{
							PoseOverride.localPosition = Pose_Reloading.localPosition;
							PoseOverride.localRotation = Pose_Reloading.localRotation;
							m_grabPointTransform.localPosition = Pose_Reloading.localPosition;
							m_grabPointTransform.localRotation = Pose_Reloading.localRotation;
						}
					}
					else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45f)
					{
						ToggleChargeMode();
					}
				}
			}
			UpdateTriggerHammer();
			UpdateCylinderRelease();
			if (!base.IsHeld || IsAltHeld || base.AltGrip != null)
			{
				m_isSpinning = false;
			}
		}

		private void ToggleChargeMode()
		{
			m_isAutoChargeEnabled = !m_isAutoChargeEnabled;
			if (m_isAutoChargeEnabled && m_batteryCharge <= 0f)
			{
				m_pool_handling.PlayClip(Chirp_DeadBattery, base.transform.position);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_triggerFloat = 0f;
			base.EndInteraction(hand);
			LaserSystem.TurnOff();
			base.RootRigidbody.AddRelativeTorque(new Vector3(xSpinVel, 0f, 0f), ForceMode.Impulse);
		}

		protected override void FVRFixedUpdate()
		{
			UpdateSpinning();
			if (m_isAutoChargeEnabled && m_hasBattery)
			{
				ChargeCapacitor();
			}
			base.FVRFixedUpdate();
		}

		public void EjectChambers()
		{
			bool flag = false;
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (Chambers[i].IsFull)
				{
					flag = true;
					Chambers[i].EjectRound(Chambers[i].transform.position + -Chambers[i].transform.forward * Chambers[i].GetRound().PalmingDimensions.z * 0.5f, -Chambers[i].transform.forward, Vector3.zero);
				}
			}
			if (flag)
			{
				PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
			}
		}

		private void UpdateSpinning()
		{
			if (!base.IsHeld || IsAltHeld || base.AltGrip != null)
			{
				m_isSpinning = false;
			}
			if (m_isSpinning)
			{
				Vector3 vector = Vector3.zero;
				if (m_hand != null)
				{
					vector = m_hand.Input.VelLinearLocal;
				}
				float value = Vector3.Dot(vector.normalized, base.transform.up);
				value = Mathf.Clamp(value, 0f - vector.magnitude, vector.magnitude);
				if (Mathf.Abs(xSpinVel) < 90f)
				{
					xSpinVel += value * Time.deltaTime * 600f;
				}
				else if (Mathf.Sign(value) == Mathf.Sign(xSpinVel))
				{
					xSpinVel += value * Time.deltaTime * 600f;
				}
				if (Mathf.Abs(xSpinVel) < 90f)
				{
					if (Vector3.Dot(base.transform.up, Vector3.down) >= 0f && Mathf.Sign(xSpinVel) == 1f)
					{
						xSpinVel += Time.deltaTime * 50f;
					}
					if (Vector3.Dot(base.transform.up, Vector3.down) < 0f && Mathf.Sign(xSpinVel) == -1f)
					{
						xSpinVel -= Time.deltaTime * 50f;
					}
				}
				xSpinVel = Mathf.Clamp(xSpinVel, -500f, 500f);
				xSpinRot += xSpinVel * Time.deltaTime * 5f;
				PoseSpinHolder.localEulerAngles = new Vector3(xSpinRot, 0f, 0f);
				xSpinVel = Mathf.Lerp(xSpinVel, 0f, Time.deltaTime * 0.6f);
			}
			else
			{
				xSpinRot = 0f;
				xSpinVel = 0f;
				PoseSpinHolder.localRotation = Quaternion.RotateTowards(PoseSpinHolder.localRotation, Quaternion.identity, Time.deltaTime * 500f);
				PoseSpinHolder.localEulerAngles = new Vector3(PoseSpinHolder.localEulerAngles.x, 0f, 0f);
			}
		}

		private void UpdateTriggerHammer()
		{
			m_triggerFloat = 0f;
			if (m_hasTriggeredUpSinceBegin && !m_isSpinning && !IsAltHeld)
			{
				m_triggerFloat = m_hand.Input.TriggerFloat;
			}
			m_mechanicalCycleLerp = Mathf.Clamp(m_triggerFloat / TriggerFireThreshold, 0f, 1f);
			if (m_isCapacitorCharged)
			{
				m_triggerCurrentRot1 = 0f;
				m_triggerCurrentRot2 = Mathf.Lerp(m_triggerForwardRot, m_triggerBackwardRot, m_triggerFloat / TriggerFireThreshold);
			}
			else
			{
				m_triggerCurrentRot1 = Mathf.Lerp(m_triggerForwardRot, m_triggerBackwardRot, m_triggerFloat / TriggerFireThreshold);
				m_triggerCurrentRot2 = 0f;
			}
			Trigger1.localEulerAngles = new Vector3(m_triggerCurrentRot1, 0f, 0f);
			Trigger2.localEulerAngles = new Vector3(m_triggerCurrentRot2, 0f, 0f);
			if (!m_hasTriggerCycled)
			{
				if (m_mechanicalCycleLerp >= 1f)
				{
					if (m_isCylinderArmLocked)
					{
						m_hasTriggerCycled = true;
						CurChamber++;
						m_curChamberLerp = 0f;
						m_tarChamberLerp = 0f;
						PlayAudioEvent(FirearmAudioEventType.Prefire);
						PlayAudioEvent(FirearmAudioEventType.HammerHit);
						if (Chambers[CurChamber].IsFull && !Chambers[CurChamber].IsSpent)
						{
							Chambers[CurChamber].Fire();
							Fire();
							if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
							{
								Chambers[CurChamber].IsSpent = false;
								Chambers[CurChamber].UpdateProxyDisplay();
							}
						}
					}
					else
					{
						m_hasTriggerCycled = true;
					}
				}
			}
			else if (m_hasTriggerCycled && m_triggerFloat <= TriggerResetThreshold)
			{
				m_hasTriggerCycled = false;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
			if (m_hasTriggerCycled)
			{
				m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerForwardRot, Time.deltaTime * 30f);
			}
			else
			{
				m_hammerCurrentRot = Mathf.Lerp(m_hammerForwardRot, m_hammerBackwardRot, m_mechanicalCycleLerp);
			}
			Hammer.localEulerAngles = new Vector3(m_hammerCurrentRot, 0f, 0f);
		}

		private void Fire()
		{
			FVRFireArmChamber fVRFireArmChamber = Chambers[CurChamber];
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = IsForegripStabilized();
			bool shoulderStabilized = IsShoulderStabilized();
			bool flag = false;
			FVRFireArmRound round = fVRFireArmChamber.GetRound();
			if (m_isCapacitorCharged)
			{
				flag = true;
				m_isCapacitorCharged = false;
				m_capacitorCharge = 0f;
				float num = Mathf.Clamp(4.2f * (1f - m_barrelHeatDamage * 0.8f), 1f, 4.2f);
				for (int i = 0; i < round.NumProjectiles; i++)
				{
					GameObject gameObject = null;
					float num2 = round.ProjectileSpread + m_barrelHeatDamage * 1f;
					if (!(round.ProjectilePrefab != null))
					{
						continue;
					}
					gameObject = Object.Instantiate(fVRFireArmChamber.GetRound().ProjectilePrefab, GetMuzzle().position, GetMuzzle().rotation);
					gameObject.transform.Rotate(new Vector3(Random.Range(0f - num2, num2), Random.Range(0f - num2, num2), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					if (round.RoundClass == FireArmRoundClass.DSM_Mine)
					{
						float num3 = Random.Range(0f, 2f);
						float heatSystem = m_heatSystem;
						if (num3 <= m_heatSystem)
						{
							Object.Destroy(gameObject);
							int num4 = 0;
							for (; i < ProxMalfunctionPrefab.Length; i++)
							{
								Object.Instantiate(ProxMalfunctionPrefab[num4], MuzzlePos.position + MuzzlePos.forward, Quaternion.identity);
							}
						}
					}
					component.Fire(component.MuzzleVelocityBase * num, gameObject.transform.forward, this);
				}
				m_barrelHeatDamage += m_heatSystem * 0.1f;
				m_barrelHeatDamage = Mathf.Clamp(m_barrelHeatDamage, 0f, 1f);
				m_heatSystem += 0.2f;
				PSystem_SparksShot2.Emit(50);
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			}
			else
			{
				for (int j = 0; j < round.NumProjectiles; j++)
				{
					GameObject gameObject2 = null;
					float num5 = round.ProjectileSpread + m_barrelHeatDamage * 1f;
					if (round.ProjectilePrefab != null)
					{
						gameObject2 = Object.Instantiate(round.ProjectilePrefab, GetMuzzle().position, GetMuzzle().rotation);
						gameObject2.transform.Rotate(new Vector3(Random.Range(0f - num5, num5), Random.Range(0f - num5, num5), 0f));
						BallisticProjectile component2 = gameObject2.GetComponent<BallisticProjectile>();
						component2.Fire(gameObject2.transform.forward, this);
					}
				}
				m_barrelHeatDamage += m_heatSystem * 0.04f;
				m_barrelHeatDamage = Mathf.Clamp(m_barrelHeatDamage, 0f, 1f);
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			}
			int num6 = (int)Mathf.Lerp(0f, 60f, Mathf.Pow(m_heatSystem, 2.5f));
			if (num6 > 0)
			{
				PSystem_SparksShot1.Emit(num6);
			}
			m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
			Vector3 position = base.transform.position;
			if (flag)
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Main);
			}
			m_pool_tail.PlayClip(SM.GetTailSet(round.TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), position);
			FireMuzzleSmoke();
		}

		public void AddCylinderCloseVel(float f)
		{
			m_CylCloseVel = f;
		}

		private void UpdateCylinderRelease()
		{
			m_isCylinderReleasePressed = false;
			if (m_hand.IsInStreamlinedMode)
			{
				if (m_hand.Input.BYButtonPressed)
				{
					m_isCylinderReleasePressed = true;
				}
			}
			else if (m_hand.Input.TouchpadPressed && Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.left) < 45f)
			{
				m_isCylinderReleasePressed = true;
			}
			if (m_isCylinderReleasePressed)
			{
				if (m_cylinderReleaseButtonLerp < 1f)
				{
					m_cylinderReleaseButtonLerp += Time.deltaTime * 5f;
				}
				else
				{
					m_cylinderReleaseButtonLerp = 1f;
				}
			}
			else if (m_cylinderReleaseButtonLerp > 0f)
			{
				m_cylinderReleaseButtonLerp -= Time.deltaTime * 5f;
			}
			else
			{
				m_cylinderReleaseButtonLerp = 0f;
			}
			CylinderReleaseButton.localPosition = Vector3.Lerp(CylinderReleaseButtonRearPos.localPosition, CylinderReleaseButtonForwardPos.localPosition, m_cylinderReleaseButtonLerp);
			if (m_isCylinderReleasePressed)
			{
				m_isCylinderArmLocked = false;
			}
			else if (Mathf.Abs(CylinderArm.localEulerAngles.z) <= 1f && !m_isCylinderArmLocked)
			{
				m_isCylinderArmLocked = true;
				CylinderArm.localEulerAngles = Vector3.zero;
			}
			float num = 160f;
			if (!GravityRotsCylinderPositive)
			{
				num *= -1f;
			}
			float num2 = 0f;
			if (!m_isCylinderArmLocked)
			{
				float z = base.transform.InverseTransformDirection(m_hand.Input.VelAngularWorld).z;
				float x = base.transform.InverseTransformDirection(m_hand.Input.VelLinearWorld).x;
				num += z * 70f;
				num += x * -250f;
				num += m_CylCloseVel;
				m_CylCloseVel = 0f;
				num2 = CylinderArmRot + num * Time.deltaTime;
				num2 = Mathf.Clamp(num2, CylinderRotRange.x, CylinderRotRange.y);
				if (num2 != CylinderArmRot)
				{
					CylinderArmRot = num2;
					CylinderArm.localEulerAngles = new Vector3(0f, 0f, num2);
				}
			}
			if (Mathf.Abs(CylinderArm.localEulerAngles.z) > 30f)
			{
				for (int i = 0; i < Chambers.Length; i++)
				{
					Chambers[i].IsAccessible = true;
				}
			}
			else
			{
				for (int j = 0; j < Chambers.Length; j++)
				{
					Chambers[j].IsAccessible = false;
				}
			}
			if (Mathf.Abs(CylinderArmRot - CylinderRotRange.y) < 5f)
			{
				float z2 = base.transform.InverseTransformDirection(m_hand.Input.VelLinearLocal).z;
				if (z2 < -2f)
				{
					EjectChambers();
				}
			}
			if (m_isCylinderArmLocked && !m_wasCylinderArmLocked)
			{
				m_curChamber = Cylinder.GetClosestChamberIndex();
				Cylinder.transform.localRotation = Cylinder.GetLocalRotationFromCylinder(m_curChamber);
				m_curChamberLerp = 0f;
				m_tarChamberLerp = 0f;
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
			}
			if (!m_isCylinderArmLocked && m_wasCylinderArmLocked)
			{
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			}
			if (!m_hasTriggerCycled)
			{
				m_tarChamberLerp = m_mechanicalCycleLerp;
			}
			m_curChamberLerp = Mathf.Lerp(m_curChamberLerp, m_tarChamberLerp, Time.deltaTime * 16f);
			int cylinder = (CurChamber + 1) % Cylinder.numChambers;
			if (isCylinderArmLocked)
			{
				Cylinder.transform.localRotation = Quaternion.Slerp(Cylinder.GetLocalRotationFromCylinder(CurChamber), Cylinder.GetLocalRotationFromCylinder(cylinder), m_curChamberLerp);
			}
			m_wasCylinderArmLocked = m_isCylinderArmLocked;
		}
	}
}
