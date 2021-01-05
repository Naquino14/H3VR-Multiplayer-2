// Decompiled with JetBrains decompiler
// Type: FistVR.GPManipulatorButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class GPManipulatorButton : MonoBehaviour
  {
    public float MaxPointingRange = 5f;
    protected bool m_isBeingPointedAt;
    public Color ColorUnselected;
    public Color ColorSelected;
    public Button Button;
    public Renderer Rend;
    public string ColorName = "_Color";
    private float m_scaleTick;

    private void Awake()
    {
      if (!((Object) this.Button == (Object) null))
        return;
      this.Button = this.GetComponent<Button>();
    }

    public virtual void OnPoint()
    {
      this.m_isBeingPointedAt = true;
      this.BeginHoverDisplay();
    }

    public virtual void EndPoint()
    {
      this.m_isBeingPointedAt = false;
      this.EndHoverDisplay();
    }

    public virtual void BeginHoverDisplay()
    {
    }

    public virtual void EndHoverDisplay()
    {
    }

    public void Update()
    {
      if (this.m_isBeingPointedAt)
      {
        float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num <= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.Rend.material.SetColor(this.ColorName, Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick));
      }
      else
      {
        float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num >= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.Rend.material.SetColor(this.ColorName, Color.Lerp(this.ColorUnselected, this.ColorSelected, this.m_scaleTick));
      }
    }

    [ContextMenu("SetButton")]
    public void SetButton() => this.Button = this.GetComponent<Button>();

    [ContextMenu("SetRenderer")]
    public void SetRenderer() => this.Rend = this.GetComponent<Renderer>();
  }
}
