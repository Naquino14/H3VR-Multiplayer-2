// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpinJack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TR_SpinJack : MonoBehaviour
  {
    private bool m_isDescending;
    private bool m_isSpinning;
    private bool m_isAscending;
    private float Height = 20f;
    public MR_DamageOnEnter[] DamageNodes;
    private Vector3 startPos = Vector3.zero;
    public AudioSource Aud;
    public List<GameObject> Meatballs;
    private float Yrot;
    private float SpinSpeed;

    private void Start()
    {
      this.m_isDescending = true;
      this.startPos = this.transform.position;
      this.Yrot = this.transform.eulerAngles.y;
    }

    private void ActivateDamage()
    {
      for (int index = 0; index < this.DamageNodes.Length; ++index)
        this.DamageNodes[index].enabled = true;
    }

    private void DeactivateDamage()
    {
      for (int index = 0; index < this.DamageNodes.Length; ++index)
        this.DamageNodes[index].enabled = false;
    }

    private void Update()
    {
      if (this.m_isDescending)
      {
        this.Height -= Time.deltaTime * 2.5f;
        if ((double) this.Height <= 0.0)
        {
          this.Height = 0.0f;
          this.m_isDescending = false;
          this.m_isSpinning = true;
          this.Aud.volume = 0.1f;
          this.Aud.Play();
        }
        this.transform.position = new Vector3(this.startPos.x, this.Height, this.startPos.z);
      }
      if (this.m_isSpinning)
      {
        this.SpinSpeed = Mathf.MoveTowards(this.SpinSpeed, 120f, Time.deltaTime * 15f);
        for (int index = this.Meatballs.Count - 1; index >= 0; --index)
        {
          if ((Object) this.Meatballs[index] == (Object) null)
            this.Meatballs.RemoveAt(index);
        }
        if (this.Meatballs.Count == 0)
        {
          this.Aud.Stop();
          this.DeactivateDamage();
          this.m_isSpinning = false;
          this.m_isAscending = true;
        }
      }
      if (this.m_isAscending)
      {
        this.SpinSpeed = Mathf.MoveTowards(this.SpinSpeed, 0.0f, Time.deltaTime * 90f);
        this.Height += Time.deltaTime * 4f;
        this.transform.position = new Vector3(this.startPos.x, this.Height, this.startPos.z);
        if ((double) this.Height >= 20.0)
          Object.Destroy((Object) this.gameObject);
      }
      float num = this.SpinSpeed / 50f;
      this.Aud.volume = num * num * 0.35f;
      this.Yrot += this.SpinSpeed * Time.deltaTime;
      this.Yrot = Mathf.Repeat(this.Yrot, 360f);
      this.transform.eulerAngles = new Vector3(0.0f, this.Yrot, 0.0f);
    }
  }
}
