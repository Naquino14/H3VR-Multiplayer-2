// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ComplexThrowable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class ComplexThrowable : MonoBehaviour
  {
    public float attachForce = 800f;
    public float attachForceDamper = 25f;
    public ComplexThrowable.AttachMode attachMode;
    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags;
    private List<Hand> holdingHands = new List<Hand>();
    private List<Rigidbody> holdingBodies = new List<Rigidbody>();
    private List<Vector3> holdingPoints = new List<Vector3>();
    private List<Rigidbody> rigidBodies = new List<Rigidbody>();

    private void Awake() => this.GetComponentsInChildren<Rigidbody>(this.rigidBodies);

    private void Update()
    {
      for (int index = 0; index < this.holdingHands.Count; ++index)
      {
        if (this.holdingHands[index].IsGrabEnding(this.gameObject))
          this.PhysicsDetach(this.holdingHands[index]);
      }
    }

    private void OnHandHoverBegin(Hand hand)
    {
      if (this.holdingHands.IndexOf(hand) != -1 || !hand.isActive)
        return;
      hand.TriggerHapticPulse((ushort) 800);
    }

    private void OnHandHoverEnd(Hand hand)
    {
      if (this.holdingHands.IndexOf(hand) != -1 || !hand.isActive)
        return;
      hand.TriggerHapticPulse((ushort) 500);
    }

    private void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      if (grabStarting == GrabTypes.None)
        return;
      this.PhysicsAttach(hand, grabStarting);
    }

    private void PhysicsAttach(Hand hand, GrabTypes startingGrabType)
    {
      this.PhysicsDetach(hand);
      Rigidbody rigidbody = (Rigidbody) null;
      Vector3 zero = Vector3.zero;
      float num1 = float.MaxValue;
      for (int index = 0; index < this.rigidBodies.Count; ++index)
      {
        float num2 = Vector3.Distance(this.rigidBodies[index].worldCenterOfMass, hand.transform.position);
        if ((double) num2 < (double) num1)
        {
          rigidbody = this.rigidBodies[index];
          num1 = num2;
        }
      }
      if ((Object) rigidbody == (Object) null)
        return;
      if (this.attachMode == ComplexThrowable.AttachMode.FixedJoint)
      {
        Util.FindOrAddComponent<Rigidbody>(hand.gameObject).isKinematic = true;
        hand.gameObject.AddComponent<FixedJoint>().connectedBody = rigidbody;
      }
      hand.HoverLock((Interactable) null);
      Vector3 vector3_1 = hand.transform.position - rigidbody.worldCenterOfMass;
      vector3_1 = Mathf.Min(vector3_1.magnitude, 1f) * vector3_1.normalized;
      Vector3 vector3_2 = rigidbody.transform.InverseTransformPoint(rigidbody.worldCenterOfMass + vector3_1);
      hand.AttachObject(this.gameObject, startingGrabType, this.attachmentFlags);
      this.holdingHands.Add(hand);
      this.holdingBodies.Add(rigidbody);
      this.holdingPoints.Add(vector3_2);
    }

    private bool PhysicsDetach(Hand hand)
    {
      int index = this.holdingHands.IndexOf(hand);
      if (index == -1)
        return false;
      this.holdingHands[index].DetachObject(this.gameObject, false);
      this.holdingHands[index].HoverUnlock((Interactable) null);
      if (this.attachMode == ComplexThrowable.AttachMode.FixedJoint)
        Object.Destroy((Object) this.holdingHands[index].GetComponent<FixedJoint>());
      Util.FastRemove<Hand>(this.holdingHands, index);
      Util.FastRemove<Rigidbody>(this.holdingBodies, index);
      Util.FastRemove<Vector3>(this.holdingPoints, index);
      return true;
    }

    private void FixedUpdate()
    {
      if (this.attachMode != ComplexThrowable.AttachMode.Force)
        return;
      for (int index = 0; index < this.holdingHands.Count; ++index)
      {
        Vector3 vector3_1 = this.holdingBodies[index].transform.TransformPoint(this.holdingPoints[index]);
        Vector3 vector3_2 = this.holdingHands[index].transform.position - vector3_1;
        this.holdingBodies[index].AddForceAtPosition(this.attachForce * vector3_2, vector3_1, ForceMode.Acceleration);
        this.holdingBodies[index].AddForceAtPosition(-this.attachForceDamper * this.holdingBodies[index].GetPointVelocity(vector3_1), vector3_1, ForceMode.Acceleration);
      }
    }

    public enum AttachMode
    {
      FixedJoint,
      Force,
    }
  }
}
