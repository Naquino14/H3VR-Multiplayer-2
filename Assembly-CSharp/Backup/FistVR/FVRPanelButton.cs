// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPanelButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRPanelButton : FVRInteractiveObject
  {
    private Button button;
    public float RefireLimit = 0.12f;
    private float tick;

    protected override void Awake()
    {
      base.Awake();
      this.button = this.GetComponent<Button>();
    }

    public override void Poke(FVRViveHand hand)
    {
      if ((double) this.tick < (double) this.RefireLimit)
        return;
      this.tick = 0.0f;
      base.Poke(hand);
      this.button.onClick.Invoke();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.tick >= 1.0)
        return;
      this.tick += Time.deltaTime;
    }
  }
}
