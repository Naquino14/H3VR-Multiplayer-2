// Decompiled with JetBrains decompiler
// Type: FistVR.wwAmmoTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwAmmoTrigger : FVRInteractiveObject
  {
    public FVRObject AmmoToSpawn;
    public Transform Spinner;
    private float spin;
    private bool m_hasSpinner;

    public new void Start()
    {
      if (!((Object) this.Spinner != (Object) null))
        return;
      this.m_hasSpinner = true;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      FVRPhysicalObject component = Object.Instantiate<GameObject>(this.AmmoToSpawn.GetGameObject(), this.transform.position, this.transform.rotation).GetComponent<FVRPhysicalObject>();
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_hasSpinner)
        return;
      this.spin = Mathf.Repeat(this.spin + Time.deltaTime * 180f, 360f);
      this.Spinner.transform.localEulerAngles = new Vector3(0.0f, this.spin, 0.0f);
    }
  }
}
