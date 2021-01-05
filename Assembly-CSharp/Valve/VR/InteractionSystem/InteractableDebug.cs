// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.InteractableDebug
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class InteractableDebug : MonoBehaviour
  {
    [NonSerialized]
    public Hand attachedToHand;
    public float simulateReleasesForXSecondsAroundRelease;
    public float simulateReleasesEveryXSeconds = 0.005f;
    public bool setPositionsForSimulations;
    private Renderer[] selfRenderers;
    private Collider[] colliders;
    private Color lastColor;
    private Throwable throwable;
    private const bool onlyColorOnChange = true;
    public Rigidbody rigidbody;
    private bool isSimulation;

    private bool isThrowable => (UnityEngine.Object) this.throwable != (UnityEngine.Object) null;

    private void Awake()
    {
      this.selfRenderers = this.GetComponentsInChildren<Renderer>();
      this.throwable = this.GetComponent<Throwable>();
      this.rigidbody = this.GetComponent<Rigidbody>();
      this.colliders = this.GetComponentsInChildren<Collider>();
    }

    private void OnAttachedToHand(Hand hand)
    {
      this.attachedToHand = hand;
      this.CreateMarker(Color.green);
    }

    protected virtual void HandAttachedUpdate(Hand hand)
    {
      Color newColor;
      switch (hand.currentAttachedObjectInfo.Value.grabbedWithType)
      {
        case GrabTypes.Trigger:
          newColor = Color.yellow;
          break;
        case GrabTypes.Pinch:
          newColor = Color.green;
          break;
        case GrabTypes.Grip:
          newColor = Color.blue;
          break;
        case GrabTypes.Scripted:
          newColor = Color.red;
          break;
        default:
          newColor = Color.white;
          break;
      }
      if (newColor != this.lastColor)
        this.ColorSelf(newColor);
      this.lastColor = newColor;
    }

    private void OnDetachedFromHand(Hand hand)
    {
      if (this.isThrowable)
      {
        Vector3 velocity;
        this.throwable.GetReleaseVelocities(hand, out velocity, out Vector3 _);
        this.CreateMarker(Color.cyan, velocity.normalized);
      }
      this.CreateMarker(Color.red);
      this.attachedToHand = (Hand) null;
      if (this.isSimulation || (double) this.simulateReleasesForXSecondsAroundRelease == 0.0)
        return;
      float a = -this.simulateReleasesForXSecondsAroundRelease;
      float xsecondsAroundRelease = this.simulateReleasesForXSecondsAroundRelease;
      List<InteractableDebug> interactableDebugList = new List<InteractableDebug>();
      interactableDebugList.Add(this);
      for (float timeOffset = a; (double) timeOffset <= (double) xsecondsAroundRelease; timeOffset += this.simulateReleasesEveryXSeconds)
      {
        float t = Mathf.InverseLerp(a, xsecondsAroundRelease, timeOffset);
        InteractableDebug simulation = this.CreateSimulation(hand, timeOffset, Color.Lerp(Color.red, Color.green, t));
        interactableDebugList.Add(simulation);
      }
      for (int index1 = 0; index1 < interactableDebugList.Count; ++index1)
      {
        for (int index2 = 0; index2 < interactableDebugList.Count; ++index2)
          interactableDebugList[index1].IgnoreObject(interactableDebugList[index2]);
      }
    }

    public Collider[] GetColliders() => this.colliders;

    public void IgnoreObject(InteractableDebug otherInteractable)
    {
      Collider[] colliders = otherInteractable.GetColliders();
      for (int index1 = 0; index1 < this.colliders.Length; ++index1)
      {
        for (int index2 = 0; index2 < colliders.Length; ++index2)
          Physics.IgnoreCollision(this.colliders[index1], colliders[index2]);
      }
    }

    public void SetIsSimulation() => this.isSimulation = true;

    private InteractableDebug CreateSimulation(
      Hand fromHand,
      float timeOffset,
      Color copyColor)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameObject);
      InteractableDebug component = gameObject.GetComponent<InteractableDebug>();
      component.SetIsSimulation();
      component.ColorSelf(copyColor);
      gameObject.name = string.Format("{0} [offset: {1:0.000}]", (object) gameObject.name, (object) timeOffset);
      Vector3 vector3 = fromHand.GetTrackedObjectVelocity(timeOffset) * this.throwable.scaleReleaseVelocity;
      component.rigidbody.velocity = vector3;
      return component;
    }

    private void CreateMarker(Color markerColor, float destroyAfter = 10f) => this.CreateMarker(markerColor, this.attachedToHand.GetTrackedObjectVelocity().normalized, destroyAfter);

    private void CreateMarker(Color markerColor, Vector3 forward, float destroyAfter = 10f)
    {
      GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) primitive.GetComponent<Collider>());
      primitive.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(primitive);
      gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.25f);
      gameObject.transform.parent = primitive.transform;
      gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, gameObject.transform.localScale.z / 2f);
      primitive.transform.position = this.attachedToHand.transform.position;
      primitive.transform.forward = forward;
      this.ColorThing(markerColor, primitive.GetComponentsInChildren<Renderer>());
      if ((double) destroyAfter <= 0.0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) primitive, destroyAfter);
    }

    private void ColorSelf(Color newColor) => this.ColorThing(newColor, this.selfRenderers);

    private void ColorThing(Color newColor, Renderer[] renderers)
    {
      for (int index = 0; index < renderers.Length; ++index)
        renderers[index].material.color = newColor;
    }
  }
}
