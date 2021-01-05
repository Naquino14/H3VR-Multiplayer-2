// Decompiled with JetBrains decompiler
// Type: BakeryLightMesh
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryLightMesh : MonoBehaviour
{
  public int UID;
  public static List<MeshFilter> All = new List<MeshFilter>();
  public Color color = Color.white;
  public float intensity = 1f;
  public Texture2D texture;
  public float cutoff = 100f;
  public int samples = 256;
  public int samples2 = 16;
  public int bitmask = 1;
  public bool selfShadow = true;
  public bool bakeToIndirect = true;
  public float indirectIntensity = 1f;
  public int lmid = -2;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    MeshRenderer component = this.gameObject.GetComponent<MeshRenderer>();
    if (!((Object) component != (Object) null))
      return;
    Gizmos.DrawWireSphere(component.bounds.center, this.cutoff);
  }
}
