// Decompiled with JetBrains decompiler
// Type: SeekBarCtrl
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeekBarCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEventSystemHandler
{
  public MediaPlayerCtrl m_srcVideo;
  public Slider m_srcSlider;
  public float m_fDragTime = 0.2f;
  private bool m_bActiveDrag = true;
  private bool m_bUpdate = true;
  private float m_fDeltaTime;
  private float m_fLastValue;
  private float m_fLastSetValue;

  private void Start()
  {
  }

  private void Update()
  {
    if (!this.m_bActiveDrag)
    {
      this.m_fDeltaTime += Time.deltaTime;
      if ((double) this.m_fDeltaTime > (double) this.m_fDragTime)
      {
        this.m_bActiveDrag = true;
        this.m_fDeltaTime = 0.0f;
      }
    }
    if (!this.m_bUpdate || !((Object) this.m_srcVideo != (Object) null) || !((Object) this.m_srcSlider != (Object) null))
      return;
    this.m_srcSlider.value = this.m_srcVideo.GetSeekBarValue();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Debug.Log((object) "OnPointerEnter:");
    this.m_bUpdate = false;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    Debug.Log((object) "OnPointerExit:");
    this.m_bUpdate = true;
  }

  public void OnPointerDown(PointerEventData eventData)
  {
  }

  public void OnPointerUp(PointerEventData eventData) => this.m_srcVideo.SetSeekBarValue(this.m_srcSlider.value);

  public void OnDrag(PointerEventData eventData)
  {
    Debug.Log((object) ("OnDrag:" + (object) eventData));
    if (!this.m_bActiveDrag)
    {
      this.m_fLastValue = this.m_srcSlider.value;
    }
    else
    {
      this.m_srcVideo.SetSeekBarValue(this.m_srcSlider.value);
      this.m_fLastSetValue = this.m_srcSlider.value;
      this.m_bActiveDrag = false;
    }
  }
}
