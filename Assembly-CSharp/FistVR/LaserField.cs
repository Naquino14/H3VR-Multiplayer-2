// Decompiled with JetBrains decompiler
// Type: FistVR.LaserField
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LaserField : MonoBehaviour
  {
    public GameObject FieldObject;
    private bool m_isActive = true;
    private float m_distCheckTick = 0.25f;
    public AudioSource FieldBuzz;
    private bool m_isBuzzActive;

    public void Start() => this.m_distCheckTick = Random.Range(0.1f, 0.25f);

    public void Update()
    {
      if (!this.m_isActive)
        return;
      this.m_distCheckTick -= Time.deltaTime;
      if ((double) this.m_distCheckTick > 0.0)
        return;
      this.m_distCheckTick = Random.Range(0.1f, 0.25f);
      this.SoundCheck();
    }

    public void ShutDown()
    {
      if (!this.FieldObject.activeSelf)
        return;
      this.FieldObject.SetActive(false);
      this.m_isActive = true;
    }

    private void SoundCheck()
    {
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      if ((double) num < 8.0 && !this.m_isBuzzActive)
      {
        this.m_isBuzzActive = true;
        this.FieldBuzz.Play();
      }
      else
      {
        if ((double) num <= 10.0 || !this.m_isBuzzActive)
          return;
        this.m_isBuzzActive = false;
        this.FieldBuzz.Stop();
      }
    }
  }
}
