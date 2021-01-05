// Decompiled with JetBrains decompiler
// Type: CFX_Demo_RandomDir
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CFX_Demo_RandomDir : MonoBehaviour
{
  public Vector3 min = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 max = new Vector3(0.0f, 360f, 0.0f);

  private void Awake() => this.transform.eulerAngles = new Vector3(Random.Range(this.min.x, this.max.x), Random.Range(this.min.y, this.max.y), Random.Range(this.min.z, this.max.z));
}
