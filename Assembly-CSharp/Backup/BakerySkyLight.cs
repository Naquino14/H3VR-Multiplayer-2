// Decompiled with JetBrains decompiler
// Type: BakerySkyLight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakerySkyLight : MonoBehaviour
{
  public string texName = "sky.dds";
  public Color color = Color.white;
  public float intensity = 1f;
  public int samples = 32;
  public bool hemispherical;
  public int bitmask = 1;
  public bool bakeToIndirect = true;
  public float indirectIntensity = 1f;
  public bool tangentSH;
  public Cubemap cubemap;
  public int UID;
}
