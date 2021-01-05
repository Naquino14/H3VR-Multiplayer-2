// Decompiled with JetBrains decompiler
// Type: FistVR.AIDamagePlate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIDamagePlate : MonoBehaviour, IFVRDamageable
  {
    public float XRotUp;
    public float XRotDown;
    private AudioSource m_aud;
    public bool IsDown;

    private void Start() => this.m_aud = this.GetComponent<AudioSource>();

    public virtual void Damage(Vector3 damagePoint, Vector3 damageDir, Vector2 damageUVCoord)
    {
      if (this.IsDown)
        return;
      this.IsDown = true;
      this.m_aud.PlayOneShot(this.m_aud.clip, 1f);
      this.transform.localEulerAngles = new Vector3(this.XRotDown, 0.0f, 0.0f);
    }

    public virtual void Damage(FistVR.Damage dam)
    {
      if (this.IsDown)
        return;
      this.IsDown = true;
      this.m_aud.PlayOneShot(this.m_aud.clip, 1f);
      this.transform.localEulerAngles = new Vector3(this.XRotDown, 0.0f, 0.0f);
    }

    public virtual void Reset()
    {
      this.IsDown = false;
      this.transform.localEulerAngles = new Vector3(this.XRotUp, 0.0f, 0.0f);
    }
  }
}
