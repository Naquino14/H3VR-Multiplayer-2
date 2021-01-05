// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBullshotCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRBullshotCamera : MonoBehaviour
  {
    public Transform CameraTarget;
    private Vector3 filteredPos;
    private Quaternion filteredRot;

    private void Update()
    {
      if (!((Object) this.CameraTarget != (Object) null))
        return;
      float num = Mathf.Min(Vector3.Distance(this.filteredPos, this.CameraTarget.position) / 0.015f, 1f);
      this.filteredPos = (1f - num) * this.filteredPos + num * this.CameraTarget.position;
      this.filteredRot = Quaternion.Slerp(this.filteredRot, this.CameraTarget.rotation, Mathf.Min(Quaternion.Angle(this.filteredRot, this.CameraTarget.rotation) / 5f, 1f));
      this.transform.position = Vector3.Lerp(this.transform.position, this.filteredPos, Time.deltaTime * 3f);
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.filteredRot, Time.deltaTime * 3f);
    }
  }
}
