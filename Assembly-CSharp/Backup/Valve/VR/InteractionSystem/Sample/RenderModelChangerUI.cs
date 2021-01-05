// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.RenderModelChangerUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class RenderModelChangerUI : UIElement
  {
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    protected SkeletonUIOptions ui;

    protected override void Awake()
    {
      base.Awake();
      this.ui = this.GetComponentInParent<SkeletonUIOptions>();
    }

    protected override void OnButtonClick()
    {
      base.OnButtonClick();
      if (!((Object) this.ui != (Object) null))
        return;
      this.ui.SetRenderModel(this);
    }
  }
}
