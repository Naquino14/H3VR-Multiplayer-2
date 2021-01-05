// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltRPMFireSelector
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class OpenBoltRPMFireSelector : MonoBehaviour
  {
    public OpenBoltReceiver Receiver;
    public OpenBoltReceiverBolt Bolt;
    public OpenBoltRPMFireSelector.BoltSetting[] Settings;

    private void Update()
    {
      this.Bolt.BoltSpeed_Forward = this.Settings[this.Receiver.FireSelectorModeIndex].ForwardSpeed;
      this.Bolt.BoltSpeed_Rearward = this.Settings[this.Receiver.FireSelectorModeIndex].RearwardSpeed;
      this.Bolt.BoltSpringStiffness = this.Settings[this.Receiver.FireSelectorModeIndex].Stiffness;
    }

    [Serializable]
    public class BoltSetting
    {
      public float ForwardSpeed;
      public float RearwardSpeed;
      public float Stiffness;
    }
  }
}
