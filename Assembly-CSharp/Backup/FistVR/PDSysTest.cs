// Decompiled with JetBrains decompiler
// Type: FistVR.PDSysTest
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PDSysTest : MonoBehaviour
  {
    public Transform PointTest;
    public Texture2D SysTex;

    private void Start()
    {
    }

    private void Update()
    {
      this.PointTest.localPosition = this.PointTest.localPosition.normalized * 0.5f;
      Vector2 vector2 = WhunkSphereMapping.PositionToUVSpherical(this.PointTest.localPosition, Vector3.right, (Vector3.up - Vector3.forward).normalized);
      vector2 = new Vector2(vector2.y, vector2.x);
      Color pixel = this.SysTex.GetPixel(Mathf.FloorToInt(vector2.x * (float) this.SysTex.width), Mathf.FloorToInt(vector2.y * (float) this.SysTex.height));
      int num1 = Mathf.FloorToInt(pixel.r * (float) byte.MaxValue);
      int num2 = Mathf.FloorToInt(pixel.g * (float) byte.MaxValue);
      int num3 = Mathf.FloorToInt(pixel.b * (float) byte.MaxValue);
      Debug.Log((object) (vector2.x.ToString() + " " + (object) vector2.y + " " + (object) num1 + " " + (object) num2 + " " + (object) num3));
    }
  }
}
