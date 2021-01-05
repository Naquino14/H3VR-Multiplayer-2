// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.RenderModel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class RenderModel : MonoBehaviour
  {
    public GameObject handPrefab;
    protected GameObject handInstance;
    protected Renderer[] handRenderers;
    public bool displayHandByDefault = true;
    protected SteamVR_Behaviour_Skeleton handSkeleton;
    protected Animator handAnimator;
    protected string animatorParameterStateName = "AnimationState";
    protected int handAnimatorStateId = -1;
    [Space]
    public GameObject controllerPrefab;
    protected GameObject controllerInstance;
    protected Renderer[] controllerRenderers;
    protected SteamVR_RenderModel controllerRenderModel;
    public bool displayControllerByDefault = true;
    protected Material delayedSetMaterial;
    protected SteamVR_Events.Action renderModelLoadedAction;
    protected SteamVR_Input_Sources inputSource;

    public event System.Action onControllerLoaded;

    protected void Awake()
    {
      this.renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(new UnityAction<SteamVR_RenderModel, bool>(this.OnRenderModelLoaded));
      this.InitializeHand();
      this.InitializeController();
    }

    protected void InitializeHand()
    {
      if (!((UnityEngine.Object) this.handPrefab != (UnityEngine.Object) null))
        return;
      this.handInstance = UnityEngine.Object.Instantiate<GameObject>(this.handPrefab);
      this.handInstance.transform.parent = this.transform;
      this.handInstance.transform.localPosition = Vector3.zero;
      this.handInstance.transform.localRotation = Quaternion.identity;
      this.handInstance.transform.localScale = this.handPrefab.transform.localScale;
      this.handSkeleton = this.handInstance.GetComponent<SteamVR_Behaviour_Skeleton>();
      this.handSkeleton.origin = Player.instance.trackingOriginTransform;
      this.handSkeleton.updatePose = false;
      this.handSkeleton.skeletonAction.onActiveChange += new SteamVR_Action_Skeleton.ActiveChangeHandler(this.OnSkeletonActiveChange);
      this.handRenderers = this.handInstance.GetComponentsInChildren<Renderer>();
      if (!this.displayHandByDefault)
        this.SetHandVisibility(false);
      this.handAnimator = this.handInstance.GetComponentInChildren<Animator>();
      if (this.handSkeleton.skeletonAction.activeBinding)
        return;
      Debug.LogWarning((object) ("Skeleton action: " + this.handSkeleton.skeletonAction.GetPath() + " is not bound. Your controller may not support SteamVR Skeleton Input."));
      this.DestroyHand();
    }

    protected void InitializeController()
    {
      if (!((UnityEngine.Object) this.controllerPrefab != (UnityEngine.Object) null))
        return;
      this.controllerInstance = UnityEngine.Object.Instantiate<GameObject>(this.controllerPrefab);
      this.controllerInstance.transform.parent = this.transform;
      this.controllerInstance.transform.localPosition = Vector3.zero;
      this.controllerInstance.transform.localRotation = Quaternion.identity;
      this.controllerInstance.transform.localScale = this.controllerPrefab.transform.localScale;
      this.controllerRenderModel = this.controllerInstance.GetComponent<SteamVR_RenderModel>();
    }

    protected virtual void DestroyHand()
    {
      if ((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null)
        this.handSkeleton.skeletonAction.onActiveChange -= new SteamVR_Action_Skeleton.ActiveChangeHandler(this.OnSkeletonActiveChange);
      if (!((UnityEngine.Object) this.handInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.handInstance);
      this.handRenderers = (Renderer[]) null;
      this.handInstance = (GameObject) null;
      this.handSkeleton = (SteamVR_Behaviour_Skeleton) null;
      this.handAnimator = (Animator) null;
    }

    protected virtual void OnSkeletonActiveChange(
      SteamVR_Action_Skeleton changedAction,
      bool newState)
    {
      if (newState)
        this.InitializeHand();
      else
        this.DestroyHand();
    }

    protected void OnEnable() => this.renderModelLoadedAction.enabled = true;

    protected void OnDisable() => this.renderModelLoadedAction.enabled = false;

    protected void OnDestroy() => this.DestroyHand();

    public SteamVR_Behaviour_Skeleton GetSkeleton() => this.handSkeleton;

    public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
    {
      this.inputSource = newInputSource;
      if (!((UnityEngine.Object) this.controllerRenderModel != (UnityEngine.Object) null))
        return;
      this.controllerRenderModel.SetInputSource(this.inputSource);
    }

    public virtual void OnHandInitialized(int deviceIndex)
    {
      this.controllerRenderModel.SetInputSource(this.inputSource);
      this.controllerRenderModel.SetDeviceIndex(deviceIndex);
    }

    public void MatchHandToTransform(Transform match)
    {
      if (!((UnityEngine.Object) this.handInstance != (UnityEngine.Object) null))
        return;
      this.handInstance.transform.position = match.transform.position;
      this.handInstance.transform.rotation = match.transform.rotation;
    }

    public void SetHandPosition(Vector3 newPosition)
    {
      if (!((UnityEngine.Object) this.handInstance != (UnityEngine.Object) null))
        return;
      this.handInstance.transform.position = newPosition;
    }

    public void SetHandRotation(Quaternion newRotation)
    {
      if (!((UnityEngine.Object) this.handInstance != (UnityEngine.Object) null))
        return;
      this.handInstance.transform.rotation = newRotation;
    }

    public Vector3 GetHandPosition() => (UnityEngine.Object) this.handInstance != (UnityEngine.Object) null ? this.handInstance.transform.position : Vector3.zero;

    public Quaternion GetHandRotation() => (UnityEngine.Object) this.handInstance != (UnityEngine.Object) null ? this.handInstance.transform.rotation : Quaternion.identity;

    private void OnRenderModelLoaded(SteamVR_RenderModel loadedRenderModel, bool success)
    {
      if (!((UnityEngine.Object) this.controllerRenderModel == (UnityEngine.Object) loadedRenderModel))
        return;
      this.controllerRenderers = this.controllerInstance.GetComponentsInChildren<Renderer>();
      if (!this.displayControllerByDefault)
        this.SetControllerVisibility(false);
      if ((UnityEngine.Object) this.delayedSetMaterial != (UnityEngine.Object) null)
        this.SetControllerMaterial(this.delayedSetMaterial);
      if (this.onControllerLoaded == null)
        return;
      this.onControllerLoaded();
    }

    public void SetVisibility(bool state, bool overrideDefault = false)
    {
      if (!state || this.displayControllerByDefault || overrideDefault)
        this.SetControllerVisibility(state);
      if (state && !this.displayHandByDefault && !overrideDefault)
        return;
      this.SetHandVisibility(state);
    }

    public void Show(bool overrideDefault = false) => this.SetVisibility(true, overrideDefault);

    public void Hide() => this.SetVisibility(false);

    public virtual void SetMaterial(Material material)
    {
      this.SetControllerMaterial(material);
      this.SetHandMaterial(material);
    }

    public void SetControllerMaterial(Material material)
    {
      if (this.controllerRenderers == null)
      {
        this.delayedSetMaterial = material;
      }
      else
      {
        for (int index = 0; index < this.controllerRenderers.Length; ++index)
          this.controllerRenderers[index].material = material;
      }
    }

    public void SetHandMaterial(Material material)
    {
      for (int index = 0; index < this.handRenderers.Length; ++index)
        this.handRenderers[index].material = material;
    }

    public void SetControllerVisibility(bool state, bool permanent = false)
    {
      if (permanent)
        this.displayControllerByDefault = state;
      if (this.controllerRenderers == null)
        return;
      for (int index = 0; index < this.controllerRenderers.Length; ++index)
        this.controllerRenderers[index].enabled = state;
    }

    public void SetHandVisibility(bool state, bool permanent = false)
    {
      if (permanent)
        this.displayHandByDefault = state;
      if (this.handRenderers == null)
        return;
      for (int index = 0; index < this.handRenderers.Length; ++index)
        this.handRenderers[index].enabled = state;
    }

    public bool IsHandVisibile()
    {
      if (this.handRenderers == null)
        return false;
      for (int index = 0; index < this.handRenderers.Length; ++index)
      {
        if (this.handRenderers[index].enabled)
          return true;
      }
      return false;
    }

    public bool IsControllerVisibile()
    {
      if (this.controllerRenderers == null)
        return false;
      for (int index = 0; index < this.controllerRenderers.Length; ++index)
      {
        if (this.controllerRenderers[index].enabled)
          return true;
      }
      return false;
    }

    public Transform GetBone(int boneIndex) => (UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null ? this.handSkeleton.GetBone(boneIndex) : (Transform) null;

    public Vector3 GetBonePosition(int boneIndex, bool local = false) => (UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null ? this.handSkeleton.GetBonePosition(boneIndex, local) : Vector3.zero;

    public Vector3 GetControllerPosition(string componentName = null) => (UnityEngine.Object) this.controllerRenderModel != (UnityEngine.Object) null ? this.controllerRenderModel.GetComponentTransform(componentName).position : Vector3.zero;

    public Quaternion GetBoneRotation(int boneIndex, bool local = false) => (UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null ? this.handSkeleton.GetBoneRotation(boneIndex, local) : Quaternion.identity;

    public void SetSkeletonRangeOfMotion(
      EVRSkeletalMotionRange newRangeOfMotion,
      float blendOverSeconds = 0.1f)
    {
      if (!((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null))
        return;
      this.handSkeleton.SetRangeOfMotion(newRangeOfMotion, blendOverSeconds);
    }

    public EVRSkeletalMotionRange GetSkeletonRangeOfMotion => (UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null ? this.handSkeleton.rangeOfMotion : EVRSkeletalMotionRange.WithController;

    public void SetTemporarySkeletonRangeOfMotion(
      SkeletalMotionRangeChange temporaryRangeOfMotionChange,
      float blendOverSeconds = 0.1f)
    {
      if (!((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null))
        return;
      this.handSkeleton.SetTemporaryRangeOfMotion((EVRSkeletalMotionRange) temporaryRangeOfMotionChange, blendOverSeconds);
    }

    public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
    {
      if (!((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null))
        return;
      this.handSkeleton.ResetTemporaryRangeOfMotion(blendOverSeconds);
    }

    public void SetAnimationState(int stateValue)
    {
      if (!((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null))
        return;
      if (!this.handSkeleton.isBlending)
        this.handSkeleton.BlendToAnimation();
      if (!this.CheckAnimatorInit())
        return;
      this.handAnimator.SetInteger(this.handAnimatorStateId, stateValue);
    }

    public void StopAnimation()
    {
      if (!((UnityEngine.Object) this.handSkeleton != (UnityEngine.Object) null))
        return;
      if (!this.handSkeleton.isBlending)
        this.handSkeleton.BlendToSkeleton();
      if (!this.CheckAnimatorInit())
        return;
      this.handAnimator.SetInteger(this.handAnimatorStateId, 0);
    }

    private bool CheckAnimatorInit()
    {
      if (this.handAnimatorStateId == -1 && (UnityEngine.Object) this.handAnimator != (UnityEngine.Object) null && (this.handAnimator.gameObject.activeInHierarchy && this.handAnimator.isInitialized))
      {
        AnimatorControllerParameter[] parameters = this.handAnimator.parameters;
        for (int index = 0; index < parameters.Length; ++index)
        {
          if (string.Equals(parameters[index].name, this.animatorParameterStateName, StringComparison.CurrentCultureIgnoreCase))
            this.handAnimatorStateId = parameters[index].nameHash;
        }
      }
      return this.handAnimatorStateId != -1 && (UnityEngine.Object) this.handAnimator != (UnityEngine.Object) null && this.handAnimator.isInitialized;
    }
  }
}
