// Decompiled with JetBrains decompiler
// Type: FistVR.Winchester1873LoadingGate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Winchester1873LoadingGate : MonoBehaviour
  {
    public Transform LoadingGateObject;
    public Vector2 LoadingGateRotRange;
    private float curRot;
    private float tarRot;
    public float Range = 0.02f;

    private void Update()
    {
      bool flag = false;
      float num1 = 1f;
      if (!((Object) GM.CurrentMovementManager != (Object) null))
        return;
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index] != (Object) null && (Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmRound)
        {
          float num2 = Vector3.Distance(GM.CurrentMovementManager.Hands[index].CurrentInteractable.transform.position, this.transform.position);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            flag = true;
          }
        }
      }
      this.tarRot = !flag ? 0.0f : ((double) num1 > (double) this.Range ? 0.0f : (this.Range - num1) / this.Range);
      if ((double) this.tarRot == (double) this.curRot)
        return;
      this.curRot = this.tarRot;
      this.LoadingGateObject.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(this.LoadingGateRotRange.x, this.LoadingGateRotRange.y, this.curRot * 2f));
    }
  }
}
