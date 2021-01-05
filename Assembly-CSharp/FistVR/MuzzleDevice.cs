// Decompiled with JetBrains decompiler
// Type: FistVR.MuzzleDevice
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MuzzleDevice : FVRFireArmAttachment
  {
    public Transform Muzzle;
    [Header("MuzzleEffects")]
    public bool ForcesEffectSize;
    public FVRFireArmMechanicalAccuracyClass MechanicalAccuracy;
    public MuzzleEffect[] MuzzleEffects;
    private float m_mechanicalAccuracy;
    private float m_dropmult;
    private float m_driftmult;

    public float GetMechanicalAccuracy() => this.m_mechanicalAccuracy;

    public float GetDropMult(FVRFireArm f) => this.m_dropmult;

    public Vector2 GetDriftMult(FVRFireArm f)
    {
      string str = "empty";
      if ((Object) f.ObjectWrapper != (Object) null)
        str = f.ObjectWrapper.ItemID;
      return new Vector2(this.ObjectIDsToFloatHash(this.ObjectWrapper.ItemID, str), this.ObjectIDsToFloatHash(str, this.ObjectWrapper.ItemID)) * this.m_driftmult;
    }

    protected override void Awake()
    {
      base.Awake();
      this.m_mechanicalAccuracy = AM.GetFireArmMechanicalSpread(this.MechanicalAccuracy);
      this.m_dropmult = AM.GetDropMult(this.MechanicalAccuracy);
      this.m_driftmult = AM.GetDriftMult(this.MechanicalAccuracy);
    }

    public virtual void OnShot(FVRFireArm f, FVRTailSoundClass tailClass)
    {
    }

    public float ObjectIDsToFloatHash(string objectID0, string objectID1)
    {
      int num = ((17 * 31 + objectID0.GetDeterministicHashCode()) * 31 + objectID1.GetDeterministicHashCode()) % 10000;
      return (float) ((num >= 0 ? (double) num : (double) (num + 10000)) / 5000.0 - 1.0);
    }
  }
}
