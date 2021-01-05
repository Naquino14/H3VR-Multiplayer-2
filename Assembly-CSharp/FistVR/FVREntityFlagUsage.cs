// Decompiled with JetBrains decompiler
// Type: FistVR.FVREntityFlagUsage
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "Entities/EntityFlagUsage", order = 0)]
  public class FVREntityFlagUsage : ScriptableObject
  {
    public bool IsSerialized = true;
    [Header("Bool Params")]
    public FVREntityFlagUsage.StoredBoolFlag[] BoolFlags;
    [Header("Int Params")]
    public FVREntityFlagUsage.StoredIntFlag[] IntFlags;
    [Header("Float Params")]
    public FVREntityFlagUsage.StoredFloatFlag[] FloatFlags;
    [Header("Vector4 Params")]
    public FVREntityFlagUsage.StoredVector4Flag[] Vector4Flags;
    [Header("String Params")]
    public FVREntityFlagUsage.StoredString[] StringFlags;

    [Serializable]
    public class StoredBoolFlag
    {
      public string Name;
      public bool ExposedForUserEdit;
      public bool DefaultValue;
    }

    [Serializable]
    public class StoredIntFlag
    {
      public string Name;
      public bool ExposedForUserEdit;
      public int MinValue;
      public int MaxValue = 1;
      public int DefaultValue;
    }

    [Serializable]
    public class StoredFloatFlag
    {
      public string Name;
      public bool ExposedForUserEdit;
      public float MinValue;
      public float MaxValue = 1f;
      public float DefaultValue;
    }

    [Serializable]
    public class StoredVector4Flag
    {
      public string Name;
      public bool ExposedForUserEdit;
      public Vector4 MinValues = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
      public Vector4 MaxValues = new Vector4(1f, 1f, 1f, 1f);
      public FVREntityFlagUsage.EntityVectorFlagUsage Usage;
      public Vector4 DefaultValue;
    }

    [Serializable]
    public class StoredString
    {
      public string Name;
      public bool ExposedForUserEdit;
      public int MaxLength = 30;
      public string DefaultValue;
    }

    public enum EntityVectorFlagUsage
    {
      Default,
      Color,
    }
  }
}
