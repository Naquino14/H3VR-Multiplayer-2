// Decompiled with JetBrains decompiler
// Type: BakeryPointLight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryPointLight : MonoBehaviour
{
  public int UID;
  public Color color = Color.white;
  public float intensity = 1f;
  public float shadowSpread = 0.05f;
  public float cutoff = 10f;
  public bool realisticFalloff;
  public int samples = 8;
  public BakeryPointLight.ftLightProjectionMode projMode;
  public Texture2D cookie;
  public float angle = 30f;
  public float innerAngle;
  public Cubemap cubemap;
  public Object iesFile;
  public int bitmask = 1;
  public bool bakeToIndirect;
  public bool shadowmask;
  public float indirectIntensity = 1f;
  public float falloffMinRadius = 1f;
  private const float GIZMO_MAXSIZE = 0.1f;
  private const float GIZMO_SCALE = 0.01f;
  private float screenRadius = 0.1f;

  public enum ftLightProjectionMode
  {
    Omni,
    Cookie,
    Cubemap,
    IES,
    Cone,
  }
}
