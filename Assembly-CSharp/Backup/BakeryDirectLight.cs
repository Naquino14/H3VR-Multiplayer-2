// Decompiled with JetBrains decompiler
// Type: BakeryDirectLight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryDirectLight : MonoBehaviour
{
  public Color color = Color.white;
  public float intensity = 1f;
  public float shadowSpread = 0.01f;
  public int samples = 16;
  public int bitmask = 1;
  public bool bakeToIndirect;
  public bool shadowmask;
  public bool shadowmaskDenoise;
  public float indirectIntensity = 1f;
  public int UID;
}
