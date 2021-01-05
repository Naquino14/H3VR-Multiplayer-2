// Decompiled with JetBrains decompiler
// Type: FistVR.MainMenuScenePointable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MainMenuScenePointable : FVRPointable
  {
    private Vector3 m_startScale;
    private float m_scaleTick;
    public MainMenuScreen Screen;
    public MainMenuSceneDef Def;

    private void Awake() => this.m_startScale = this.transform.localScale;

    public override void Update()
    {
      base.Update();
      if (this.m_isBeingPointedAt)
      {
        float num = Mathf.Clamp(this.m_scaleTick + Time.deltaTime * 3f, 0.0f, 1f);
        if ((double) num <= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.transform.localScale = Vector3.Lerp(this.m_startScale, this.m_startScale * 1.25f, this.m_scaleTick);
      }
      else
      {
        float num = Mathf.Clamp(this.m_scaleTick - Time.deltaTime * 3f, 0.0f, 1f);
        if ((double) num >= (double) this.m_scaleTick)
          return;
        this.m_scaleTick = num;
        this.transform.localScale = Vector3.Lerp(this.m_startScale, this.m_startScale * 1.25f, this.m_scaleTick);
      }
    }

    public override void OnPoint(FVRViveHand hand)
    {
      base.OnPoint(hand);
      if (!hand.Input.TriggerDown)
        return;
      this.Screen.SetSelectedScene(this.Def);
    }
  }
}
