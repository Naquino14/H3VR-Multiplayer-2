// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMeleeWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRMeleeWeapon : FVRPhysicalObject
  {
    [ContextMenu("MigrateToAttachable")]
    public void MigrateToAttachable()
    {
      AttachableMeleeWeapon component = this.GetComponent<AttachableMeleeWeapon>();
      component.MP.HandPoint = this.MP.HandPoint;
      component.MP.EndPoint = this.MP.EndPoint;
      component.MP.BaseDamageBCP = this.MP.BaseDamageBCP;
      component.MP.HighDamageBCP = this.MP.HighDamageBCP;
      component.MP.StabDamageBCP = this.MP.StabDamageBCP;
      component.MP.TearDamageBCP = this.MP.TearDamageBCP;
      component.MP.HighDamageColliders = this.MP.HighDamageColliders;
      component.MP.HighDamageVectors = this.MP.HighDamageVectors;
      component.MP.DoesCyclePosePoints = this.MP.DoesCyclePosePoints;
      component.MP.PosePoints = this.MP.PosePoints;
      component.MP.IsThrownDisposable = this.MP.IsThrownDisposable;
      component.MP.m_isThrownAutoAim = this.MP.m_isThrownAutoAim;
      component.MP.ThrownDetectMask = this.MP.ThrownDetectMask;
      component.MP.StartThrownDisposalTickdownOnSpawn = this.MP.StartThrownDisposalTickdownOnSpawn;
      component.MP.IsLongThrowable = this.MP.IsLongThrowable;
      component.MP.IsThrowableDirInverted = this.MP.IsThrowableDirInverted;
      component.MP.CanNewStab = this.MP.CanNewStab;
      component.MP.BladeLength = this.MP.BladeLength;
      component.MP.MassWhileStabbed = this.MP.MassWhileStabbed;
      component.MP.StabDirection = this.MP.StabDirection;
      component.MP.StabAngularThreshold = this.MP.StabAngularThreshold;
      component.MP.StabColliders = this.MP.StabColliders;
      component.MP.StabVelocityRequirement = this.MP.StabVelocityRequirement;
      component.MP.CanTearOut = this.MP.CanTearOut;
      component.MP.TearOutVelThreshold = this.MP.TearOutVelThreshold;
      component.MP.CanNewLodge = this.MP.CanNewLodge;
      component.MP.LodgeDepth = this.MP.LodgeDepth;
      component.MP.MassWhileLodged = this.MP.MassWhileLodged;
      component.MP.LodgeDirections = this.MP.LodgeDirections;
      component.MP.LodgeColliders = this.MP.LodgeColliders;
      component.MP.LodgeVelocityRequirement = this.MP.LodgeVelocityRequirement;
      component.MP.DeLodgeVelocityRequirement = this.MP.DeLodgeVelocityRequirement;
      component.MP.UsesSweepTesting = this.MP.UsesSweepTesting;
      component.MP.UsesSweepDebug = this.MP.UsesSweepDebug;
      component.MP.TestCols = this.MP.TestCols;
      component.MP.SweepTransformStart = this.MP.SweepTransformStart;
      component.MP.SweepTransformEnd = this.MP.SweepTransformEnd;
      component.MP.LM_DamageTest = this.MP.LM_DamageTest;
    }
  }
}
