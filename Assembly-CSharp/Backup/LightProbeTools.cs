﻿// Decompiled with JetBrains decompiler
// Type: LightProbeTools
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class LightProbeTools : MonoBehaviour
{
  public LayerMask projectMask = (LayerMask) -1;
  public float projectOffset = 1f;
  public float projectMaxDistance = 100000f;

  [ContextMenu("Project Down")]
  public void ProjectDown()
  {
  }
}