using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmClip : FVRPhysicalObject
	{
		public enum ClipState
		{
			Free,
			Locked
		}

		[Serializable]
		public class FVRLoadedRound
		{
			public FireArmRoundClass LR_Class;

			public Mesh LR_Mesh;

			public Material LR_Material;

			public FVRObject LR_ObjectWrapper;

			public GameObject LR_ProjectilePrefab;
		}

		[Serializable]
		public class FVRClipLoadingPattern
		{
			public FireArmRoundClass[] Classes;
		}

		[Header("Clip Params")]
		public FireArmClipType ClipType;

		public FireArmRoundType RoundType;

		public FVRFireArm FireArm;

		public FVRFireArmClipInterface Interface;

		public bool CanManuallyEjectRounds = true;

		public Transform RoundEjectionPos;

		public float EjectionSpeed = 1f;

		public ClipState State;

		public GameObject[] DisplayBullets;

		public MeshFilter[] DisplayMeshFilters;

		public Renderer[] DisplayRenderers;

		[HideInInspector]
		public FVRLoadedRound[] LoadedRounds;

		public FVRClipLoadingPattern DefaultLoadingPattern;

		public int m_numRounds = 10;

		public int m_capacity = 10;

		public bool IsDropInLoadable;

		private float m_timeSinceRoundInserted;

		[Header("Audio Config")]
		public AudioEvent LoadFromClipToMag;

		public AudioEvent InsertOntoClip;

		public AudioEvent EjectFromClip;

		public AudioEvent AffixClip;

		public AudioEvent RemoveClip;

		public bool IsInfinite;

		public bool IsExtractable = true;

		public float TimeSinceRoundInserted => m_timeSinceRoundInserted;

		protected override void Awake()
		{
			base.Awake();
			Interface.gameObject.SetActive(value: false);
			LoadedRounds = new FVRLoadedRound[m_capacity];
			m_numRounds = 0;
			for (int i = 0; i < Mathf.Min(DefaultLoadingPattern.Classes.Length, m_capacity); i++)
			{
				AddRound(DefaultLoadingPattern.Classes[i], makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			FVRFireArmClip component = gameObject.GetComponent<FVRFireArmClip>();
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

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (State != 0 || !CanManuallyEjectRounds || !(RoundEjectionPos != null) || !HasARound())
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
				SM.PlayGenericSound(EjectFromClip, base.transform.position);
				if (hand.OtherHand.CurrentInteractable == null && hand.OtherHand.Input.IsGrabbing && Vector3.Distance(RoundEjectionPos.position, hand.OtherHand.Input.Pos) < 0.15f)
				{
					GameObject original = RemoveRound(b: false);
					GameObject gameObject = UnityEngine.Object.Instantiate(original, RoundEjectionPos.position, RoundEjectionPos.rotation);
					FVRFireArmRound component = gameObject.GetComponent<FVRFireArmRound>();
					component.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
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
					component2.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
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

		public void LoadOneRoundFromClipToMag()
		{
			if (!(FireArm == null) && !(FireArm.Magazine == null) && !FireArm.Magazine.IsFull() && HasARound())
			{
				FireArmRoundClass rClass = RemoveRoundReturnClass();
				SM.PlayGenericSound(LoadFromClipToMag, base.transform.position);
				FireArm.Magazine.AddRound(rClass, makeSound: false, updateDisplay: true);
			}
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
					SM.PlayGenericSound(InsertOntoClip, base.transform.position);
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
					SM.PlayGenericSound(InsertOntoClip, base.transform.position);
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

		public FireArmRoundClass RemoveRoundReturnClass()
		{
			FireArmRoundClass lR_Class = LoadedRounds[m_numRounds - 1].LR_Class;
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
			return lR_Class;
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

		public void UpdateBulletDisplay()
		{
			int num = m_numRounds - 1;
			for (int i = 0; i < DisplayBullets.Length; i++)
			{
				if (DisplayBullets[i] != null)
				{
					if (i >= m_numRounds || num < 0)
					{
						DisplayBullets[i].SetActive(value: false);
					}
					else
					{
						DisplayBullets[i].SetActive(value: true);
						DisplayMeshFilters[i].mesh = LoadedRounds[num].LR_Mesh;
						DisplayRenderers[i].material = LoadedRounds[num].LR_Material;
					}
				}
				num--;
			}
		}

		public void ReloadClipWithType(FireArmRoundClass rClass)
		{
			m_numRounds = 0;
			for (int i = 0; i < m_capacity; i++)
			{
				AddRound(rClass, makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public void ReloadClipWithList(List<FireArmRoundClass> list)
		{
			m_numRounds = 0;
			List<FireArmRoundClass> list2 = new List<FireArmRoundClass>();
			int num = list.Count - m_capacity;
			for (int i = num; i < list.Count; i++)
			{
				list2.Add(list[i]);
			}
			int num2 = Mathf.Min(list2.Count, m_capacity);
			for (int j = 0; j < num2; j++)
			{
				AddRound(list2[j], makeSound: false, updateDisplay: false);
			}
			UpdateBulletDisplay();
		}

		public void Release()
		{
			State = ClipState.Free;
			SetParentage(null);
			base.transform.position = FireArm.ClipEjectPos.position;
			base.transform.rotation = FireArm.ClipEjectPos.rotation;
			RecoverRigidbody();
			if (FireArm != null)
			{
				SM.PlayGenericSound(RemoveClip, base.transform.position);
			}
			base.RootRigidbody.isKinematic = false;
			base.RootRigidbody.velocity = FireArm.RootRigidbody.velocity + base.transform.up * EjectionSpeed;
			base.RootRigidbody.angularVelocity = FireArm.RootRigidbody.angularVelocity;
			FireArm = null;
			SetAllCollidersToLayer(triggersToo: false, "Default");
			Interface.gameObject.SetActive(value: false);
		}

		public void Load(FVRFireArm fireArm)
		{
			State = ClipState.Locked;
			FireArm = fireArm;
			FireArm.LoadClip(this);
			base.IsHeld = false;
			ForceBreakInteraction();
			SetParentage(FireArm.transform);
			base.transform.rotation = FireArm.ClipMountPos.rotation;
			base.transform.position = FireArm.ClipMountPos.position;
			StoreAndDestroyRigidbody();
			SM.PlayGenericSound(AffixClip, base.transform.position);
			if (FireArm.QuickbeltSlot != null)
			{
				SetAllCollidersToLayer(triggersToo: false, "NoCol");
			}
			else
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			Interface.gameObject.SetActive(value: true);
		}

		public override bool IsInteractable()
		{
			if (State == ClipState.Locked)
			{
				return false;
			}
			return base.IsInteractable();
		}

		protected override void FVRFixedUpdate()
		{
			if (m_timeSinceRoundInserted < 5f)
			{
				m_timeSinceRoundInserted += Time.deltaTime;
			}
			base.FVRFixedUpdate();
		}
	}
}
