// Decompiled with JetBrains decompiler
// Type: FistVR.wwHorseShoePlinth
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwHorseShoePlinth : MonoBehaviour
  {
    public int PlinthIndex;
    public wwHorseShoeGame Game;
    public GameObject HorseShoeTrigger;
    private bool m_isShoeActive;
    public GameObject GlyphNotCompleted;
    public GameObject GlyphCompleted;
    public Transform spinnybit;
    private float m_spinnyRot;
    public AudioEvent SuccessEvent;
    private bool m_isCompleted;

    public bool IsCompleted() => this.m_isCompleted;

    public void SetCompleted()
    {
      this.m_isCompleted = true;
      this.GlyphNotCompleted.SetActive(false);
      this.GlyphCompleted.SetActive(true);
    }

    public void HitSuccess()
    {
      SM.PlayGenericSound(this.SuccessEvent, this.transform.position);
      if (!this.m_isCompleted)
      {
        this.m_isCompleted = true;
        this.Game.RegisterSuccess(this.PlinthIndex);
      }
      this.SetCompleted();
    }

    public void GrabbedHorseshoe() => this.m_isShoeActive = true;

    public void NeedNewHorseshoe() => this.m_isShoeActive = false;

    public void Update()
    {
      if (this.m_isShoeActive)
        return;
      this.m_spinnyRot += 360f * Time.deltaTime;
      this.spinnybit.localEulerAngles = new Vector3(0.0f, this.m_spinnyRot, 0.0f);
    }
  }
}
