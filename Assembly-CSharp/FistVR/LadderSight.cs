// Decompiled with JetBrains decompiler
// Type: FistVR.LadderSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class LadderSight : FVRInteractiveObject
  {
    public Transform Ladder;
    public Transform Bead;
    public List<string> RangeNames;
    public List<float> LadderRots;
    public List<Vector3> BeadLocalPoses;
    public int Setting = 1;
    public OpticUI UI;
    public Vector3 offset = new Vector3(0.0f, 0.02f, -0.01f);

    protected override void Start()
    {
      base.Start();
      this.UpdateRots();
      GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<AM>.Instance.Prefab_OpticUI, this.transform.position + (this.transform.up * this.offset.y + this.transform.forward * this.offset.z), this.transform.rotation);
      this.UI = gameObject.GetComponent<OpticUI>();
      this.UI.UpdateUI(this);
      gameObject.SetActive(false);
      this.UXGeo_Held = gameObject;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.IsHeld)
        return;
      this.UI.transform.position = this.transform.position + (this.transform.up * this.offset.y + this.transform.forward * this.offset.z);
      this.UI.transform.rotation = this.transform.rotation;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.SettingDown();
        if (!hand.Input.AXButtonDown)
          return;
        this.SettingUp();
      }
      else
      {
        if (!hand.Input.TouchpadDown)
          return;
        if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45.0)
        {
          this.SettingDown();
        }
        else
        {
          if ((double) Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) >= 45.0)
            return;
          this.SettingUp();
        }
      }
    }

    private void SettingDown()
    {
      --this.Setting;
      if (this.Setting < 0)
        this.Setting = 0;
      this.UpdateRots();
      this.UI.UpdateUI(this);
    }

    private void SettingUp()
    {
      ++this.Setting;
      if (this.Setting >= this.LadderRots.Count)
        this.Setting = this.LadderRots.Count - 1;
      this.UpdateRots();
      this.UI.UpdateUI(this);
    }

    [ContextMenu("UpdateRots")]
    private void UpdateRots()
    {
      this.Ladder.localEulerAngles = new Vector3(this.LadderRots[this.Setting], 0.0f, 0.0f);
      this.Bead.localPosition = this.BeadLocalPoses[this.Setting];
    }
  }
}
