// Decompiled with JetBrains decompiler
// Type: FistVR.FVRHealthBar
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRHealthBar : MonoBehaviour
  {
    public Text HealthReadOut;

    private void Update()
    {
      this.HealthReadOut.text = string.Empty + GM.CurrentPlayerBody.GetPlayerHealthRaw().ToString() + " / " + GM.CurrentPlayerBody.GetMaxHealthPlayerRaw().ToString();
      Vector3 vector3 = GM.CurrentPlayerBody.Head.position + Vector3.up * 0.4f;
      Vector3 forward1 = GM.CurrentPlayerBody.Head.forward;
      forward1.y = 0.0f;
      forward1.Normalize();
      Vector3 forward2 = forward1 * 0.25f;
      this.transform.position = vector3 + forward2;
      this.transform.rotation = Quaternion.LookRotation(forward2, Vector3.up);
    }
  }
}
