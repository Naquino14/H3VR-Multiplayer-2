// Decompiled with JetBrains decompiler
// Type: AmplifyColorTriggerProxy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (SphereCollider))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
  private SphereCollider sphereCollider;
  private Rigidbody rigidBody;

  private void Start()
  {
    this.sphereCollider = this.GetComponent<SphereCollider>();
    this.sphereCollider.radius = 0.01f;
    this.sphereCollider.isTrigger = true;
    this.rigidBody = this.GetComponent<Rigidbody>();
    this.rigidBody.useGravity = false;
    this.rigidBody.isKinematic = true;
  }

  private void LateUpdate()
  {
    this.transform.position = this.Reference.position;
    this.transform.rotation = this.Reference.rotation;
  }
}
