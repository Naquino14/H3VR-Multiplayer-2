// Decompiled with JetBrains decompiler
// Type: RayTester
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class RayTester : MonoBehaviour
{
  private RaycastHit hit;
  public LayerMask LayerMask;
  public Texture2D MaskTexture;

  private void Start()
  {
  }

  private void Update()
  {
    if (!Physics.Raycast(this.transform.position, this.transform.forward, out this.hit, 10f, (int) this.LayerMask) || !(this.hit.collider.gameObject.tag == "MaskedTarget"))
      return;
    Color pixel = this.MaskTexture.GetPixel(Mathf.RoundToInt((float) this.MaskTexture.width * this.hit.textureCoord.x), Mathf.RoundToInt((float) this.MaskTexture.width * this.hit.textureCoord.y));
    Debug.Log((object) ("Color" + (object) pixel + " Points:" + (object) Mathf.RoundToInt(pixel.a * 10f)));
  }
}
