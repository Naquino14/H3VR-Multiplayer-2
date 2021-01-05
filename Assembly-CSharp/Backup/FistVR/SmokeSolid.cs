// Decompiled with JetBrains decompiler
// Type: FistVR.SmokeSolid
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SmokeSolid : MonoBehaviour, IFVRDamageable
  {
    public Rigidbody RB;
    public AnimationCurve ScaleOverLife;
    public AnimationCurve AngularDragCurve;
    public float ScaleMult = 1f;
    public float Life;
    public float DecaySpeed = 0.1f;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
      this.Life += this.DecaySpeed * Time.deltaTime;
      this.RB.angularDrag = this.AngularDragCurve.Evaluate(this.Life);
      float num = this.ScaleOverLife.Evaluate(this.Life) * this.ScaleMult;
      if ((double) this.Life >= 1.0)
        Object.Destroy((Object) this.gameObject);
      else
        this.transform.localScale = new Vector3(num, num, num);
    }

    public void Damage(FistVR.Damage d) => this.DecaySpeed += this.DecaySpeed * 0.25f;
  }
}
