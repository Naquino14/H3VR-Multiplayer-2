// Decompiled with JetBrains decompiler
// Type: Valve.VR.Extras.SteamVR_TestThrow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.Extras
{
  [RequireComponent(typeof (SteamVR_TrackedObject))]
  public class SteamVR_TestThrow : MonoBehaviour
  {
    public GameObject prefab;
    public Rigidbody attachPoint;
    public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private SteamVR_Behaviour_Pose trackedObj;
    private FixedJoint joint;

    private void Awake() => this.trackedObj = this.GetComponent<SteamVR_Behaviour_Pose>();

    private void FixedUpdate()
    {
      if ((Object) this.joint == (Object) null && this.spawn.GetStateDown(this.trackedObj.inputSource))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
        gameObject.transform.position = this.attachPoint.transform.position;
        this.joint = gameObject.AddComponent<FixedJoint>();
        this.joint.connectedBody = this.attachPoint;
      }
      else
      {
        if (!((Object) this.joint != (Object) null) || !this.spawn.GetStateUp(this.trackedObj.inputSource))
          return;
        GameObject gameObject = this.joint.gameObject;
        Rigidbody component = gameObject.GetComponent<Rigidbody>();
        Object.DestroyImmediate((Object) this.joint);
        this.joint = (FixedJoint) null;
        Object.Destroy((Object) gameObject, 15f);
        Transform transform = !(bool) (Object) this.trackedObj.origin ? this.trackedObj.transform.parent : this.trackedObj.origin;
        if ((Object) transform != (Object) null)
        {
          component.velocity = transform.TransformVector(this.trackedObj.GetVelocity());
          component.angularVelocity = transform.TransformVector(this.trackedObj.GetAngularVelocity());
        }
        else
        {
          component.velocity = this.trackedObj.GetVelocity();
          component.angularVelocity = this.trackedObj.GetAngularVelocity();
        }
        component.maxAngularVelocity = component.angularVelocity.magnitude;
      }
    }
  }
}
