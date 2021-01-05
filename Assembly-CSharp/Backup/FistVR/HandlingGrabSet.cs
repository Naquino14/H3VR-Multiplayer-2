// Decompiled with JetBrains decompiler
// Type: FistVR.HandlingGrabSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/HandlingGrabSet", order = 0)]
  public class HandlingGrabSet : ScriptableObject
  {
    public HandlingGrabType Type;
    public AudioEvent GrabSet_Light;
    public AudioEvent GrabSet_Hard;
  }
}
