// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPokeyButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRPokeyButton : FVRInteractiveObject
  {
    public Vector3 OutPosition;
    public Vector3 InPosition;
    public Transform ButtonObject;
    private float m_buttonPos;
    private AudioSource aud;
    public GameObject PokeyTarget;
    public string MessageName;
    public int NumValue;
    public AudioClip Yay;
    public AudioClip NotYay;
    public float VolMod = 1f;
    private bool m_hasAud;

    protected override void Awake()
    {
      base.Awake();
      this.aud = this.GetComponent<AudioSource>();
      if (!((Object) this.aud != (Object) null))
        return;
      this.m_hasAud = true;
    }

    public override bool IsInteractable() => false;

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      hand.Buzz(hand.Buzzer.Buzz_GunShot);
      if ((double) this.m_buttonPos >= 0.5)
        return;
      this.m_buttonPos = 1f;
      if (this.m_hasAud)
        this.aud.pitch = Random.Range(0.85f, 1.15f);
      if ((Object) this.PokeyTarget != (Object) null)
      {
        this.PokeyTarget.SendMessage(this.MessageName, (object) this.NumValue);
        if (!this.m_hasAud)
          return;
        this.aud.PlayOneShot(this.Yay, this.VolMod);
      }
      else
      {
        if (!this.m_hasAud)
          return;
        this.aud.PlayOneShot(this.NotYay, this.VolMod);
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_buttonPos <= 0.0)
        return;
      this.m_buttonPos = Mathf.MoveTowards(this.m_buttonPos, 0.0f, 3f);
      if (!((Object) this.ButtonObject != (Object) null))
        return;
      this.ButtonObject.localPosition = Vector3.Lerp(this.OutPosition, this.InPosition, this.m_buttonPos);
    }
  }
}
