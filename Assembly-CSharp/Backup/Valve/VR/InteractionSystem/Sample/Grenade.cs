// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.Grenade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class Grenade : MonoBehaviour
  {
    public GameObject explodePartPrefab;
    public int explodeCount = 10;
    public float minMagnitudeToExplode = 1f;
    private Interactable interactable;

    private void Start() => this.interactable = this.GetComponent<Interactable>();

    private void OnCollisionEnter(Collision collision)
    {
      if ((Object) this.interactable != (Object) null && (Object) this.interactable.attachedToHand != (Object) null || (double) collision.impulse.magnitude <= (double) this.minMagnitudeToExplode)
        return;
      for (int index = 0; index < this.explodeCount; ++index)
        Object.Instantiate<GameObject>(this.explodePartPrefab, this.transform.position, this.transform.rotation).GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0.0f, 1f, 1f, 1f, 0.5f, 1f));
      Object.Destroy((Object) this.gameObject);
    }
  }
}
