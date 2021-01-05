using UnityEngine;

namespace FistVR
{
	public class FVRMeleeWeapon : FVRPhysicalObject
	{
		[ContextMenu("MigrateToAttachable")]
		public void MigrateToAttachable()
		{
			AttachableMeleeWeapon component = GetComponent<AttachableMeleeWeapon>();
			component.MP.HandPoint = MP.HandPoint;
			component.MP.EndPoint = MP.EndPoint;
			component.MP.BaseDamageBCP = MP.BaseDamageBCP;
			component.MP.HighDamageBCP = MP.HighDamageBCP;
			component.MP.StabDamageBCP = MP.StabDamageBCP;
			component.MP.TearDamageBCP = MP.TearDamageBCP;
			component.MP.HighDamageColliders = MP.HighDamageColliders;
			component.MP.HighDamageVectors = MP.HighDamageVectors;
			component.MP.DoesCyclePosePoints = MP.DoesCyclePosePoints;
			component.MP.PosePoints = MP.PosePoints;
			component.MP.IsThrownDisposable = MP.IsThrownDisposable;
			component.MP.m_isThrownAutoAim = MP.m_isThrownAutoAim;
			component.MP.ThrownDetectMask = MP.ThrownDetectMask;
			component.MP.StartThrownDisposalTickdownOnSpawn = MP.StartThrownDisposalTickdownOnSpawn;
			component.MP.IsLongThrowable = MP.IsLongThrowable;
			component.MP.IsThrowableDirInverted = MP.IsThrowableDirInverted;
			component.MP.CanNewStab = MP.CanNewStab;
			component.MP.BladeLength = MP.BladeLength;
			component.MP.MassWhileStabbed = MP.MassWhileStabbed;
			component.MP.StabDirection = MP.StabDirection;
			component.MP.StabAngularThreshold = MP.StabAngularThreshold;
			component.MP.StabColliders = MP.StabColliders;
			component.MP.StabVelocityRequirement = MP.StabVelocityRequirement;
			component.MP.CanTearOut = MP.CanTearOut;
			component.MP.TearOutVelThreshold = MP.TearOutVelThreshold;
			component.MP.CanNewLodge = MP.CanNewLodge;
			component.MP.LodgeDepth = MP.LodgeDepth;
			component.MP.MassWhileLodged = MP.MassWhileLodged;
			component.MP.LodgeDirections = MP.LodgeDirections;
			component.MP.LodgeColliders = MP.LodgeColliders;
			component.MP.LodgeVelocityRequirement = MP.LodgeVelocityRequirement;
			component.MP.DeLodgeVelocityRequirement = MP.DeLodgeVelocityRequirement;
			component.MP.UsesSweepTesting = MP.UsesSweepTesting;
			component.MP.UsesSweepDebug = MP.UsesSweepDebug;
			component.MP.TestCols = MP.TestCols;
			component.MP.SweepTransformStart = MP.SweepTransformStart;
			component.MP.SweepTransformEnd = MP.SweepTransformEnd;
			component.MP.LM_DamageTest = MP.LM_DamageTest;
		}
	}
}
