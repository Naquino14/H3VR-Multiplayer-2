// Decompiled with JetBrains decompiler
// Type: FistVR.GPSavedPlaceable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class GPSavedPlaceable
  {
    public string ObjectID;
    public int UniqueID;
    public Vector3 Position;
    public Quaternion Rotation;
    public List<string> InternalList;
    public SerializableStringDictionary Flags;
  }
}
