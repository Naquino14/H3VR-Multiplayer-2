// Decompiled with JetBrains decompiler
// Type: AmplifyColorTriggerProxy2D
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (CircleCollider2D))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy2D : AmplifyColorTriggerProxyBase
{
  private CircleCollider2D circleCollider;
  private Rigidbody2D rigidBody;

  private void Start()
  {
    this.circleCollider = this.GetComponent<CircleCollider2D>();
    this.circleCollider.radius = 0.01f;
    this.circleCollider.isTrigger = true;
    this.rigidBody = this.GetComponent<Rigidbody2D>();
    this.rigidBody.gravityScale = 0.0f;
    this.rigidBody.isKinematic = true;
  }

  private void LateUpdate()
  {
    this.transform.position = this.Reference.position;
    this.transform.rotation = this.Reference.rotation;
  }
}
