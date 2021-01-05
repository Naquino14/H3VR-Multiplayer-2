using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmMagazine : FVRPhysicalObject
	{
		public enum MagazineState
		{
			Free,
			Locked
		}

		[Serializable]
		public class FVRMagazineLoadingPattern
		{
			public FireArmRoundClass[] Classes;
		}

		[Header("Magazine Config")]
		public bool IsIntegrated;

		public bool DoesFollowerStopBolt = true;

		public bool IsBeltBox;

		public bool UsesVizInterp;

		public Transform Viz;

		private Vector3 m_vizLerpStartPos;

		private Quaternion m_vizLerpStartRot;

		private float m_vizLerp;

		private bool m_isVizLerping;

		private bool m_isVizLerpInward;

		private float m_vizLerpSpeedMultiplier_Eject = 18f;

		private float m_vizLerpSpeedMultiplier_Insert = 18f;

		private Transform m_vizLerpReferenceTransform;

		[SearchableEnum]
		public FireArmMagazineType MagazineType;

		[SearchableEnum]
		public FireArmRoundType RoundType;

		public float EjectionSpeed = 1f;

		public Transform RoundEjectionPos;

		public bool IsNonPhysForLoad;

		public MagazineState State;

		public bool DoesDisplayXOscillate;

		public GameObject[] DisplayBullets;

		public MeshFilter[] DisplayMeshFilters;

		public Renderer[] DisplayRenderers;

		private Vector3[] m_DisplayStartPositions;

		[HideInInspector]
		public FVRLoadedRound[] LoadedRounds;

		public FVRMagazineLoadingPattern DefaultLoadingPattern;

		public int m_numRounds = 30;

		public int m_capacity = 30;

		public FVRFireArm FireArm;

		public bool IsDropInLoadable;

		private float m_timeSinceRoundInserted;

		public Transform AlternatePoseOverride;

		private Vector3 m_originalPoseOverrideLocalPosition = Vector3.zero;

		private Quaternion m_originalPoseOverrideLocalRotation = Quaternion.identity;

		public bool IsInfinite;

		public bool CanManuallyEjectRounds = true;

		public bool IsExtractable = true;

		public bool AutoEjectsOnEmpty;

		public FVRFireArmBeltGrabTrigger BeltGrabTrigger;

		[Header("Audio Config")]
		public FVRFirearmAudioSet Profile;

		public bool UsesOverrideInOut;

		public FVRFirearmMagazineAudioSet ProfileOverride;

		public bool SwapsImpactTypeOnEmpty;

		public ImpactType ImpactEmptyType = ImpactType.Generic;

		private bool m_setToFullSounds = true;

		private ImpactType m_originalImpactType;

		[Header("Mag Rotation Bits")]
		public bool UsesRotatingBit;

		public Transform RotatingBit;

		public Axis RotAxis = Axis.Y;

		public float Rot_Full;

		public float Rot_Empty;

		[Header("FuelOverride")]
		public float FuelAmountLeft;

		[Header("BeltBoxOverride")]
		public bool HidesAllDisplayBulletsWhenGrabbed = true;

		public int IndexBeforeWhichIsHiddenWhenGrabbed;

		public bool CanBeTornOut;

		public float TimeSinceRoundInserted => m_timeSinceRoundInserted;

		public override bool IsInteractable()
		{
			if (State == MagazineState.Free)
			{
				return true;
			}
			return false;
		}

		public void ReloadMagWithType(FireArmRoundClass rClass)
		{
			m_numRounds = 0;
			for (int i = 0; i < m_capacity; i++)
			{
				AddRound(rClass, makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public void ReloadMagWithList(List<FireArmRoundClass> list)
		{
			m_numRounds = 0;
			int num = Mathf.Min(list.Count, m_capacity);
			for (int i = 0; i < num; i++)
			{
				AddRound(list[i], makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public void ReloadMagWithTypeUpToPercentage(FireArmRoundClass rClass, float percentage)
		{
			int num = Mathf.Clamp((int)((float)m_capacity * percentage), 1, m_capacity);
			m_numRounds = 0;
			for (int i = 0; i < num; i++)
			{
				AddRound(rClass, makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			FVRFireArmMagazine component = gameObject.GetComponent<FVRFireArmMagazine>();
			for (int i = 0; i < Mathf.Min(LoadedRounds.Length, component.LoadedRounds.Length); i++)
			{
				if (LoadedRounds[i] != null && LoadedRounds[i].LR_Mesh != null)
				{
					component.LoadedRounds[i].LR_Class = LoadedRounds[i].LR_Class;
					component.LoadedRounds[i].LR_Mesh = LoadedRounds[i].LR_Mesh;
					component.LoadedRounds[i].LR_Material = LoadedRounds[i].LR_Material;
					component.LoadedRounds[i].LR_ObjectWrapper = LoadedRounds[i].LR_ObjectWrapper;
				}
			}
			component.m_numRounds = m_numRounds;
			component.UpdateBulletDisplay();
			return gameObject;
		}

		protected override void Awake()
		{
			base.Awake();
			m_DisplayStartPositions = new Vector3[DisplayBullets.Length];
			for (int i = 0; i < DisplayBullets.Length; i++)
			{
				if (DisplayBullets[i] != null)
				{
					ref Vector3 reference = ref m_DisplayStartPositions[i];
					reference = DisplayBullets[i].transform.localPosition;
				}
			}
			LoadedRounds = new FVRLoadedRound[m_capacity];
			m_numRounds = 0;
			for (int j = 0; j < Mathf.Min(DefaultLoadingPattern.Classes.Length, m_capacity); j++)
			{
				AddRound(DefaultLoadingPattern.Classes[j], makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
			if (SwapsImpactTypeOnEmpty && base.HasImpactController)
			{
				m_originalImpactType = base.AudioImpactController.ImpactType;
				m_setToFullSounds = true;
			}
		}

		public Vector3 GetAmmoLocalEulers()
		{
			float t = (float)m_numRounds / (float)m_capacity;
			float num = Mathf.Lerp(Rot_Empty, Rot_Full, t);
			return RotAxis switch
			{
				Axis.X => new Vector3(num, 0f, 0f), 
				Axis.Y => new Vector3(0f, num, 0f), 
				Axis.Z => new Vector3(0f, 0f, num), 
				_ => new Vector3(0f, 0f, 0f), 
			};
		}

		public bool HasARound()
		{
			if (m_numRounds > 0 && IsExtractable)
			{
				return true;
			}
			return false;
		}

		public bool IsFull()
		{
			if (m_numRounds >= m_capacity)
			{
				return true;
			}
			return false;
		}

		public void ForceEmpty()
		{
			m_numRounds = 0;
		}

		public void ForceFull()
		{
			m_numRounds = m_capacity;
		}

		public bool HasFuel()
		{
			if (FuelAmountLeft > 0f)
			{
				return true;
			}
			return false;
		}

		public void DrainFuel(float f)
		{
			FuelAmountLeft -= f;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (IsNonPhysForLoad)
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			base.EndInteraction(hand);
		}

		public void AddRound(FireArmRoundClass rClass, bool makeSound, bool updateDisplay)
		{
			if (m_numRounds < m_capacity)
			{
				m_timeSinceRoundInserted = 0f;
				FVRLoadedRound fVRLoadedRound = new FVRLoadedRound();
				fVRLoadedRound.LR_Class = rClass;
				fVRLoadedRound.LR_Mesh = AM.GetRoundMesh(RoundType, rClass);
				fVRLoadedRound.LR_Material = AM.GetRoundMaterial(RoundType, rClass);
				fVRLoadedRound.LR_ObjectWrapper = AM.GetRoundSelfPrefab(RoundType, rClass);
				LoadedRounds[m_numRounds] = fVRLoadedRound;
				m_numRounds++;
				if (makeSound)
				{
					if (FireArm != null)
					{
						FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineInsertRound);
					}
					else if (UsesOverrideInOut)
					{
						SM.PlayGenericSound(ProfileOverride.MagazineInsertRound, base.transform.position);
					}
					else
					{
						SM.PlayGenericSound(Profile.MagazineInsertRound, base.transform.position);
					}
				}
			}
			if (updateDisplay)
			{
				UpdateBulletDisplay();
			}
		}

		public void AddRound(FVRFireArmRound round, bool makeSound, bool updateDisplay)
		{
			if (m_numRounds < m_capacity)
			{
				m_timeSinceRoundInserted = 0f;
				FVRLoadedRound fVRLoadedRound = new FVRLoadedRound();
				fVRLoadedRound.LR_Class = round.RoundClass;
				fVRLoadedRound.LR_Mesh = AM.GetRoundMesh(round.RoundType, round.RoundClass);
				fVRLoadedRound.LR_Material = AM.GetRoundMaterial(round.RoundType, round.RoundClass);
				fVRLoadedRound.LR_ObjectWrapper = AM.GetRoundSelfPrefab(round.RoundType, round.RoundClass);
				LoadedRounds[m_numRounds] = fVRLoadedRound;
				m_numRounds++;
				if (makeSound)
				{
					if (FireArm != null)
					{
						FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineInsertRound);
					}
					else
					{
						SM.PlayGenericSound(Profile.MagazineInsertRound, base.transform.position);
					}
				}
			}
			if (updateDisplay)
			{
				UpdateBulletDisplay();
			}
		}

		public void RemoveRound()
		{
			if ((!IsInfinite || !GM.CurrentSceneSettings.AllowsInfiniteAmmoMags) && !GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				if (GM.CurrentPlayerBody.IsAmmoDrain)
				{
					m_numRounds = 0;
				}
				if (m_numRounds > 0)
				{
					LoadedRounds[m_numRounds - 1] = null;
					m_numRounds--;
				}
				UpdateBulletDisplay();
			}
		}

		public GameObject RemoveRound(bool b)
		{
			GameObject gameObject = LoadedRounds[m_numRounds - 1].LR_ObjectWrapper.GetGameObject();
			if ((!IsInfinite || !GM.CurrentSceneSettings.AllowsInfiniteAmmoMags) && !GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				if (GM.CurrentPlayerBody.IsAmmoDrain)
				{
					m_numRounds = 0;
				}
				else
				{
					if (m_numRounds > 0)
					{
						LoadedRounds[m_numRounds - 1] = null;
						m_numRounds--;
					}
					UpdateBulletDisplay();
				}
			}
			return gameObject;
		}

		public FVRLoadedRound RemoveRound(int i)
		{
			FVRLoadedRound result = LoadedRounds[m_numRounds - 1];
			if ((!IsInfinite || !GM.CurrentSceneSettings.AllowsInfiniteAmmoMags) && !GM.CurrentSceneSettings.IsAmmoInfinite && !GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				if (GM.CurrentPlayerBody.IsAmmoDrain)
				{
					m_numRounds = 0;
				}
				else
				{
					if (m_numRounds > 0)
					{
						LoadedRounds[m_numRounds - 1] = null;
						m_numRounds--;
					}
					UpdateBulletDisplay();
				}
			}
			return result;
		}

		public void UpdateBulletDisplay()
		{
			if (FireArm != null && FireArm.UsesBelts)
			{
				bool flag = false;
				if (FireArm.BeltDD.isBeltGrabbed())
				{
					flag = true;
				}
				if (HidesAllDisplayBulletsWhenGrabbed && (FireArm.ConnectedToBox || flag))
				{
					for (int i = 0; i < DisplayBullets.Length; i++)
					{
						DisplayBullets[i].SetActive(value: false);
					}
					return;
				}
			}
			int num = m_numRounds - 1;
			for (int j = 0; j < DisplayBullets.Length; j++)
			{
				if (DisplayBullets[j] != null)
				{
					bool flag2 = false;
					if (HidesAllDisplayBulletsWhenGrabbed && FireArm != null && FireArm.UsesBelts && FireArm.BeltDD.isBeltGrabbed())
					{
						flag2 = true;
					}
					if (j >= m_numRounds || num < 0 || (flag2 && j < IndexBeforeWhichIsHiddenWhenGrabbed))
					{
						DisplayBullets[j].SetActive(value: false);
					}
					else
					{
						DisplayBullets[j].SetActive(value: true);
						DisplayMeshFilters[j].mesh = LoadedRounds[num].LR_Mesh;
						DisplayRenderers[j].material = LoadedRounds[num].LR_Material;
					}
				}
				num--;
			}
			if (UsesRotatingBit)
			{
				RotatingBit.localEulerAngles = GetAmmoLocalEulers();
			}
			if (DoesDisplayXOscillate)
			{
				if (m_numRounds % 2 == 1)
				{
					for (int k = 0; k < DisplayBullets.Length; k++)
					{
						if (DisplayBullets[k] != null)
						{
							DisplayBullets[k].transform.localPosition = new Vector3(m_DisplayStartPositions[k].x * -1f, m_DisplayStartPositions[k].y, m_DisplayStartPositions[k].z);
						}
					}
				}
				else
				{
					for (int l = 0; l < DisplayBullets.Length; l++)
					{
						if (DisplayBullets[l] != null)
						{
							DisplayBullets[l].transform.localPosition = m_DisplayStartPositions[l];
						}
					}
				}
			}
			if (!SwapsImpactTypeOnEmpty)
			{
				return;
			}
			if (m_setToFullSounds)
			{
				if (m_numRounds <= 0)
				{
					m_setToFullSounds = false;
					base.AudioImpactController.ImpactType = ImpactEmptyType;
				}
			}
			else if (m_numRounds > 0)
			{
				m_setToFullSounds = true;
				base.AudioImpactController.ImpactType = m_originalImpactType;
			}
		}

		public void Release()
		{
			State = MagazineState.Free;
			SetParentage(null);
			if (UsesVizInterp)
			{
				m_vizLerpStartPos = Viz.transform.position;
				m_vizLerp = 0f;
				m_isVizLerpInward = false;
				m_isVizLerping = true;
			}
			if (FireArm.MagazineEjectPos != null)
			{
				base.transform.position = FireArm.GetMagEjectPos(IsBeltBox).position;
			}
			else
			{
				base.transform.position = FireArm.GetMagMountPos(IsBeltBox).position;
			}
			if (UsesVizInterp)
			{
				Viz.position = m_vizLerpStartPos;
				m_vizLerpReferenceTransform = FireArm.GetMagMountPos(IsBeltBox);
			}
			RecoverRigidbody();
			base.RootRigidbody.isKinematic = false;
			base.RootRigidbody.velocity = FireArm.RootRigidbody.velocity - base.transform.up * EjectionSpeed;
			base.RootRigidbody.angularVelocity = FireArm.RootRigidbody.angularVelocity;
			if (FireArm.m_hand != null)
			{
				bool flag = false;
				FVRViveHand otherHand = FireArm.m_hand.OtherHand;
				if (otherHand.CurrentInteractable == null && otherHand.Input.IsGrabbing)
				{
					flag = true;
				}
				if (flag)
				{
					Vector3 position = otherHand.transform.position;
					if (GM.Options.ControlOptions.UseInvertedHandgunMagPose)
					{
						position = otherHand.GetMagPose().position;
					}
					Vector3 to = position - FireArm.GetMagMountPos(IsBeltBox).position;
					float num = Vector3.Distance(base.transform.position, position);
					if (num < 0.2f && Vector3.Angle(base.transform.up, to) > 90f)
					{
						otherHand.ForceSetInteractable(this);
						BeginInteraction(otherHand);
					}
				}
			}
			FireArm = null;
			SetAllCollidersToLayer(triggersToo: false, "Default");
		}

		public void Load(FVRFireArm fireArm)
		{
			State = MagazineState.Locked;
			FireArm = fireArm;
			FireArm.LoadMag(this);
			base.IsHeld = false;
			ForceBreakInteraction();
			if (UsesVizInterp)
			{
				m_vizLerpStartPos = Viz.transform.position;
				m_vizLerpStartRot = Viz.transform.rotation;
				m_vizLerp = 0f;
				m_isVizLerpInward = true;
				m_isVizLerping = true;
			}
			SetParentage(FireArm.GetMagMountingTransform());
			base.transform.rotation = FireArm.GetMagMountPos(IsBeltBox).rotation;
			base.transform.position = FireArm.GetMagMountPos(IsBeltBox).position;
			if (UsesVizInterp)
			{
				Viz.position = m_vizLerpStartPos;
				Viz.rotation = m_vizLerpStartRot;
			}
			StoreAndDestroyRigidbody();
			if (FireArm.QuickbeltSlot != null)
			{
				SetAllCollidersToLayer(triggersToo: false, "NoCol");
			}
			else
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			if (fireArm.ObjectWrapper != null)
			{
				GM.CurrentSceneSettings.OnFireArmReloaded(fireArm.ObjectWrapper);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!CanManuallyEjectRounds || !(RoundEjectionPos != null) || !HasARound())
			{
				return;
			}
			bool flag = false;
			if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
			{
				flag = true;
			}
			else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45f)
			{
				flag = true;
			}
			if (flag)
			{
				if (FireArm != null)
				{
					FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
				}
				else
				{
					SM.PlayGenericSound(Profile.MagazineEjectRound, base.transform.position);
				}
				if (hand.OtherHand.CurrentInteractable == null && hand.OtherHand.Input.IsGrabbing && Vector3.Distance(RoundEjectionPos.position, hand.OtherHand.Input.Pos) < 0.15f)
				{
					GameObject original = RemoveRound(b: false);
					GameObject gameObject = UnityEngine.Object.Instantiate(original, RoundEjectionPos.position, RoundEjectionPos.rotation);
					FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
					component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
					hand.OtherHand.ForceSetInteractable(component);
					component.BeginInteraction(hand.OtherHand);
				}
				else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).RoundType == RoundType && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).MaxPalmedAmount && Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.15f)
				{
					FireArmRoundClass lR_Class = LoadedRounds[m_numRounds - 1].LR_Class;
					FVRObject lR_ObjectWrapper = LoadedRounds[m_numRounds - 1].LR_ObjectWrapper;
					((FVRFireArmRound)hand.OtherHand.CurrentInteractable).AddProxy(lR_Class, lR_ObjectWrapper);
					((FVRFireArmRound)hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
					RemoveRound();
				}
				else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == null)
				{
					GameObject original2 = RemoveRound(b: false);
					GameObject gameObject2 = UnityEngine.Object.Instantiate(original2, RoundEjectionPos.position, RoundEjectionPos.rotation);
					FVRFireArmRound component2 = gameObject2.GetComponent<FVRFireArmRound>();
					component2.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
					component2.SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlotDirty);
				}
				else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == RoundType && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount)
				{
					FireArmRoundClass lR_Class2 = LoadedRounds[m_numRounds - 1].LR_Class;
					FVRObject lR_ObjectWrapper2 = LoadedRounds[m_numRounds - 1].LR_ObjectWrapper;
					((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(lR_Class2, lR_ObjectWrapper2);
					((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
					RemoveRound();
				}
				else
				{
					GameObject original3 = RemoveRound(b: false);
					GameObject gameObject3 = UnityEngine.Object.Instantiate(original3, RoundEjectionPos.position, RoundEjectionPos.rotation);
					gameObject3.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
					gameObject3.GetComponent<Rigidbody>().AddForce(gameObject3.transform.forward * 0.5f);
				}
			}
		}

		protected override Vector3 GetGrabPos()
		{
			return base.GetGrabPos();
		}

		protected override Quaternion GetGrabRot()
		{
			return base.GetGrabRot();
		}

		protected override Vector3 GetPosTarget()
		{
			if (GM.Options.ControlOptions.UseInvertedHandgunMagPose && base.QuickbeltSlot == null && base.IsHeld)
			{
				return m_hand.GetMagPose().position;
			}
			return base.GetPosTarget();
		}

		protected override Quaternion GetRotTarget()
		{
			if (GM.Options.ControlOptions.UseInvertedHandgunMagPose && base.QuickbeltSlot == null && base.IsHeld)
			{
				return m_hand.GetMagPose().rotation;
			}
			return base.GetRotTarget();
		}

		protected override void FVRFixedUpdate()
		{
			if (m_timeSinceRoundInserted < 5f)
			{
				m_timeSinceRoundInserted += Time.deltaTime;
			}
			base.FVRFixedUpdate();
			if (base.IsHeld && GM.Options.ControlOptions.UseEasyMagLoading && m_hand.OtherHand.CurrentInteractable != null && m_hand.OtherHand.CurrentInteractable is FVRFireArm)
			{
				FVRFireArm fVRFireArm = m_hand.OtherHand.CurrentInteractable as FVRFireArm;
				if (fVRFireArm.MagazineType == MagazineType && fVRFireArm.GetMagMountPos(IsBeltBox) != null)
				{
					float num = Vector3.Distance(RoundEjectionPos.position, fVRFireArm.GetMagMountPos(IsBeltBox).position);
					if (num <= 0.15f)
					{
						SetAllCollidersToLayer(triggersToo: false, "NoCol");
						IsNonPhysForLoad = true;
					}
					else
					{
						SetAllCollidersToLayer(triggersToo: false, "Default");
						IsNonPhysForLoad = false;
					}
				}
			}
			if (!UsesVizInterp || !m_isVizLerping)
			{
				return;
			}
			if (!m_isVizLerpInward)
			{
				m_vizLerp += Time.deltaTime * m_vizLerpSpeedMultiplier_Eject;
				if (m_vizLerp >= 1f)
				{
					m_vizLerp = 1f;
					m_isVizLerping = false;
				}
				Viz.position = Vector3.Lerp(m_vizLerpStartPos, base.transform.position, m_vizLerp);
				if (m_vizLerpReferenceTransform != null)
				{
					Viz.rotation = Quaternion.Slerp(m_vizLerpReferenceTransform.rotation, base.transform.rotation, m_vizLerp * m_vizLerp);
				}
				else
				{
					Viz.rotation = Quaternion.Slerp(Viz.rotation, base.transform.rotation, m_vizLerp);
				}
			}
			else
			{
				m_vizLerp += Time.deltaTime * m_vizLerpSpeedMultiplier_Insert;
				if (m_vizLerp >= 1f)
				{
					m_vizLerp = 1f;
					m_isVizLerping = false;
				}
				Viz.position = Vector3.Lerp(m_vizLerpStartPos, base.transform.position, m_vizLerp);
				Viz.rotation = Quaternion.Slerp(m_vizLerpStartRot, base.transform.rotation, Mathf.Sqrt(m_vizLerp));
			}
		}
	}
}
