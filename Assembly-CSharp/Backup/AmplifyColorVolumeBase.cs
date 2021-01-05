// Decompiled with JetBrains decompiler
// Type: AmplifyColorVolumeBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using AmplifyColor;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
  public Texture2D LutTexture;
  public float Exposure = 1f;
  public float EnterBlendTime = 1f;
  public int Priority;
  public bool ShowInSceneView = true;
  [HideInInspector]
  public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();

  private void OnDrawGizmos()
  {
    if (!this.ShowInSceneView)
      return;
    BoxCollider component1 = this.GetComponent<BoxCollider>();
    BoxCollider2D component2 = this.GetComponent<BoxCollider2D>();
    if (!((Object) component1 != (Object) null) && !((Object) component2 != (Object) null))
      return;
    Vector3 center;
    Vector3 size;
    if ((Object) component1 != (Object) null)
    {
      center = component1.center;
      size = component1.size;
    }
    else
    {
      center = (Vector3) component2.offset;
      size = (Vector3) component2.size;
    }
    Gizmos.color = Color.green;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.DrawWireCube(center, size);
  }

  private void OnDrawGizmosSelected()
  {
    BoxCollider component1 = this.GetComponent<BoxCollider>();
    BoxCollider2D component2 = this.GetComponent<BoxCollider2D>();
    if (!((Object) component1 != (Object) null) && !((Object) component2 != (Object) null))
      return;
    Color green = Color.green;
    green.a = 0.2f;
    Gizmos.color = green;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Vector3 center;
    Vector3 size;
    if ((Object) component1 != (Object) null)
    {
      center = component1.center;
      size = component1.size;
    }
    else
    {
      center = (Vector3) component2.offset;
      size = (Vector3) component2.size;
    }
    Gizmos.DrawCube(center, size);
  }
}
