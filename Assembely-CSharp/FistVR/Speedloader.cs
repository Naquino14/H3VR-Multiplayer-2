using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Speedloader : FVRPhysicalObject
	{
		public enum SpeedLoaderType
		{
			Standard,
			P6Twelve,
			Jackhammer
		}

		public SpeedLoaderType SLType;

		public List<SpeedloaderChamber> Chambers;

		public FVRFirearmMagazineAudioSet ProfileOverride;

		private RevolverCylinder HoveredCylinder;

		private RevolvingShotgunTrigger HoveredRSTrigger;

		public float forwardEjectionDisplacement;

		private float TimeTilLoadAttempt;

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			Speedloader component = gameObject.GetComponent<Speedloader>();
			for (int i = 0; i < Chambers.Count; i++)
			{
				if (Chambers[i].IsLoaded)
				{
					component.Chambers[i].Load(Chambers[i].LoadedClass);
				}
				else
				{
					component.Chambers[i].Unload();
				}
			}
			return gameObject;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (HoveredCylinder != null && TimeTilLoadAttempt <= 0f && base.QuickbeltSlot == null && HoveredCylinder.IsInteractable())
			{
				float num = Vector3.Angle(base.transform.forward, HoveredCylinder.transform.forward);
				if (num < 20f)
				{
					TimeTilLoadAttempt = 0.25f;
					HoveredCylinder.LoadFromSpeedLoader(this);
				}
			}
			if (TimeTilLoadAttempt > 0f)
			{
				TimeTilLoadAttempt -= Time.deltaTime;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (HoveredRSTrigger != null && TimeTilLoadAttempt <= 0f && base.QuickbeltSlot == null && !HoveredRSTrigger.Shotgun.CylinderLoaded)
			{
				HoveredRSTrigger.Shotgun.LoadCylinder(this);
				Object.Destroy(base.gameObject);
			}
		}

		public void ReloadClipWithType(FireArmRoundClass rClass)
		{
			for (int i = 0; i < Chambers.Count; i++)
			{
				Chambers[i].Load(rClass);
			}
		}

		public void ReloadSpeedLoaderWithList(List<FireArmRoundClass> list)
		{
			for (int num = Chambers.Count - 1; num >= 0; num--)
			{
				int num2 = list.Count - 1 - num;
				if (num2 >= 0)
				{
					Chambers[num].Load(list[num2]);
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;
			if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
			{
				flag = true;
			}
			else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45f)
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			SpeedloaderChamber speedloaderChamber = null;
			bool flag2 = false;
			for (int i = 0; i < Chambers.Count; i++)
			{
				if (Chambers[i].IsLoaded)
				{
					speedloaderChamber = Chambers[i];
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
			if (ProfileOverride != null)
			{
				SM.PlayGenericSound(ProfileOverride.MagazineEjectRound, base.transform.position);
			}
			Vector3 vector = speedloaderChamber.transform.position + speedloaderChamber.transform.forward * (0.02f + forwardEjectionDisplacement);
			Quaternion rotation = speedloaderChamber.transform.rotation;
			if (hand.OtherHand.CurrentInteractable == null && hand.OtherHand.Input.IsGrabbing && Vector3.Distance(vector, hand.OtherHand.Input.Pos) < 0.15f)
			{
				GameObject gameObject = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject();
				GameObject gameObject2 = Object.Instantiate(gameObject, vector, rotation);
				FVRFireArmRound component = gameObject2.GetComponent<FVRFireArmRound>();
				component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
				hand.OtherHand.ForceSetInteractable(component);
				component.BeginInteraction(hand.OtherHand);
				if (speedloaderChamber.IsSpent)
				{
					component.Fire();
					component.SetKillCounting(b: true);
				}
				speedloaderChamber.Unload();
			}
			else if (hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).RoundType == speedloaderChamber.Type && ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)hand.OtherHand.CurrentInteractable).MaxPalmedAmount && Vector3.Distance(hand.Input.Pos, hand.OtherHand.Input.Pos) < 0.15f)
			{
				FireArmRoundClass loadedClass = speedloaderChamber.LoadedClass;
				FVRObject roundSelfPrefab = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass);
				((FVRFireArmRound)hand.OtherHand.CurrentInteractable).AddProxy(loadedClass, roundSelfPrefab);
				((FVRFireArmRound)hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
				speedloaderChamber.Unload();
			}
			else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject == null)
			{
				GameObject gameObject3 = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject();
				GameObject gameObject4 = Object.Instantiate(gameObject3, vector, rotation);
				FVRFireArmRound component2 = gameObject4.GetComponent<FVRFireArmRound>();
				component2.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
				component2.SetQuickBeltSlot(hand.CurrentHoveredQuickbeltSlotDirty);
				if (speedloaderChamber.IsSpent)
				{
					component2.Fire();
					component2.SetKillCounting(b: true);
				}
				speedloaderChamber.Unload();
			}
			else if (hand.CurrentHoveredQuickbeltSlotDirty != null && hand.CurrentHoveredQuickbeltSlotDirty.HeldObject is FVRFireArmRound && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).RoundType == speedloaderChamber.Type && ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).ProxyRounds.Count < ((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).MaxPalmedAmount)
			{
				FireArmRoundClass loadedClass2 = speedloaderChamber.LoadedClass;
				FVRObject roundSelfPrefab2 = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass);
				((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).AddProxy(loadedClass2, roundSelfPrefab2);
				((FVRFireArmRound)hand.CurrentHoveredQuickbeltSlotDirty.HeldObject).UpdateProxyDisplay();
				speedloaderChamber.Unload();
			}
			else
			{
				GameObject gameObject5 = AM.GetRoundSelfPrefab(speedloaderChamber.Type, speedloaderChamber.LoadedClass).GetGameObject();
				GameObject gameObject6 = Object.Instantiate(gameObject5, vector, rotation);
				FVRFireArmRound component3 = gameObject6.GetComponent<FVRFireArmRound>();
				gameObject6.GetComponent<FVRFireArmRound>().SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
				gameObject6.GetComponent<Rigidbody>().AddForce(gameObject6.transform.forward * 0.5f);
				if (speedloaderChamber.IsSpent)
				{
					component3.Fire();
					component3.SetKillCounting(b: true);
				}
				speedloaderChamber.Unload();
			}
		}

		public void OnTriggerEnter(Collider c)
		{
			if (base.QuickbeltSlot == null)
			{
				RevolverCylinder component = c.GetComponent<RevolverCylinder>();
				if (component != null && component.Revolver.RoundType == Chambers[0].Type)
				{
					HoveredCylinder = component;
				}
				RevolvingShotgunTrigger component2 = c.GetComponent<RevolvingShotgunTrigger>();
				if (component2 != null && component2.Shotgun.EjectDelay <= 0f && c.gameObject.CompareTag("FVRFireArmReloadTriggerWell") && component2.Shotgun.RoundType == Chambers[0].Type && SLType == component2.Shotgun.SLType)
				{
					HoveredRSTrigger = component2;
				}
			}
		}

		public void OnTriggerExit(Collider c)
		{
			RevolverCylinder component = c.GetComponent<RevolverCylinder>();
			if (component != null && HoveredCylinder == component)
			{
				HoveredCylinder = null;
			}
			RevolvingShotgunTrigger component2 = c.GetComponent<RevolvingShotgunTrigger>();
			if (component2 != null)
			{
				HoveredRSTrigger = null;
			}
		}
	}
}
