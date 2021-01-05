// Decompiled with JetBrains decompiler
// Type: FistVR.DamageDealt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public struct DamageDealt
  {
    public Vector3 force;
    public float MPa;
    public float MPaRootMeter;
    public float PointsDamage;
    public float StunDamage;
    public Vector3 point;
    public Vector3 hitNormal;
    public Vector3 strikeDir;
    public Vector2 uvCoords;
    public bool IsInitialContact;
    public bool IsInside;
    public bool IsMelee;
    public bool DoesIgnite;
    public bool DoesFreeze;
    public bool DoesDisrupt;
    public bool IsPlayer;
    public DamageDealt.DamageType Type;
    public Transform ShotOrigin;
    public FVRFireArm SourceFirearm;

    public enum DamageType
    {
      None,
      Pistol,
      Shotgun,
      SMGRifle,
      Support,
      Explosive,
      Melee,
      Trap,
    }
  }
}
