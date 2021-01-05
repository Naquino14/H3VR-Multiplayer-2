// Decompiled with JetBrains decompiler
// Type: ProbeGizmo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class ProbeGizmo : MonoBehaviour
{
  public ReflectionProbe probe;
  public bool Draw;

  private void OnDrawGizmos()
  {
    if (!this.Draw)
      return;
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(this.transform.position + this.probe.center, new Vector3(this.probe.size.x, this.probe.size.y, this.probe.size.z));
  }

  [ContextMenu("attach")]
  public void attach() => this.probe = this.GetComponent<ReflectionProbe>();
}
