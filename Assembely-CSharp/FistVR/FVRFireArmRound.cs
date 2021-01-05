using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmRound : FVRPhysicalObject, IFVRDamageable
	{
		[Serializable]
		public class ProxyRound
		{
			public GameObject GO;

			public MeshFilter Filter;

			public Renderer Renderer;

			public FireArmRoundClass Class;

			public FVRObject ObjectWrapper;
		}

		[Header("FireArm Round Config")]
		[SearchableEnum]
		public FireArmRoundType RoundType;

		[SearchableEnum]
		public FireArmRoundClass RoundClass;

		public bool IsHighPressure = true;

		public GameObject BallisticProjectilePrefab;

		public GameObject ProjectilePrefab;

		public int NumProjectiles = 1;

		public float ProjectileSpread;

		public Renderer UnfiredRenderer;

		public Renderer FiredRenderer;

		private bool m_isChambered;

		private bool m_isSpent;

		public bool IsDestroyedAfterCounter = true;

		private float m_killAfter = 10f;

		private bool m_isKillCounting;

		public bool isManuallyChamberable;

		[HideInInspector]
		public FVRFireArmChamber HoveredOverChamber;

		public bool isMagazineLoadable;

		private FVRFireArmMagazineReloadTrigger m_hoverOverReloadTrigger;

		private float m_phys_mass;

		private float m_phys_drag;

		private float m_phys_angularDrag;

		private float m_pickUpCooldown = 1f;

		public bool isPalmable = true;

		public Vector3 PalmingDimensions = new Vector3(0.01f, 0.01f, 0.03f);

		public int MaxPalmedAmount = 5;

		[HideInInspector]
		public FVRFireArmRound HoveredOverRound;

		public List<ProxyRound> ProxyRounds = new List<ProxyRound>();

		private bool m_proxyDumpFlag = true;

		public bool IsCaseless;

		private bool isCookingOff;

		private float TickTilCookOff = 1f;

		public PMaterial PopType = PMaterial.BulletPopNormal;

		[Header("Audio Configuration")]
		public FVRTailSoundClass TailClass;

		public FVRTailSoundClass TailClassSuppressed = FVRTailSoundClass.SuppressedSmall;

		public ImpactType ImpactSound_Unfired;

		public ImpactType ImpactSound_Fired;

		public bool IsChambered => m_isChambered;

		public bool IsSpent => m_isSpent;

		protected override void Awake()
		{
			base.Awake();
			if (UnfiredRenderer != null)
			{
				UnfiredRenderer.enabled = true;
			}
			if (FiredRenderer != null)
			{
				FiredRenderer.enabled = false;
			}
			m_phys_mass = base.RootRigidbody.mass;
			m_phys_drag = base.RootRigidbody.drag;
			m_phys_angularDrag = base.RootRigidbody.angularDrag;
			switch (GM.Options.SimulationOptions.ShellTime)
			{
			case SimulationOptions.SpentShellDespawnTime.Seconds_5:
				m_killAfter = 5f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Seconds_10:
				m_killAfter = 10f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Seconds_30:
				m_killAfter = 30f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Infinite:
				m_killAfter = 999999f;
				break;
			}
			if (GM.CurrentSceneSettings.ForcesCasingDespawn)
			{
				m_killAfter = 5f;
			}
			if (IsCaseless)
			{
				IsDestroyedAfterCounter = true;
				m_killAfter = 0.1f;
			}
			if (base.HasImpactController)
			{
				base.AudioImpactController.ImpactType = ImpactSound_Unfired;
			}
		}

		public override bool IsInteractable()
		{
			return !m_isChambered;
		}

		public void Fire()
		{
			if (UnfiredRenderer != null)
			{
				UnfiredRenderer.enabled = false;
			}
			if (FiredRenderer != null)
			{
				FiredRenderer.enabled = true;
			}
			m_isSpent = true;
			if (base.HasImpactController)
			{
				base.AudioImpactController.ImpactType = ImpactSound_Fired;
			}
		}

		public void Damage(Damage d)
		{
			if (!m_isSpent && !(base.QuickbeltSlot != null) && !m_isChambered)
			{
				if (d.Dam_TotalKinetic > 100f)
				{
					Splode(UnityEngine.Random.Range(0.3f, 0.8f), isRandomDir: false, DoesDestroyBrass: false);
				}
				else if (d.Dam_Thermal > 0f)
				{
					isCookingOff = true;
					TickTilCookOff = UnityEngine.Random.Range(0.5f, 1.5f);
				}
			}
		}

		private void Splode(float velMultiplier, bool isRandomDir, bool DoesDestroyBrass)
		{
			Fire();
			if (base.IsHeld)
			{
				EndInteraction(m_hand);
				ForceBreakInteraction();
			}
			isCookingOff = false;
			for (int i = 0; i < NumProjectiles; i++)
			{
				GameObject gameObject = null;
				float num = ProjectileSpread * 5f;
				if (isRandomDir)
				{
					num *= 5f;
				}
				if (BallisticProjectilePrefab != null)
				{
					gameObject = UnityEngine.Object.Instantiate(BallisticProjectilePrefab, base.transform.position + PalmingDimensions.z * base.transform.forward * 2f, base.transform.rotation);
					gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(0f - num, num), UnityEngine.Random.Range(0f - num, num), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.Fire(component.MuzzleVelocityBase * velMultiplier, base.transform.forward, null);
				}
			}
			AudioEvent audioEvent = new AudioEvent();
			audioEvent.Clips.Add(PM.GetRandomImpactClip(PopType));
			audioEvent.VolumeRange = new Vector2(0.3f, 0.4f);
			audioEvent.PitchRange = new Vector2(0.8f, 1.1f);
			float num2 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			float delay = num2 / 343f;
			SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, audioEvent, base.transform.position, delay);
			if (IsSpent && IsDestroyedAfterCounter)
			{
				m_isKillCounting = true;
			}
			base.RootRigidbody.velocity = -base.transform.forward * UnityEngine.Random.Range(2f, 15f);
			if (DoesDestroyBrass)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Recharge()
		{
			if (UnfiredRenderer != null)
			{
				UnfiredRenderer.enabled = true;
			}
			if (FiredRenderer != null)
			{
				FiredRenderer.enabled = false;
			}
			m_isSpent = false;
			if (base.HasImpactController)
			{
				base.AudioImpactController.ImpactType = ImpactSound_Unfired;
			}
		}

		public void Chamber(FVRFireArmChamber c, bool makeChamberingSound)
		{
			c.SetRound(this);
			if (makeChamberingSound)
			{
				c.PlayChamberingAudio();
			}
		}

		public void Eject(Vector3 EjectionPosition, Vector3 EjectionVelocity, Vector3 EjectionAngularVelocity)
		{
			GetComponent<Collider>().enabled = true;
			m_pickUpCooldown = 1f;
			if (base.RootRigidbody == null)
			{
				base.RootRigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			base.RootRigidbody.mass = m_phys_mass;
			base.RootRigidbody.drag = m_phys_drag;
			base.RootRigidbody.angularDrag = m_phys_angularDrag;
			base.RootRigidbody.isKinematic = false;
			base.RootRigidbody.useGravity = true;
			SetParentage(null);
			SetAllCollidersToLayer(triggersToo: false, "Default");
			HoveredOverChamber = null;
			m_isChambered = false;
			base.RootRigidbody.transform.position = EjectionPosition;
			base.RootRigidbody.velocity = Vector3.Lerp(EjectionVelocity * 0.7f, EjectionVelocity, UnityEngine.Random.value);
			base.RootRigidbody.maxAngularVelocity = 200f;
			base.RootRigidbody.angularVelocity = Vector3.Lerp(EjectionAngularVelocity * 0.3f, EjectionAngularVelocity, UnityEngine.Random.value);
			if (IsSpent && IsDestroyedAfterCounter)
			{
				m_isKillCounting = true;
			}
		}

		public void SetKillCounting(bool b)
		{
			m_isKillCounting = b;
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
			for (int i = 0; i < ProxyRounds.Count; i++)
			{
				component.AddProxy(ProxyRounds[i].Class, ProxyRounds[i].ObjectWrapper);
			}
			component.UpdateProxyDisplay();
			return gameObject;
		}

		public void AddProxy(FireArmRoundClass roundClass, FVRObject prefabWrapper)
		{
			ProxyRound proxyRound = new ProxyRound();
			GameObject gameObject = (proxyRound.GO = new GameObject("Proxy"));
			gameObject.transform.SetParent(base.transform);
			proxyRound.Filter = gameObject.AddComponent<MeshFilter>();
			proxyRound.Renderer = gameObject.AddComponent<MeshRenderer>();
			proxyRound.Class = roundClass;
			proxyRound.ObjectWrapper = prefabWrapper;
			ProxyRounds.Add(proxyRound);
		}

		public void PalmRound(FVRFireArmRound round, bool insertAtFront, bool updateDisplay, int addAtIndex = 0)
		{
			SM.PlayHandlingGrabSound(HandlingGrabSound, base.transform.position, isHard: false);
			ProxyRound proxyRound = new ProxyRound();
			GameObject gameObject = (proxyRound.GO = new GameObject("Proxy"));
			gameObject.transform.SetParent(base.transform);
			proxyRound.Filter = gameObject.AddComponent<MeshFilter>();
			proxyRound.Renderer = gameObject.AddComponent<MeshRenderer>();
			proxyRound.Class = round.RoundClass;
			proxyRound.ObjectWrapper = round.ObjectWrapper;
			if (insertAtFront)
			{
				for (int num = ProxyRounds.Count - 1; num >= 1; num--)
				{
					ProxyRounds[num] = ProxyRounds[num - 1];
				}
				ProxyRounds[0] = proxyRound;
			}
			else
			{
				ProxyRounds.Add(proxyRound);
			}
			HoveredOverRound = null;
			UnityEngine.Object.Destroy(round.gameObject);
			if (updateDisplay)
			{
				UpdateProxyDisplay();
			}
		}

		public void CycleToProxy(bool forward, bool destroyThisRound)
		{
			int num = 0;
			if (!forward)
			{
				num = ProxyRounds.Count - 1;
			}
			m_proxyDumpFlag = false;
			ProxyRound proxyRound = ProxyRounds[num];
			if (forward)
			{
				UnityEngine.Object.Destroy(ProxyRounds[num].GO);
				ProxyRounds.RemoveAt(num);
			}
			if (destroyThisRound)
			{
				PalmRound(this, !forward, updateDisplay: true, num);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(proxyRound.ObjectWrapper.GetGameObject(), base.transform.position, base.transform.rotation);
			FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
			for (int i = 0; i < ProxyRounds.Count; i++)
			{
				component.ProxyRounds.Add(ProxyRounds[i]);
			}
			for (int j = 0; j < ProxyRounds.Count; j++)
			{
				ProxyRounds[j].GO.transform.SetParent(gameObject.transform);
			}
			if (!destroyThisRound)
			{
				ProxyRounds.Clear();
			}
			component.BeginInteraction(m_hand);
			m_hand.ForceSetInteractable(component);
			if (destroyThisRound)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void UpdateProxyDisplay()
		{
			if (ProxyRounds.Count <= 0)
			{
				return;
			}
			if (base.IsHeld)
			{
				Vector3 position = base.transform.position;
				Vector3 vector = -base.transform.forward * PalmingDimensions.z + -base.transform.up * PalmingDimensions.y;
				for (int i = 0; i < ProxyRounds.Count; i++)
				{
					ProxyRounds[i].GO.transform.position = position + vector * (i + 2);
					ProxyRounds[i].GO.transform.localRotation = Quaternion.identity;
				}
			}
			else
			{
				Vector3 position2 = base.transform.position;
				Vector3 vector2 = -base.transform.up * PalmingDimensions.y;
				for (int j = 0; j < ProxyRounds.Count; j++)
				{
					ProxyRounds[j].GO.transform.position = position2 + vector2 * (j + 2);
					ProxyRounds[j].GO.transform.localRotation = Quaternion.identity;
				}
			}
			for (int k = 0; k < ProxyRounds.Count; k++)
			{
				ProxyRounds[k].Filter.mesh = AM.GetRoundMesh(RoundType, ProxyRounds[k].Class);
				ProxyRounds[k].Renderer.material = AM.GetRoundMaterial(RoundType, ProxyRounds[k].Class);
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_pickUpCooldown = 0.3f;
			m_proxyDumpFlag = true;
			UpdateProxyDisplay();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					flag3 = true;
				}
				if (hand.Input.AXButtonPressed)
				{
					flag4 = true;
				}
			}
			else
			{
				if (hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.1f)
				{
					if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45f)
					{
						flag = true;
					}
					else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45f)
					{
						flag2 = true;
					}
					else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45f)
					{
						flag3 = true;
					}
				}
				if (hand.Input.TouchpadPressed && hand.Input.TouchpadAxes.magnitude > 0.1f && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45f)
				{
					flag4 = true;
				}
			}
			if (!isPalmable || MaxPalmedAmount <= 1)
			{
				return;
			}
			if (flag)
			{
				if (ProxyRounds.Count > 0)
				{
					CycleToProxy(forward: true, destroyThisRound: true);
					SM.PlayHandlingGrabSound(HandlingGrabSound, base.transform.position, isHard: false);
				}
			}
			else if (flag2)
			{
				if (ProxyRounds.Count > 0)
				{
					CycleToProxy(forward: false, destroyThisRound: true);
					SM.PlayHandlingGrabSound(HandlingGrabSound, base.transform.position, isHard: false);
				}
			}
			else if (flag3 && !IsSpent)
			{
				if (hand.OtherHand.CurrentInteractable == null && hand.OtherHand.Input.IsGrabbing && Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.15f)
				{
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					hand.OtherHand.ForceSetInteractable(this);
					BeginInteraction(hand.OtherHand);
					UpdateProxyDisplay();
					m_proxyDumpFlag = true;
				}
				else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).RoundType == RoundType && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).MaxPalmedAmount && Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.15f)
				{
					((FVRFireArmRound)hand.OtherHand.CurrentInteractable).AddProxy(RoundClass, ObjectWrapper);
					((FVRFireArmRound)hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					SM.PlayHandlingGrabSound(HandlingGrabSound, base.transform.position, isHard: false);
					ForceBreakInteraction();
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == null)
				{
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					else
					{
						ForceBreakInteraction();
					}
					SM.PlayHandlingReleaseIntoSlotSound(HandlingReleaseIntoSlotSound, base.transform.position);
					EndInteractionIntoInventorySlot(hand, hand.CurrentHoveredQuickbeltSlot);
					UpdateProxyDisplay();
					m_proxyDumpFlag = true;
				}
				else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == RoundType && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount)
				{
					((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(RoundClass, ObjectWrapper);
					((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					SM.PlayHandlingReleaseIntoSlotSound(HandlingReleaseIntoSlotSound, base.transform.position);
					ForceBreakInteraction();
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (ProxyRounds.Count > 0)
				{
					SM.PlayHandlingGrabSound(HandlingGrabSound, base.transform.position, isHard: false);
					CycleToProxy(forward: true, destroyThisRound: false);
					m_proxyDumpFlag = false;
					base.transform.position += PalmingDimensions.z * base.transform.forward - PalmingDimensions.y * base.transform.up;
				}
			}
			else if (flag4 && HoveredOverRound != null)
			{
				PalmRound(HoveredOverRound, insertAtFront: false, updateDisplay: true);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (ProxyRounds.Count > 0 && m_proxyDumpFlag)
			{
				for (int i = 0; i < ProxyRounds.Count; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(ProxyRounds[i].ObjectWrapper.GetGameObject(), ProxyRounds[i].GO.transform.position, ProxyRounds[i].GO.transform.rotation);
					gameObject.GetComponent<Rigidbody>().velocity = base.RootRigidbody.velocity;
					UnityEngine.Object.Destroy(ProxyRounds[i].GO);
					ProxyRounds[i].GO = null;
					ProxyRounds[i].Filter = null;
					ProxyRounds[i].Renderer = null;
					ProxyRounds[i].ObjectWrapper = null;
				}
				ProxyRounds.Clear();
			}
		}

		public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
		{
			base.EndInteractionIntoInventorySlot(hand, slot);
			UpdateProxyDisplay();
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_isSpent || base.IsHeld || m_isChambered || base.QuickbeltSlot != null)
			{
				isCookingOff = false;
			}
			if (isCookingOff)
			{
				TickTilCookOff -= Time.deltaTime;
				if (TickTilCookOff <= 0f)
				{
					Splode(UnityEngine.Random.Range(0.2f, 0.5f), isRandomDir: false, DoesDestroyBrass: false);
				}
			}
			float num = 5f;
			switch (GM.Options.SimulationOptions.ShellTime)
			{
			case SimulationOptions.SpentShellDespawnTime.Seconds_5:
				num = 5f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Seconds_10:
				num = 10f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Seconds_30:
				num = 30f;
				break;
			case SimulationOptions.SpentShellDespawnTime.Infinite:
				num = 999999f;
				break;
			}
			if (m_pickUpCooldown > 0f)
			{
				m_pickUpCooldown -= Time.deltaTime;
			}
			if (m_isKillCounting && base.QuickbeltSlot == null)
			{
				if (m_killAfter > num)
				{
					m_killAfter = num;
				}
				m_killAfter -= Time.deltaTime;
				if (m_killAfter <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			bool flag = true;
			if (isManuallyChamberable && m_pickUpCooldown <= 0f && base.QuickbeltSlot == null)
			{
				if (HoveredOverChamber != null && !HoveredOverChamber.IsFull && HoveredOverChamber.IsAccessible)
				{
					Chamber(HoveredOverChamber, makeChamberingSound: true);
					flag = false;
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					ForceBreakInteraction();
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else
				{
					HoveredOverChamber = null;
				}
			}
			if (!(m_hoverOverReloadTrigger != null) || !flag)
			{
				return;
			}
			if (m_hoverOverReloadTrigger.IsClipTrigger)
			{
				if ((base.IsHeld || m_hoverOverReloadTrigger.Clip.IsDropInLoadable) && !IsSpent && base.QuickbeltSlot == null && m_hoverOverReloadTrigger.Clip.RoundType == RoundType && m_hoverOverReloadTrigger.Clip.TimeSinceRoundInserted > 0.5f)
				{
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					m_hoverOverReloadTrigger.Clip.AddRound(this, makeSound: true, updateDisplay: true);
					ForceBreakInteraction();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else if (m_hoverOverReloadTrigger.IsSpeedloaderTrigger)
			{
				if (base.IsHeld && !IsSpent && base.QuickbeltSlot == null && m_hoverOverReloadTrigger.SpeedloaderChamber.Type == RoundType)
				{
					if (ProxyRounds.Count > 0)
					{
						CycleToProxy(forward: true, destroyThisRound: false);
						m_proxyDumpFlag = false;
					}
					m_hoverOverReloadTrigger.SpeedloaderChamber.Load(RoundClass, playSound: true);
					ForceBreakInteraction();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else if ((base.IsHeld || m_hoverOverReloadTrigger.Magazine.IsDropInLoadable) && !IsSpent && base.QuickbeltSlot == null && m_hoverOverReloadTrigger.Magazine.RoundType == RoundType && m_hoverOverReloadTrigger.Magazine.TimeSinceRoundInserted > 0.3f)
			{
				if (ProxyRounds.Count > 0)
				{
					CycleToProxy(forward: true, destroyThisRound: false);
					m_proxyDumpFlag = false;
				}
				m_hoverOverReloadTrigger.Magazine.AddRound(this, makeSound: true, updateDisplay: true);
				ForceBreakInteraction();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void OnParticleCollision(GameObject other)
		{
			if (other.CompareTag("IgnitorSystem"))
			{
				isCookingOff = true;
				TickTilCookOff = Mathf.Min(TickTilCookOff, UnityEngine.Random.Range(0.5f, 1.5f));
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (IsSpent)
			{
				return;
			}
			if (isManuallyChamberable && !IsSpent && HoveredOverChamber == null && m_hoverOverReloadTrigger == null && !IsSpent && collider.gameObject.CompareTag("FVRFireArmChamber"))
			{
				FVRFireArmChamber component = collider.gameObject.GetComponent<FVRFireArmChamber>();
				if (component.RoundType == RoundType && component.IsManuallyChamberable && component.IsAccessible && !component.IsFull)
				{
					HoveredOverChamber = component;
				}
			}
			if (isMagazineLoadable && HoveredOverChamber == null && !IsSpent && collider.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger"))
			{
				FVRFireArmMagazineReloadTrigger component2 = collider.gameObject.GetComponent<FVRFireArmMagazineReloadTrigger>();
				if (component2.IsClipTrigger)
				{
					if (component2 != null && component2.Clip != null && component2.Clip.RoundType == RoundType && !component2.Clip.IsFull() && (component2.Clip.FireArm == null || component2.Clip.IsDropInLoadable))
					{
						m_hoverOverReloadTrigger = component2;
					}
				}
				else if (component2.IsSpeedloaderTrigger)
				{
					if (!component2.SpeedloaderChamber.IsLoaded)
					{
						m_hoverOverReloadTrigger = component2;
					}
				}
				else if (component2 != null && component2.Magazine != null && component2.Magazine.RoundType == RoundType && !component2.Magazine.IsFull() && (component2.Magazine.FireArm == null || component2.Magazine.IsDropInLoadable))
				{
					m_hoverOverReloadTrigger = component2;
				}
			}
			if (isPalmable && ProxyRounds.Count < MaxPalmedAmount && !IsSpent && collider.gameObject.CompareTag("FVRFireArmRound"))
			{
				FVRFireArmRound component3 = collider.gameObject.GetComponent<FVRFireArmRound>();
				if (component3.RoundType == RoundType && !component3.IsSpent && component3.QuickbeltSlot == null)
				{
					HoveredOverRound = component3;
				}
			}
		}

		public void OnTriggerExit(Collider collider)
		{
			if (isManuallyChamberable && collider.gameObject.CompareTag("FVRFireArmChamber") && HoveredOverChamber != null && HoveredOverChamber.gameObject == collider.gameObject)
			{
				HoveredOverChamber = null;
			}
			if (isMagazineLoadable && collider.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger") && m_hoverOverReloadTrigger != null && m_hoverOverReloadTrigger.gameObject == collider.gameObject)
			{
				m_hoverOverReloadTrigger = null;
			}
			if (isPalmable && collider != null && HoveredOverRound != null && collider.gameObject.CompareTag("FVRFireArmRound") && collider.gameObject == HoveredOverRound.gameObject)
			{
				HoveredOverRound = null;
			}
		}

		[ContextMenu("MigrateImpactSetting")]
		public void MigrateImpactSetting()
		{
			AudioImpactController component = base.gameObject.GetComponent<AudioImpactController>();
			if (component != null)
			{
				ImpactSound_Fired = component.ImpactType;
				bool flag = false;
				switch (component.ImpactType)
				{
				case ImpactType.CasePistolSmall:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletSmall;
					break;
				case ImpactType.CasePistolLarge:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletSmall;
					break;
				case ImpactType.CaseRifleSmall:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletLarge;
					break;
				case ImpactType.CaseRifleLarge:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletLarge;
					break;
				case ImpactType.CaseAntiMateriel:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletVeryLarge;
					break;
				case ImpactType.CaseShotgun:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletShotgun;
					break;
				case ImpactType.CaseLauncher:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletVeryLarge;
					break;
				case ImpactType.CaseRifleTiny:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletTiny;
					break;
				case ImpactType.CasePistolTiny:
					flag = true;
					ImpactSound_Unfired = ImpactType.BulletTiny;
					break;
				}
				if (!flag)
				{
					Debug.Log("Cartidge " + base.gameObject.name + " used unusual impact sound");
				}
			}
			else
			{
				Debug.Log("Cartidge " + base.gameObject.name + " used NO impact sound");
			}
		}
	}
}
