// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.LockToPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class LockToPoint : MonoBehaviour
  {
    public Transform snapTo;
    private Rigidbody body;
    public float snapTime = 2f;
    private float dropTimer;
    private Interactable interactable;

    private void Start()
    {
      this.interactable = this.GetComponent<Interactable>();
      this.body = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
      bool flag = false;
      if ((Object) this.interactable != (Object) null)
        flag = (bool) (Object) this.interactable.attachedToHand;
      if (flag)
      {
        this.body.isKinematic = false;
        this.dropTimer = -1f;
      }
      else
      {
        this.dropTimer += Time.deltaTime / (this.snapTime / 2f);
        this.body.isKinematic = (double) this.dropTimer > 1.0;
        if ((double) this.dropTimer > 1.0)
        {
          this.transform.position = this.snapTo.position;
          this.transform.rotation = this.snapTo.rotation;
        }
        else
        {
          float num = Mathf.Pow(35f, this.dropTimer);
          this.body.velocity = Vector3.Lerp(this.body.velocity, Vector3.zero, Time.fixedDeltaTime * 4f);
          if (this.body.useGravity)
            this.body.AddForce(-Physics.gravity);
          this.transform.position = Vector3.Lerp(this.transform.position, this.snapTo.position, (float) ((double) Time.fixedDeltaTime * (double) num * 3.0));
          this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.snapTo.rotation, (float) ((double) Time.fixedDeltaTime * (double) num * 2.0));
        }
      }
    }
  }
}
