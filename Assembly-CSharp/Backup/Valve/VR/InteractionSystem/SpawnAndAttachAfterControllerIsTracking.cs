// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SpawnAndAttachAfterControllerIsTracking
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
  {
    private Hand hand;
    public GameObject itemPrefab;

    private void Start() => this.hand = this.GetComponentInParent<Hand>();

    private void Update()
    {
      if (!((Object) this.itemPrefab != (Object) null) || !this.hand.isActive || !this.hand.isPoseValid)
        return;
      GameObject objectToAttach = Object.Instantiate<GameObject>(this.itemPrefab);
      objectToAttach.SetActive(true);
      this.hand.AttachObject(objectToAttach, GrabTypes.Scripted);
      this.hand.TriggerHapticPulse((ushort) 800);
      Object.Destroy((Object) this.gameObject);
      objectToAttach.transform.localScale = this.itemPrefab.transform.localScale;
    }
  }
}
