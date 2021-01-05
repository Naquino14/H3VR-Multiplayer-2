// Decompiled with JetBrains decompiler
// Type: FistVR.LugerToggleAction
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LugerToggleAction : MonoBehaviour
  {
    public HandgunSlide Slide;
    public Transform BarrelSlide;
    public Transform BarrelSlideForward;
    public Transform BarrelSlideLockPoint;
    public Transform TogglePiece1;
    public Transform TogglePiece2;
    public Transform TogglePiece3;
    public Vector2 RotSet1 = new Vector2(0.0f, -86f);
    public Vector2 RotSet2 = new Vector2(0.0f, 132.864f);
    public Vector2 PosSet1 = new Vector2(0.02199817f, -0.02124f);
    public float Height = 0.03527606f;

    private void Update()
    {
      float t = 1f - this.Slide.GetSlideLerpBetweenRearAndFore();
      this.BarrelSlide.localPosition = Vector3.Lerp(this.BarrelSlideForward.localPosition, this.BarrelSlideLockPoint.localPosition, t);
      float x1 = Mathf.Lerp(this.RotSet1.x, this.RotSet1.y, t);
      float x2 = Mathf.Lerp(this.RotSet2.x, this.RotSet2.y, t);
      float z = Mathf.Lerp(this.PosSet1.x, this.PosSet1.y, t);
      this.TogglePiece1.localEulerAngles = new Vector3(x1, 0.0f, 0.0f);
      this.TogglePiece2.localEulerAngles = new Vector3(x2, 0.0f, 0.0f);
      this.TogglePiece3.localPosition = new Vector3(0.0f, this.Height, z);
    }
  }
}
