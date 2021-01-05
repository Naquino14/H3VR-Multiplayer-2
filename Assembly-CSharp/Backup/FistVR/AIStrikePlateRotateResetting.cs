// Decompiled with JetBrains decompiler
// Type: FistVR.AIStrikePlateRotateResetting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIStrikePlateRotateResetting : AIStrikePlate
  {
    private bool isFlippedUp;
    public float XRotUp;
    public float XRotDown;
    private float curXRot;
    private float tarXRot;
    public GameObject FlipUpTarget;
    public string FlipUpMessage;
    public GameObject FlipDownTarget;
    public string FlipDownMessage;

    public override void Damage(FistVR.Damage dam) => base.Damage(dam);

    public override void Reset()
    {
      this.isFlippedUp = false;
      this.tarXRot = this.XRotDown;
      this.FlipDownTarget.SendMessage(this.FlipDownMessage);
      base.Reset();
    }

    public void Update()
    {
      this.curXRot = Mathf.Lerp(this.curXRot, this.tarXRot, 15f);
      this.transform.localEulerAngles = new Vector3(this.curXRot, 0.0f, 0.0f);
    }

    public override void PlateFelled()
    {
      this.isFlippedUp = true;
      this.tarXRot = this.XRotUp;
      this.FlipUpTarget.SendMessage(this.FlipUpMessage);
      this.CancelInvoke();
      this.Invoke("Reset", 10f);
    }
  }
}
