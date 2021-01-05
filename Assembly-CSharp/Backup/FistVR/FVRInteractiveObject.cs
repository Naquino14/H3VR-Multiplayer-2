// Decompiled with JetBrains decompiler
// Type: FistVR.FVRInteractiveObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRInteractiveObject : MonoBehaviour
  {
    [NonSerialized]
    public GameObject GameObject;
    [NonSerialized]
    public Transform Transform;
    [NonSerialized]
    private int m_index = -1;
    public static List<FVRInteractiveObject> All = new List<FVRInteractiveObject>();
    [Header("Interactive Object Config")]
    public FVRInteractionControlType ControlType;
    public bool IsSimpleInteract;
    public HandlingGrabType HandlingGrabSound;
    public HandlingReleaseType HandlingReleaseSound;
    public Transform PoseOverride;
    public Transform QBPoseOverride;
    public Transform PoseOverride_Touch;
    public bool UseGrabPointChild;
    public bool UseGripRotInterp;
    public float PositionInterpSpeed = 1f;
    public float RotationInterpSpeed = 1f;
    protected Transform m_grabPointTransform;
    protected float m_pos_interp_tick;
    protected float m_rot_interp_tick;
    public bool EndInteractionIfDistant = true;
    public float EndInteractionDistance = 0.25f;
    [HideInInspector]
    public bool m_hasTriggeredUpSinceBegin;
    protected float triggerCooldown = 0.5f;
    public FVRViveHand m_hand;
    public GameObject UXGeo_Hover;
    public GameObject UXGeo_Held;
    public bool UseFilteredHandTransform;
    public bool UseFilteredHandPosition;
    public bool UseFilteredHandRotation;
    public bool UseSecondStepRotationFiltering;
    protected Quaternion SecondStepFilteredRotation = Quaternion.identity;
    private bool m_isHovered;
    private bool m_isHeld;
    protected Collider[] m_colliders;

    public Transform GrabPointTransform
    {
      get => this.m_grabPointTransform;
      set => this.m_grabPointTransform = value;
    }

    [HideInInspector]
    public Vector3 m_handPos => this.UseFilteredHandTransform || this.UseFilteredHandPosition ? this.m_hand.Input.FilteredPos : this.m_hand.Input.Pos;

    [HideInInspector]
    public Quaternion m_handRot => this.UseFilteredHandTransform || this.UseFilteredHandRotation ? this.m_hand.Input.FilteredRot : this.m_hand.Input.Rot;

    [HideInInspector]
    public Vector3 m_palmPos => this.UseFilteredHandTransform || this.UseFilteredHandPosition ? this.m_hand.Input.FilteredPalmPos : this.m_hand.Input.PalmPos;

    [HideInInspector]
    public Quaternion m_palmRot => this.UseFilteredHandTransform || this.UseFilteredHandRotation ? this.m_hand.Input.FilteredPalmRot : this.m_hand.Input.PalmRot;

    public bool IsHovered
    {
      get => this.m_isHovered;
      set
      {
        bool isHovered = this.m_isHovered;
        if (value)
          this.SetUXHoverGeoViz(true);
        else
          this.SetUXHoverGeoViz(false);
        this.m_isHovered = value;
        if (this.m_isHovered && !isHovered)
        {
          this.OnHoverStart();
        }
        else
        {
          if (!this.m_isHovered || !isHovered)
            return;
          this.OnHoverEnd();
        }
      }
    }

    public bool IsHeld
    {
      get => this.m_isHeld;
      set
      {
        bool isHeld = this.m_isHeld;
        if (value)
          this.SetUXHeldGeoViz(true);
        else
          this.SetUXHeldGeoViz(false);
        this.m_isHeld = value;
        if (isHeld || !this.m_isHeld || (!(this is FVRPhysicalObject) || !((UnityEngine.Object) (this as FVRPhysicalObject).ObjectWrapper != (UnityEngine.Object) null)))
          return;
        GM.CurrentSceneSettings.OnFVRObjectPickedUp(this as FVRPhysicalObject);
      }
    }

    public virtual bool IsSelectionRestricted() => false;

    public virtual bool IsInteractable() => true;

    public virtual bool IsDistantGrabbable() => false;

    protected virtual void OnHoverStart()
    {
    }

    protected virtual void OnHoverEnd()
    {
    }

    protected virtual void OnHoverStay()
    {
    }

    protected virtual void SetUXHoverGeoViz(bool b)
    {
      if (!((UnityEngine.Object) this.UXGeo_Hover != (UnityEngine.Object) null))
        return;
      this.UXGeo_Hover.SetActive(b);
    }

    protected virtual void SetUXHeldGeoViz(bool b)
    {
      if (!((UnityEngine.Object) this.UXGeo_Held != (UnityEngine.Object) null))
        return;
      this.UXGeo_Held.SetActive(b);
    }

    public void UpdateGrabPointTransform()
    {
      if (!((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null))
        return;
      this.m_grabPointTransform.position = this.m_handPos;
      this.m_grabPointTransform.rotation = this.m_handRot;
    }

    public virtual void PlayGrabSound(bool isHard, FVRViveHand hand)
    {
      if (this.HandlingGrabSound == HandlingGrabType.None || !hand.CanMakeGrabReleaseSound)
        return;
      SM.PlayHandlingGrabSound(this.HandlingGrabSound, hand.Input.Pos, isHard);
      hand.HandMadeGrabReleaseSound();
    }

    public virtual void PlayReleaseSound(FVRViveHand hand)
    {
      if (this.HandlingReleaseSound == HandlingReleaseType.None || !hand.CanMakeGrabReleaseSound)
        return;
      SM.PlayHandlingReleaseSound(this.HandlingReleaseSound, hand.Input.Pos);
      hand.HandMadeGrabReleaseSound();
    }

    public virtual void BeginInteraction(FVRViveHand hand)
    {
      this.PlayGrabSound(!this.IsHeld, hand);
      if (this.IsHeld && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) hand && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        this.m_hand.EndInteractionIfHeld(this);
      this.m_hasTriggeredUpSinceBegin = false;
      this.IsHeld = true;
      this.m_hand = hand;
      this.triggerCooldown = 0.5f;
      if ((UnityEngine.Object) this.m_grabPointTransform == (UnityEngine.Object) null)
        this.m_grabPointTransform = new GameObject("interpRot").transform;
      this.m_grabPointTransform.SetParent(this.Transform);
      this.m_grabPointTransform.position = this.m_handPos;
      this.m_grabPointTransform.rotation = this.m_handRot;
      this.m_pos_interp_tick = 0.0f;
      this.m_rot_interp_tick = 0.0f;
    }

    public virtual void UpdateInteraction(FVRViveHand hand)
    {
      this.IsHeld = true;
      this.m_hand = hand;
      if (!this.m_hasTriggeredUpSinceBegin && (double) this.m_hand.Input.TriggerFloat < 0.150000005960464)
        this.m_hasTriggeredUpSinceBegin = true;
      if ((double) this.triggerCooldown <= 0.0)
        return;
      this.triggerCooldown -= Time.deltaTime;
    }

    public virtual void EndInteraction(FVRViveHand hand)
    {
      this.m_hasTriggeredUpSinceBegin = false;
      this.m_hand = (FVRViveHand) null;
      this.IsHeld = false;
      this.PlayReleaseSound(hand);
    }

    public virtual void SimpleInteraction(FVRViveHand hand)
    {
    }

    public virtual void Poke(FVRViveHand hand)
    {
    }

    public virtual void ForceBreakInteraction()
    {
      if (!((UnityEngine.Object) this.m_hand != (UnityEngine.Object) null))
        return;
      this.m_hand.EndInteractionIfHeld(this);
      this.EndInteraction(this.m_hand);
    }

    protected virtual void Awake()
    {
      this.m_index = FVRInteractiveObject.All.Count;
      FVRInteractiveObject.All.Add(this);
      this.GameObject = this.gameObject;
      this.Transform = this.transform;
      this.m_colliders = this.GetComponentsInChildren<Collider>(true);
    }

    protected virtual void Start()
    {
    }

    public static void GlobalUpdate()
    {
      for (int index = 0; index < FVRInteractiveObject.All.Count; ++index)
      {
        if ((UnityEngine.Object) FVRInteractiveObject.All[index] != (UnityEngine.Object) null && FVRInteractiveObject.All[index].enabled && FVRInteractiveObject.All[index].GameObject.activeInHierarchy)
          FVRInteractiveObject.All[index].FVRUpdate();
      }
    }

    protected virtual void FVRUpdate()
    {
      if (!this.IsHovered)
        return;
      this.OnHoverStay();
    }

    public static void GlobalFixedUpdate()
    {
      for (int index = 0; index < FVRInteractiveObject.All.Count; ++index)
      {
        if ((UnityEngine.Object) FVRInteractiveObject.All[index] != (UnityEngine.Object) null && FVRInteractiveObject.All[index].enabled && FVRInteractiveObject.All[index].GameObject.activeInHierarchy)
          FVRInteractiveObject.All[index].FVRFixedUpdate();
      }
    }

    protected virtual void FVRFixedUpdate()
    {
      if (!this.IsHeld)
        return;
      this.TestHandDistance();
    }

    public virtual void OnDestroy()
    {
      if (this.IsHeld && (UnityEngine.Object) this.m_hand != (UnityEngine.Object) null)
        this.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
      if (this.m_index < 0)
        return;
      FVRInteractiveObject.All[this.m_index] = FVRInteractiveObject.All[FVRInteractiveObject.All.Count - 1];
      FVRInteractiveObject.All[this.m_index].m_index = this.m_index;
      FVRInteractiveObject.All.RemoveAt(FVRInteractiveObject.All.Count - 1);
    }

    public virtual void TestHandDistance()
    {
      if (!this.EndInteractionIfDistant)
        return;
      if ((UnityEngine.Object) this.PoseOverride == (UnityEngine.Object) null)
      {
        if ((double) Vector3.Distance(this.m_handPos, this.Transform.position) < (double) this.EndInteractionDistance)
          return;
        this.ForceBreakInteraction();
      }
      else
      {
        if ((double) Vector3.Distance(this.m_handPos, this.PoseOverride.position) < (double) this.EndInteractionDistance)
          return;
        this.ForceBreakInteraction();
      }
    }

    public void SetCollidersToLayer(List<Collider> cols, bool triggersToo, string layerName)
    {
      if (triggersToo)
      {
        foreach (Collider col in cols)
        {
          if ((UnityEngine.Object) col != (UnityEngine.Object) null)
            col.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
      }
      else
      {
        foreach (Collider col in cols)
        {
          if ((UnityEngine.Object) col != (UnityEngine.Object) null && !col.isTrigger)
            col.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
      }
    }

    public void SetAllCollidersToLayer(bool triggersToo, string layerName)
    {
      if (triggersToo)
      {
        foreach (Collider collider in this.m_colliders)
        {
          if ((UnityEngine.Object) collider != (UnityEngine.Object) null)
            collider.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
      }
      else
      {
        foreach (Collider collider in this.m_colliders)
        {
          if ((UnityEngine.Object) collider != (UnityEngine.Object) null && !collider.isTrigger)
            collider.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
      }
    }

    public Vector3 GetClosestValidPoint(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
      Vector3 rhs = vPoint - vA;
      Vector3 normalized = (vB - vA).normalized;
      float num1 = Vector3.Distance(vA, vB);
      float num2 = Vector3.Dot(normalized, rhs);
      if ((double) num2 <= 0.0)
        return vA;
      if ((double) num2 >= (double) num1)
        return vB;
      Vector3 vector3 = normalized * num2;
      return vA + vector3;
    }

    public enum HandFilterMode
    {
      Unfiltered,
      FilterMode_A,
      FilterMode_B,
    }
  }
}
