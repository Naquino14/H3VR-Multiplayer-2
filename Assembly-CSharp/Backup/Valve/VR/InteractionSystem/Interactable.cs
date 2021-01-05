// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Interactable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class Interactable : MonoBehaviour
  {
    [Tooltip("Activates an action set on attach and deactivates on detach")]
    public SteamVR_ActionSet activateActionSetOnAttach;
    [Tooltip("Hide the whole hand on attachment and show on detach")]
    public bool hideHandOnAttach = true;
    [Tooltip("Hide the skeleton part of the hand on attachment and show on detach")]
    public bool hideSkeletonOnAttach;
    [Tooltip("Hide the controller part of the hand on attachment and show on detach")]
    public bool hideControllerOnAttach;
    [Tooltip("The integer in the animator to trigger on pickup. 0 for none")]
    public int handAnimationOnPickup;
    [Tooltip("The range of motion to set on the skeleton. None for no change.")]
    public SkeletalMotionRangeChange setRangeOfMotionOnPickup = SkeletalMotionRangeChange.None;
    [Tooltip("Specify whether you want to snap to the hand's object attachment point, or just the raw hand")]
    public bool useHandObjectAttachmentPoint = true;
    public bool attachEaseIn;
    [HideInInspector]
    public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
    public float snapAttachEaseInTime = 0.15f;
    public bool snapAttachEaseInCompleted;
    [HideInInspector]
    public SteamVR_Skeleton_Poser skeletonPoser;
    [Tooltip("Should the rendered hand lock on to and follow the object")]
    public bool handFollowTransform = true;
    [Tooltip("Set whether or not you want this interactible to highlight when hovering over it")]
    public bool highlightOnHover = true;
    protected MeshRenderer[] highlightRenderers;
    protected MeshRenderer[] existingRenderers;
    protected GameObject highlightHolder;
    protected SkinnedMeshRenderer[] highlightSkinnedRenderers;
    protected SkinnedMeshRenderer[] existingSkinnedRenderers;
    protected static Material highlightMat;
    [Tooltip("An array of child gameObjects to not render a highlight for. Things like transparent parts, vfx, etc.")]
    public GameObject[] hideHighlight;
    [NonSerialized]
    public Hand attachedToHand;
    [NonSerialized]
    public Hand hoveringHand;
    protected float blendToPoseTime = 0.1f;
    protected float releasePoseBlendTime = 0.2f;

    public event Interactable.OnAttachedToHandDelegate onAttachedToHand;

    public event Interactable.OnDetachedFromHandDelegate onDetachedFromHand;

    public bool isDestroying { get; protected set; }

    public bool isHovering { get; protected set; }

    public bool wasHovering { get; protected set; }

    private void Awake() => this.skeletonPoser = this.GetComponent<SteamVR_Skeleton_Poser>();

    protected virtual void Start()
    {
      Interactable.highlightMat = (Material) Resources.Load("SteamVR_HoverHighlight", typeof (Material));
      if ((UnityEngine.Object) Interactable.highlightMat == (UnityEngine.Object) null)
        Debug.LogError((object) "<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");
      if (!((UnityEngine.Object) this.skeletonPoser != (UnityEngine.Object) null) || !this.useHandObjectAttachmentPoint)
        return;
      this.useHandObjectAttachmentPoint = false;
    }

    protected virtual bool ShouldIgnoreHighlight(Component component) => this.ShouldIgnore(component.gameObject);

    protected virtual bool ShouldIgnore(GameObject check)
    {
      for (int index = 0; index < this.hideHighlight.Length; ++index)
      {
        if ((UnityEngine.Object) check == (UnityEngine.Object) this.hideHighlight[index])
          return true;
      }
      return false;
    }

    protected virtual void CreateHighlightRenderers()
    {
      this.existingSkinnedRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
      this.highlightHolder = new GameObject("Highlighter");
      this.highlightSkinnedRenderers = new SkinnedMeshRenderer[this.existingSkinnedRenderers.Length];
      for (int index1 = 0; index1 < this.existingSkinnedRenderers.Length; ++index1)
      {
        SkinnedMeshRenderer existingSkinnedRenderer = this.existingSkinnedRenderers[index1];
        if (!this.ShouldIgnoreHighlight((Component) existingSkinnedRenderer))
        {
          SkinnedMeshRenderer skinnedMeshRenderer = new GameObject("SkinnedHolder")
          {
            transform = {
              parent = this.highlightHolder.transform
            }
          }.AddComponent<SkinnedMeshRenderer>();
          Material[] materialArray = new Material[existingSkinnedRenderer.sharedMaterials.Length];
          for (int index2 = 0; index2 < materialArray.Length; ++index2)
            materialArray[index2] = Interactable.highlightMat;
          skinnedMeshRenderer.sharedMaterials = materialArray;
          skinnedMeshRenderer.sharedMesh = existingSkinnedRenderer.sharedMesh;
          skinnedMeshRenderer.rootBone = existingSkinnedRenderer.rootBone;
          skinnedMeshRenderer.updateWhenOffscreen = existingSkinnedRenderer.updateWhenOffscreen;
          skinnedMeshRenderer.bones = existingSkinnedRenderer.bones;
          this.highlightSkinnedRenderers[index1] = skinnedMeshRenderer;
        }
      }
      MeshFilter[] componentsInChildren = this.GetComponentsInChildren<MeshFilter>(true);
      this.existingRenderers = new MeshRenderer[componentsInChildren.Length];
      this.highlightRenderers = new MeshRenderer[componentsInChildren.Length];
      for (int index1 = 0; index1 < componentsInChildren.Length; ++index1)
      {
        MeshFilter meshFilter = componentsInChildren[index1];
        MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
        if (!((UnityEngine.Object) meshFilter == (UnityEngine.Object) null) && !((UnityEngine.Object) component == (UnityEngine.Object) null) && !this.ShouldIgnoreHighlight((Component) meshFilter))
        {
          GameObject gameObject = new GameObject("FilterHolder");
          gameObject.transform.parent = this.highlightHolder.transform;
          gameObject.AddComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
          MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
          Material[] materialArray = new Material[component.sharedMaterials.Length];
          for (int index2 = 0; index2 < materialArray.Length; ++index2)
            materialArray[index2] = Interactable.highlightMat;
          meshRenderer.sharedMaterials = materialArray;
          this.highlightRenderers[index1] = meshRenderer;
          this.existingRenderers[index1] = component;
        }
      }
    }

    protected virtual void UpdateHighlightRenderers()
    {
      if ((UnityEngine.Object) this.highlightHolder == (UnityEngine.Object) null)
        return;
      for (int index1 = 0; index1 < this.existingSkinnedRenderers.Length; ++index1)
      {
        SkinnedMeshRenderer existingSkinnedRenderer = this.existingSkinnedRenderers[index1];
        SkinnedMeshRenderer highlightSkinnedRenderer = this.highlightSkinnedRenderers[index1];
        if ((UnityEngine.Object) existingSkinnedRenderer != (UnityEngine.Object) null && (UnityEngine.Object) highlightSkinnedRenderer != (UnityEngine.Object) null && !(bool) (UnityEngine.Object) this.attachedToHand)
        {
          highlightSkinnedRenderer.transform.position = existingSkinnedRenderer.transform.position;
          highlightSkinnedRenderer.transform.rotation = existingSkinnedRenderer.transform.rotation;
          highlightSkinnedRenderer.transform.localScale = existingSkinnedRenderer.transform.lossyScale;
          highlightSkinnedRenderer.localBounds = existingSkinnedRenderer.localBounds;
          highlightSkinnedRenderer.enabled = this.isHovering && existingSkinnedRenderer.enabled && existingSkinnedRenderer.gameObject.activeInHierarchy;
          int blendShapeCount = existingSkinnedRenderer.sharedMesh.blendShapeCount;
          for (int index2 = 0; index2 < blendShapeCount; ++index2)
            highlightSkinnedRenderer.SetBlendShapeWeight(index2, existingSkinnedRenderer.GetBlendShapeWeight(index2));
        }
        else if ((UnityEngine.Object) highlightSkinnedRenderer != (UnityEngine.Object) null)
          highlightSkinnedRenderer.enabled = false;
      }
      for (int index = 0; index < this.highlightRenderers.Length; ++index)
      {
        MeshRenderer existingRenderer = this.existingRenderers[index];
        MeshRenderer highlightRenderer = this.highlightRenderers[index];
        if ((UnityEngine.Object) existingRenderer != (UnityEngine.Object) null && (UnityEngine.Object) highlightRenderer != (UnityEngine.Object) null && !(bool) (UnityEngine.Object) this.attachedToHand)
        {
          highlightRenderer.transform.position = existingRenderer.transform.position;
          highlightRenderer.transform.rotation = existingRenderer.transform.rotation;
          highlightRenderer.transform.localScale = existingRenderer.transform.lossyScale;
          highlightRenderer.enabled = this.isHovering && existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;
        }
        else if ((UnityEngine.Object) highlightRenderer != (UnityEngine.Object) null)
          highlightRenderer.enabled = false;
      }
    }

    protected virtual void OnHandHoverBegin(Hand hand)
    {
      this.wasHovering = this.isHovering;
      this.isHovering = true;
      this.hoveringHand = hand;
      if (!this.highlightOnHover)
        return;
      this.CreateHighlightRenderers();
      this.UpdateHighlightRenderers();
    }

    private void OnHandHoverEnd(Hand hand)
    {
      this.wasHovering = this.isHovering;
      this.isHovering = false;
      if (!this.highlightOnHover || !((UnityEngine.Object) this.highlightHolder != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.highlightHolder);
    }

    protected virtual void Update()
    {
      if (!this.highlightOnHover)
        return;
      this.UpdateHighlightRenderers();
      if (this.isHovering || !((UnityEngine.Object) this.highlightHolder != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.highlightHolder);
    }

    protected virtual void OnAttachedToHand(Hand hand)
    {
      if (this.activateActionSetOnAttach != (SteamVR_ActionSet) null)
        this.activateActionSetOnAttach.Activate(hand.handType, 0, false);
      if (this.onAttachedToHand != null)
        this.onAttachedToHand(hand);
      if ((UnityEngine.Object) this.skeletonPoser != (UnityEngine.Object) null && (UnityEngine.Object) hand.skeleton != (UnityEngine.Object) null)
        hand.skeleton.BlendToPoser(this.skeletonPoser, this.blendToPoseTime);
      this.attachedToHand = hand;
    }

    protected virtual void OnDetachedFromHand(Hand hand)
    {
      if (this.activateActionSetOnAttach != (SteamVR_ActionSet) null && ((UnityEngine.Object) hand.otherHand == (UnityEngine.Object) null || !hand.otherHand.currentAttachedObjectInfo.HasValue || (UnityEngine.Object) hand.otherHand.currentAttachedObjectInfo.Value.interactable != (UnityEngine.Object) null && hand.otherHand.currentAttachedObjectInfo.Value.interactable.activateActionSetOnAttach != this.activateActionSetOnAttach))
        this.activateActionSetOnAttach.Deactivate(hand.handType);
      if (this.onDetachedFromHand != null)
        this.onDetachedFromHand(hand);
      if ((UnityEngine.Object) this.skeletonPoser != (UnityEngine.Object) null && (UnityEngine.Object) hand.skeleton != (UnityEngine.Object) null)
        hand.skeleton.BlendToSkeleton(this.releasePoseBlendTime);
      this.attachedToHand = (Hand) null;
    }

    protected virtual void OnDestroy()
    {
      this.isDestroying = true;
      if ((UnityEngine.Object) this.attachedToHand != (UnityEngine.Object) null)
      {
        this.attachedToHand.DetachObject(this.gameObject, false);
        this.attachedToHand.skeleton.BlendToSkeleton();
      }
      if (!((UnityEngine.Object) this.highlightHolder != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.highlightHolder);
    }

    protected virtual void OnDisable()
    {
      this.isDestroying = true;
      if ((UnityEngine.Object) this.attachedToHand != (UnityEngine.Object) null)
        this.attachedToHand.ForceHoverUnlock();
      if (!((UnityEngine.Object) this.highlightHolder != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.highlightHolder);
    }

    public delegate void OnAttachedToHandDelegate(Hand hand);

    public delegate void OnDetachedFromHandDelegate(Hand hand);
  }
}
