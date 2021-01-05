// Decompiled with JetBrains decompiler
// Type: FistVR.wwHorseShoe
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwHorseShoe : FVRPhysicalObject
  {
    private bool m_isShattered;
    public wwHorseShoePlinth Plinth;
    public GameObject ShatterFX;
    public GameObject SuccessParticles;
    public GameObject SuccessParticles2;

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if ((Object) col.collider.attachedRigidbody != (Object) null && col.collider.attachedRigidbody.gameObject.CompareTag("HorseshoePost"))
      {
        wwHorseShoePost component = col.collider.attachedRigidbody.gameObject.GetComponent<wwHorseShoePost>();
        if ((Object) component != (Object) null && component.PostIndex == this.Plinth.PlinthIndex)
        {
          this.Plinth.HitSuccess();
          Object.Instantiate<GameObject>(this.SuccessParticles, this.Plinth.transform.position, this.Plinth.transform.rotation);
          Object.Instantiate<GameObject>(this.SuccessParticles2, col.collider.attachedRigidbody.gameObject.transform.position, col.collider.attachedRigidbody.gameObject.transform.rotation);
        }
      }
      this.Shatter();
    }

    private void Shatter()
    {
      if (this.m_isShattered)
        return;
      this.m_isShattered = true;
      if ((Object) this.m_hand != (Object) null)
      {
        FVRViveHand hand = this.m_hand;
        this.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
        this.EndInteraction(hand);
      }
      Object.Instantiate<GameObject>(this.ShatterFX, this.transform.position, this.transform.rotation);
      this.Plinth.NeedNewHorseshoe();
      Object.Destroy((Object) this.gameObject);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.transform.position.y < -100.0)
        this.Shatter();
      if (!this.IsHeld && !((Object) this.QuickbeltSlot != (Object) null))
        return;
      this.DistanceCheck();
    }

    private void DistanceCheck()
    {
      if ((double) Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(this.Plinth.transform.position.x, this.Plinth.transform.position.z)) <= 2.0)
        return;
      this.Shatter();
    }
  }
}
