// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.WheelDust
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class WheelDust : MonoBehaviour
  {
    private WheelCollider col;
    public ParticleSystem p;
    public float EmissionMul;
    public float velocityMul = 2f;
    public float maxEmission;
    public float minSlip;
    [HideInInspector]
    public float amt;
    [HideInInspector]
    public Vector3 slip;
    private float emitTimer;

    private void Start()
    {
      this.col = this.GetComponent<WheelCollider>();
      this.StartCoroutine(this.emitter());
    }

    private void Update()
    {
      this.slip = Vector3.zero;
      if (this.col.isGrounded)
      {
        WheelHit hit;
        this.col.GetGroundHit(out hit);
        this.slip += Vector3.right * hit.sidewaysSlip;
        this.slip += Vector3.forward * -hit.forwardSlip;
      }
      this.amt = this.slip.magnitude;
    }

    [DebuggerHidden]
    private IEnumerator emitter() => (IEnumerator) new WheelDust.\u003Cemitter\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void DoEmit()
    {
      this.p.transform.rotation = Quaternion.LookRotation(this.transform.TransformDirection(this.slip));
      this.p.startSpeed = this.velocityMul * this.amt;
      this.p.Emit(1);
    }
  }
}
