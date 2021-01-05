// Decompiled with JetBrains decompiler
// Type: FistVR.SavedGunComponent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class SavedGunComponent
  {
    public int Index = -1;
    public string ObjectID = string.Empty;
    public Vector3 PosOffset = Vector3.zero;
    public Vector3 OrientationForward = Vector3.zero;
    public Vector3 OrientationUp = Vector3.zero;
    public int ObjectAttachedTo = -1;
    public int MountAttachedTo = -1;
    public bool isFirearm;
    public bool isMagazine;
    public bool isAttachment;
    public Dictionary<string, string> Flags = new Dictionary<string, string>();

    public void DebugPrintData()
    {
    }
  }
}
