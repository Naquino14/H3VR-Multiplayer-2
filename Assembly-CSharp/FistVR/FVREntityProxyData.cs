// Decompiled with JetBrains decompiler
// Type: FistVR.FVREntityProxyData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class FVREntityProxyData
  {
    public string UniqueID = "unassigned";
    public string EntityID = "unassigned";
    public Vector3 Position = Vector3.zero;
    public Vector3 EulerAngles = Vector3.zero;
    public bool[] StoredBools;
    public int[] StoredInts;
    public float[] StoredFloats;
    public Vector4[] StoredVector4s;
    public string[] StoredStrings;

    public void PrimeDataLists(FVREntityFlagUsage flags)
    {
      this.StoredBools = new bool[flags.BoolFlags.Length];
      this.StoredInts = new int[flags.IntFlags.Length];
      this.StoredFloats = new float[flags.FloatFlags.Length];
      this.StoredVector4s = new Vector4[flags.Vector4Flags.Length];
      this.StoredStrings = new string[flags.StringFlags.Length];
    }

    public void Init(FVREntityFlagUsage flags)
    {
      for (int index = 0; index < this.StoredBools.Length; ++index)
        this.StoredBools[index] = flags.BoolFlags[index].DefaultValue;
      for (int index = 0; index < this.StoredInts.Length; ++index)
        this.StoredInts[index] = flags.IntFlags[index].DefaultValue;
      for (int index = 0; index < this.StoredFloats.Length; ++index)
        this.StoredFloats[index] = flags.FloatFlags[index].DefaultValue;
      for (int index = 0; index < this.StoredVector4s.Length; ++index)
        this.StoredVector4s[index] = flags.Vector4Flags[index].DefaultValue;
      for (int index = 0; index < this.StoredStrings.Length; ++index)
        this.StoredStrings[index] = flags.StringFlags[index].DefaultValue;
    }
  }
}
