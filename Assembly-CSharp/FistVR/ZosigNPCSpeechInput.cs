// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigNPCSpeechInput
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigNPCSpeechInput : FVRPointable
  {
    public ZosigNPCInterface Interface;
    public Renderer Rend;
    public ZosigNPCProfile.NPCLine Line;
    private bool m_usesRend;
    public string ColorName = "_Color";
    public Color ColorUnselected;
    public Color ColorSelected;
    private float m_scaleTick;

    private void Awake()
    {
      if ((Object) this.Rend != (Object) null)
        this.m_usesRend = true;
      if (!this.m_usesRend)
        return;
      this.Rend.material.SetColor(this.ColorName, this.ColorUnselected);
    }

    public void SetLine(ZosigNPCProfile.NPCLine line)
    {
      this.Rend.material.SetTexture("_MainTex", (Texture) this.Interface.M.NPCSpeechIcons[(int) line.Type]);
      this.Line = line;
    }

    public void ClearLine() => this.Line = (ZosigNPCProfile.NPCLine) null;

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!hand.Input.TriggerDown)
        return;
      this.Interface.SpeakLine(this.Line);
    }

    public override void Update()
    {
      base.Update();
      if (!this.m_usesRend)
        return;
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
  }
}
