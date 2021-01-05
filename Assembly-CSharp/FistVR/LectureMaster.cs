// Decompiled with JetBrains decompiler
// Type: FistVR.LectureMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class LectureMaster : MonoBehaviour
  {
    public List<GameObject> Cameras;
    public List<Transform> Slides;
    public SlideSequenceDef SlidesDef;
    public List<FVRViveHand> Hands;
    private int curSlide = -1;
    public Text Notes;
    private FVRObject CurFO_Head;
    private FVRObject CurFO_Torso;
    private FVRObject CurFO_Legs;
    private GameObject m_cur_head;
    private GameObject m_cur_torso;
    private GameObject m_cur_legs;
    public Transform Targ_Head;
    public Transform Targ_Torso;
    public Transform Targ_Legs;
    private float refire = 0.2f;

    private void Start()
    {
    }

    private void UpdateClothing()
    {
      this.MaybeReplace(this.CurFO_Head, this.SlidesDef.Slides[this.curSlide].Head, ref this.m_cur_head, this.Targ_Head);
      this.MaybeReplace(this.CurFO_Torso, this.SlidesDef.Slides[this.curSlide].Torso, ref this.m_cur_torso, this.Targ_Torso);
      this.MaybeReplace(this.CurFO_Legs, this.SlidesDef.Slides[this.curSlide].Abdomen, ref this.m_cur_legs, this.Targ_Legs);
    }

    private void MaybeReplace(
      FVRObject curFO,
      FVRObject slideFO,
      ref GameObject curPiece,
      Transform targ)
    {
      if (!((Object) curFO == (Object) null) && !((Object) slideFO == (Object) null) && !(slideFO.ItemID != curFO.ItemID))
        return;
      if ((Object) curPiece != (Object) null)
      {
        curPiece.transform.SetParent((Transform) null);
        Object.Destroy((Object) curPiece);
      }
      if (!((Object) slideFO != (Object) null))
        return;
      curPiece = Object.Instantiate<GameObject>(slideFO.GetGameObject(), targ.position, targ.rotation);
      curPiece.transform.SetParent(targ);
      curPiece.layer = LayerMask.NameToLayer("ExternalCamOnly");
    }

    private void Update()
    {
      if ((double) this.refire > 0.0)
        this.refire -= Time.deltaTime;
      for (int index = 0; index < this.Hands.Count; ++index)
      {
        if (this.Hands[index].Input.AXButtonDown)
        {
          if (this.Hands[index].IsThisTheRightHand)
            this.Advance();
          else
            this.GoBack();
        }
      }
    }

    private void Advance()
    {
      if ((double) this.refire > 0.0)
        return;
      this.refire = 0.2f;
      ++this.curSlide;
      if (this.curSlide >= this.SlidesDef.Slides.Count)
        this.curSlide = this.SlidesDef.Slides.Count - 1;
      this.UpdateCam();
      this.UpdateSlide();
      this.UpdateText();
      this.UpdateClothing();
    }

    private void GoBack()
    {
      if ((double) this.refire > 0.0)
        return;
      this.refire = 0.2f;
      --this.curSlide;
      if (this.curSlide < 0)
        this.curSlide = 0;
      this.UpdateCam();
      this.UpdateSlide();
      this.UpdateText();
      this.UpdateClothing();
    }

    private void UpdateCam()
    {
      for (int index = 0; index < this.Cameras.Count; ++index)
      {
        if ((LectureMaster.LectureCam) index == this.SlidesDef.Slides[this.curSlide].CameraIndex)
          this.Cameras[index].SetActive(true);
        else
          this.Cameras[index].SetActive(false);
      }
    }

    private void UpdateSlide()
    {
      for (int index = 0; index < this.Slides.Count; ++index)
      {
        if (index == this.SlidesDef.Slides[this.curSlide].SlideIndex)
          this.Slides[index].gameObject.SetActive(true);
        else
          this.Slides[index].gameObject.SetActive(false);
      }
    }

    private void UpdateText() => this.Notes.text = this.SlidesDef.Slides[this.curSlide].text;

    public enum LectureCam
    {
      CloseStandard,
      CloseStandard2,
      CloseStandard3,
      CloseStandard4,
      MidStandard,
      MidStandard2,
      MidStandard3,
      MidLeft,
      MidRight,
      MidLow,
      WideCorner,
      WideCentered,
      WideHigh,
      WideHighAngleLeft,
      WideHighAngleRight,
      WideLow,
    }
  }
}
