// Decompiled with JetBrains decompiler
// Type: FistVR.MainMenuLoadSceneButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MainMenuLoadSceneButton : FVRPointable
  {
    public MainMenuScreen Screen;
    private Color m_colorUnselected;
    private Color m_colorSelected = Color.white;
    public Image ButtonImage;
    private float m_scaleTick;

    private void Awake() => this.m_colorUnselected = this.ButtonImage.color;

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!hand.Input.TriggerDown)
        return;
      this.Screen.LoadScene();
    }

    public override void Update()
    {
      base.Update();
      if (this.m_isBeingPointedAt)
      {
        float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num <= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.ButtonImage.color = Color.Lerp(this.m_colorUnselected, this.m_colorSelected, this.m_scaleTick);
      }
      else
      {
        float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 5f, 0.0f, 1f);
        if ((double) num >= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.ButtonImage.color = Color.Lerp(this.m_colorUnselected, this.m_colorSelected, this.m_scaleTick);
      }
    }
  }
}
