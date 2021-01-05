// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ArrowHand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class ArrowHand : MonoBehaviour
  {
    private Hand hand;
    private Longbow bow;
    private GameObject currentArrow;
    public GameObject arrowPrefab;
    public Transform arrowNockTransform;
    public float nockDistance = 0.1f;
    public float lerpCompleteDistance = 0.08f;
    public float rotationLerpThreshold = 0.15f;
    public float positionLerpThreshold = 0.15f;
    private bool allowArrowSpawn = true;
    private bool nocked;
    private GrabTypes nockedWithType;
    private bool inNockRange;
    private bool arrowLerpComplete;
    public SoundPlayOneshot arrowSpawnSound;
    private AllowTeleportWhileAttachedToHand allowTeleport;
    public int maxArrowCount = 10;
    private List<GameObject> arrowList;

    private void Awake()
    {
      this.allowTeleport = this.GetComponent<AllowTeleportWhileAttachedToHand>();
      this.allowTeleport.overrideHoverLock = false;
      this.arrowList = new List<GameObject>();
    }

    private void OnAttachedToHand(Hand attachedHand)
    {
      this.hand = attachedHand;
      this.FindBow();
    }

    private GameObject InstantiateArrow()
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.arrowPrefab, this.arrowNockTransform.position, this.arrowNockTransform.rotation);
      gameObject.name = "Bow Arrow";
      gameObject.transform.parent = this.arrowNockTransform;
      Util.ResetTransform(gameObject.transform);
      this.arrowList.Add(gameObject);
      while (this.arrowList.Count > this.maxArrowCount)
      {
        GameObject arrow = this.arrowList[0];
        this.arrowList.RemoveAt(0);
        if ((bool) (Object) arrow)
          Object.Destroy((Object) arrow);
      }
      return gameObject;
    }

    private void HandAttachedUpdate(Hand hand)
    {
      if ((Object) this.bow == (Object) null)
        this.FindBow();
      if ((Object) this.bow == (Object) null)
        return;
      if (this.allowArrowSpawn && (Object) this.currentArrow == (Object) null)
      {
        this.currentArrow = this.InstantiateArrow();
        this.arrowSpawnSound.Play();
      }
      float num = Vector3.Distance(this.transform.parent.position, this.bow.nockTransform.position);
      if (!this.nocked)
      {
        if ((double) num < (double) this.rotationLerpThreshold)
          this.arrowNockTransform.rotation = Quaternion.Lerp(this.arrowNockTransform.parent.rotation, this.bow.nockRestTransform.rotation, Util.RemapNumber(num, this.rotationLerpThreshold, this.lerpCompleteDistance, 0.0f, 1f));
        else
          this.arrowNockTransform.localRotation = Quaternion.identity;
        this.arrowNockTransform.position = (double) num >= (double) this.positionLerpThreshold ? this.arrowNockTransform.parent.position : Vector3.Lerp(this.arrowNockTransform.parent.position, this.bow.nockRestTransform.position, Mathf.Clamp(Util.RemapNumber(num, this.positionLerpThreshold, this.lerpCompleteDistance, 0.0f, 1f), 0.0f, 1f));
        if ((double) num < (double) this.lerpCompleteDistance)
        {
          if (!this.arrowLerpComplete)
          {
            this.arrowLerpComplete = true;
            hand.TriggerHapticPulse((ushort) 500);
          }
        }
        else if (this.arrowLerpComplete)
          this.arrowLerpComplete = false;
        if ((double) num < (double) this.nockDistance)
        {
          if (!this.inNockRange)
          {
            this.inNockRange = true;
            this.bow.ArrowInPosition();
          }
        }
        else if (this.inNockRange)
          this.inNockRange = false;
        GrabTypes bestGrabbingType = hand.GetBestGrabbingType(GrabTypes.Pinch, true);
        if ((double) num < (double) this.nockDistance && bestGrabbingType != GrabTypes.None && !this.nocked)
        {
          if ((Object) this.currentArrow == (Object) null)
            this.currentArrow = this.InstantiateArrow();
          this.nocked = true;
          this.nockedWithType = bestGrabbingType;
          this.bow.StartNock(this);
          hand.HoverLock(this.GetComponent<Interactable>());
          this.allowTeleport.teleportAllowed = false;
          this.currentArrow.transform.parent = this.bow.nockTransform;
          Util.ResetTransform(this.currentArrow.transform);
          Util.ResetTransform(this.arrowNockTransform);
        }
      }
      if (!this.nocked || hand.IsGrabbingWithType(this.nockedWithType))
        return;
      if (this.bow.pulled)
      {
        this.FireArrow();
      }
      else
      {
        this.arrowNockTransform.rotation = this.currentArrow.transform.rotation;
        this.currentArrow.transform.parent = this.arrowNockTransform;
        Util.ResetTransform(this.currentArrow.transform);
        this.nocked = false;
        this.nockedWithType = GrabTypes.None;
        this.bow.ReleaseNock();
        hand.HoverUnlock(this.GetComponent<Interactable>());
        this.allowTeleport.teleportAllowed = true;
      }
      this.bow.StartRotationLerp();
    }

    private void OnDetachedFromHand(Hand hand) => Object.Destroy((Object) this.gameObject);

    private void FireArrow()
    {
      this.currentArrow.transform.parent = (Transform) null;
      Arrow component = this.currentArrow.GetComponent<Arrow>();
      component.shaftRB.isKinematic = false;
      component.shaftRB.useGravity = true;
      component.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;
      component.arrowHeadRB.isKinematic = false;
      component.arrowHeadRB.useGravity = true;
      component.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;
      component.arrowHeadRB.AddForce(this.currentArrow.transform.forward * this.bow.GetArrowVelocity(), ForceMode.VelocityChange);
      component.arrowHeadRB.AddTorque(this.currentArrow.transform.forward * 10f);
      this.nocked = false;
      this.nockedWithType = GrabTypes.None;
      this.currentArrow.GetComponent<Arrow>().ArrowReleased(this.bow.GetArrowVelocity());
      this.bow.ArrowReleased();
      this.allowArrowSpawn = false;
      this.Invoke("EnableArrowSpawn", 0.5f);
      this.StartCoroutine(this.ArrowReleaseHaptics());
      this.currentArrow = (GameObject) null;
      this.allowTeleport.teleportAllowed = true;
    }

    private void EnableArrowSpawn() => this.allowArrowSpawn = true;

    [DebuggerHidden]
    private IEnumerator ArrowReleaseHaptics() => (IEnumerator) new ArrowHand.\u003CArrowReleaseHaptics\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void OnHandFocusLost(Hand hand) => this.gameObject.SetActive(false);

    private void OnHandFocusAcquired(Hand hand) => this.gameObject.SetActive(true);

    private void FindBow() => this.bow = this.hand.otherHand.GetComponentInChildren<Longbow>();
  }
}
