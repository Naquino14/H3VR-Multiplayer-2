// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.BodyCollider
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (CapsuleCollider))]
  public class BodyCollider : MonoBehaviour
  {
    public Transform head;
    private CapsuleCollider capsuleCollider;

    private void Awake() => this.capsuleCollider = this.GetComponent<CapsuleCollider>();

    private void FixedUpdate()
    {
      float b = Vector3.Dot(this.head.localPosition, Vector3.up);
      this.capsuleCollider.height = Mathf.Max(this.capsuleCollider.radius, b);
      this.transform.localPosition = this.head.localPosition - 0.5f * b * Vector3.up;
    }
  }
}
