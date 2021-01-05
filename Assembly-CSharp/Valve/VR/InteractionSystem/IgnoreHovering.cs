// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.IgnoreHovering
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class IgnoreHovering : MonoBehaviour
  {
    [Tooltip("If Hand is not null, only ignore the specified hand")]
    public Hand onlyIgnoreHand;
  }
}
