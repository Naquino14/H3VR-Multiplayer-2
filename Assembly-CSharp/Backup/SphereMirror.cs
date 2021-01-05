// Decompiled with JetBrains decompiler
// Type: SphereMirror
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class SphereMirror : MonoBehaviour
{
  private void Start()
  {
    Vector2[] uv = this.transform.GetComponent<MeshFilter>().mesh.uv;
    for (int index = 0; index < uv.Length; ++index)
      uv[index] = new Vector2(1f - uv[index].x, uv[index].y);
    this.transform.GetComponent<MeshFilter>().mesh.uv = uv;
  }

  private void Update()
  {
  }
}
