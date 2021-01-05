using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Config Template", menuName = "Sosig/ConfigTemplate", order = 0)]
	public class SosigConfigTemplate : ScriptableObject
	{
		[Header("AIEntityParams")]
		public float ViewDistance = 250f;

		public float HearingDistance = 300f;

		public float MaxFOV = 105f;

		public bool DoesAggroOnFriendlyFire;

		public bool HasABrain = true;

		public float SearchExtentsModifier = 1f;

		public bool DoesDropWeaponsOnBallistic = true;

		public bool CanPickup_Ranged = true;

		public bool CanPickup_Melee = true;

		public bool CanPickup_Other = true;

		[Header("TargetPrioritySystemParams")]
		public int TargetCapacity = 5;

		public float TargetTrackingTime = 2f;

		public float NoFreshTargetTime = 1.5f;

		public float AssaultPointOverridesSkirmishPointWhenFurtherThan = 200f;

		[Header("Movement Params")]
		public float RunSpeed = 3.5f;

		public float WalkSpeed = 1.4f;

		public float SneakSpeed = 0.6f;

		public float CrawlSpeed = 0.3f;

		public float TurnSpeed = 2f;

		public float MaxJointLimit = 6f;

		public float MovementRotMagnitude = 10f;

		[Header("Damage Params")]
		public bool AppliesDamageResistToIntegrityLoss;

		public float TotalMustard = 100f;

		public float BleedDamageMult = 0.5f;

		public float BleedRateMultiplier = 1f;

		public float BleedVFXIntensity = 0.2f;

		public float DamMult_Projectile = 1f;

		public float DamMult_Explosive = 1f;

		public float DamMult_Melee = 1f;

		public float DamMult_Piercing = 1f;

		public float DamMult_Blunt = 1f;

		public float DamMult_Cutting = 1f;

		public float DamMult_Thermal = 1f;

		public float DamMult_Chilling = 1f;

		public float DamMult_EMP = 1f;

		public List<float> LinkDamageMultipliers;

		public List<float> LinkStaggerMultipliers;

		public List<Vector2> StartingLinkIntegrity;

		public List<float> StartingChanceBrokenJoint;

		[Header("Shudder Params")]
		public float ShudderThreshold = 2f;

		[Header("Confusion Params")]
		public float ConfusionThreshold = 0.3f;

		public float ConfusionMultiplier = 6f;

		public float ConfusionTimeMax = 4f;

		[Header("Stun Params")]
		public float StunThreshold = 1.4f;

		public float StunMultiplier = 2f;

		public float StunTimeMax = 4f;

		[Header("Resistances")]
		public bool CanBeGrabbed = true;

		public bool CanBeSevered = true;

		public bool CanBeStabbed = true;

		[Header("Suppression")]
		public bool CanBeSurpressed = true;

		public float SuppressionMult = 1f;

		[Header("Death Flags")]
		public bool DoesJointBreakKill_Head = true;

		public bool DoesJointBreakKill_Upper;

		public bool DoesJointBreakKill_Lower;

		public bool DoesSeverKill_Head = true;

		public bool DoesSeverKill_Upper = true;

		public bool DoesSeverKill_Lower = true;

		public bool DoesExplodeKill_Head = true;

		public bool DoesExplodeKill_Upper = true;

		public bool DoesExplodeKill_Lower = true;

		[Header("SpawnOnLinkDestroy")]
		public bool UsesLinkSpawns;

		public List<FVRObject> LinkSpawns;

		public List<float> LinkSpawnChance;

		[Header("SpeechOverride")]
		public bool OverrideSpeech;

		public SosigSpeechSet OverrideSpeechSet;
	}
}
