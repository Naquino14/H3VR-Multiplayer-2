// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPointableButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRPointableButton : FVRPointable
  {
    public Color ColorUnselected;
    public Color ColorSelected;
    public Button Button;
    public Image Image;
    public Text Text;
    public Renderer Rend;
    private bool m_usesRend;
    public string ColorName = "_Color";
    private float m_scaleTick;

    private void Awake()
    {
      if ((Object) this.Button == (Object) null)
        this.Button = this.GetComponent<Button>();
      if ((Object) this.Image != (Object) null)
        this.Image.color = this.ColorUnselected;
      if ((Object) this.Text != (Object) null)
        this.Text.color = this.ColorUnselected;
      if (!((Object) this.Rend != (Object) null))
        return;
      this.m_usesRend = true;
    }

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!hand.Input.TriggerDown || !((Object) this.Button != (Object) null))
        return;
      this.Button.onClick.Invoke();
    }

    public void ForceUpdate()
    {
      if ((Object) this.Image != (Object) null)
        this.Image.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
      if (!((Object) this.Text != (Object) null))
        return;
      this.Text.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
    }

    [ContextMenu("SetButton")]
    public void SetButton() => this.Button = this.GetComponent<Button>();

    [ContextMenu("SetText")]
    public void SetText() => this.Text = this.GetComponent<Text>();

    [ContextMenu("SetImage")]
    public void SetImage() => this.Image = this.GetComponent<Image>();

    [ContextMenu("SetRenderer")]
    public void SetRenderer() => this.Rend = this.GetComponent<Renderer>();

    public override void Update()
    {
      base.Update();
      if (this.m_usesRend)
      {
        if (this.m_isBeingPointedAt)
        {
          float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 5f, 0.0f, 1f);
          if ((double) num > (double) this.m_scaleTick)
          {
            this.m_scaleTick = num;
            this.Rend.material.SetColor(this.ColorName, Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick));
          }
        }
        else
        {
          float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 5f, 0.0f, 1f);
          if ((double) num < (double) this.m_scaleTick)
          {
            this.m_scaleTick = num;
            this.Rend.material.SetColor(this.ColorName, Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick));
          }
        }
      }
      if ((Object) this.Image != (Object) null)
      {
        if (this.m_isBeingPointedAt)
        {
          float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 5f, 0.0f, 1f);
          if ((double) num > (double) this.m_scaleTick)
          {
            this.m_scaleTick = num;
            this.Image.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
          }
        }
        else
        {
          float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 5f, 0.0f, 1f);
          if ((double) num < (double) this.m_scaleTick)
          {
            this.m_scaleTick = num;
            this.Image.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
          }
        }
      }
      if (!((Object) this.Text != (Object) null))
        return;
      if (this.m_isBeingPointedAt)
      {
        float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num <= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.Text.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
      }
      else
      {
        float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num >= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.Text.color = Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick);
      }
    }
  }
}
