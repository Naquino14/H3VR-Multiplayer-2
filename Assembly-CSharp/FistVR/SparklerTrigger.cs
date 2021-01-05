// Decompiled with JetBrains decompiler
// Type: FistVR.SparklerTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SparklerTrigger : MonoBehaviour
  {
    public FVRSparkler Sparkler;
    public Collider collider;

    private void OnTriggerEnter(Collider col)
    {
      this.collider.enabled = false;
      this.collider.gameObject.layer = LayerMask.NameToLayer("NoCol");
      this.Sparkler.Ignite();
    }
  }
}
