// Decompiled with JetBrains decompiler
// Type: TreeEditScript
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class TreeEditScript : MonoBehaviour
{
  public Vector2 SizeY;
  public Vector2 SizeXZ;

  private void Update() => this.Scaler();

  private void Scaler()
  {
    float num = Random.Range(this.SizeXZ.x, this.SizeXZ.y);
    float y1 = Random.Range(this.SizeY.x, this.SizeY.y);
    float y2 = Random.Range(0.0f, 360f);
    this.transform.localScale = new Vector3(num, y1, num);
    this.transform.localEulerAngles = new Vector3(0.0f, y2, 0.0f);
  }
}
