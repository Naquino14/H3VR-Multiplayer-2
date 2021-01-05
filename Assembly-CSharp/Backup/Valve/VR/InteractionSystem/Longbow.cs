// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Longbow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class Longbow : MonoBehaviour
  {
    public Longbow.Handedness currentHandGuess;
    private float timeOfPossibleHandSwitch;
    private float timeBeforeConfirmingHandSwitch = 1.5f;
    private bool possibleHandSwitch;
    public Transform pivotTransform;
    public Transform handleTransform;
    private Hand hand;
    private ArrowHand arrowHand;
    public Transform nockTransform;
    public Transform nockRestTransform;
    public bool autoSpawnArrowHand = true;
    public ItemPackage arrowHandItemPackage;
    public GameObject arrowHandPrefab;
    public bool nocked;
    public bool pulled;
    private const float minPull = 0.05f;
    private const float maxPull = 0.5f;
    private float nockDistanceTravelled;
    private float hapticDistanceThreshold = 0.01f;
    private float lastTickDistance;
    private const float bowPullPulseStrengthLow = 100f;
    private const float bowPullPulseStrengthHigh = 500f;
    private Vector3 bowLeftVector;
    public float arrowMinVelocity = 3f;
    public float arrowMaxVelocity = 30f;
    private float arrowVelocity = 30f;
    private float minStrainTickTime = 0.1f;
    private float maxStrainTickTime = 0.5f;
    private float nextStrainTick;
    private bool lerpBackToZeroRotation;
    private float lerpStartTime;
    private float lerpDuration = 0.15f;
    private Quaternion lerpStartRotation;
    private float nockLerpStartTime;
    private Quaternion nockLerpStartRotation;
    public float drawOffset = 0.06f;
    public LinearMapping bowDrawLinearMapping;
    private Vector3 lateUpdatePos;
    private Quaternion lateUpdateRot;
    public SoundBowClick drawSound;
    private float drawTension;
    public SoundPlayOneshot arrowSlideSound;
    public SoundPlayOneshot releaseSound;
    public SoundPlayOneshot nockSound;
    private SteamVR_Events.Action newPosesAppliedAction;

    private void OnAttachedToHand(Hand attachedHand) => this.hand = attachedHand;

    private void HandAttachedUpdate(Hand hand)
    {
      this.EvaluateHandedness();
      if (this.nocked)
      {
        Vector3 lhs = this.arrowHand.arrowNockTransform.parent.position - this.nockRestTransform.position;
        float t = Util.RemapNumberClamped(Time.time, this.nockLerpStartTime, this.nockLerpStartTime + this.lerpDuration, 0.0f, 1f);
        Vector3 normalized = (this.arrowHand.arrowNockTransform.parent.position + (Player.instance.hmdTransform.position + Vector3.down * 0.05f - this.arrowHand.arrowNockTransform.parent.position).normalized * this.drawOffset * Util.RemapNumberClamped(lhs.magnitude, 0.05f, 0.5f, 0.0f, 1f) - this.pivotTransform.position).normalized;
        this.bowLeftVector = -Vector3.Cross((this.handleTransform.position - this.pivotTransform.position).normalized, normalized);
        this.pivotTransform.rotation = Quaternion.Lerp(this.nockLerpStartRotation, Quaternion.LookRotation(normalized, this.bowLeftVector), t);
        if ((double) Vector3.Dot(lhs, -this.nockTransform.forward) > 0.0)
        {
          this.nockTransform.localPosition = new Vector3(0.0f, 0.0f, Mathf.Clamp(-(lhs.magnitude * t), -0.5f, 0.0f));
          this.nockDistanceTravelled = -this.nockTransform.localPosition.z;
          this.arrowVelocity = Util.RemapNumber(this.nockDistanceTravelled, 0.05f, 0.5f, this.arrowMinVelocity, this.arrowMaxVelocity);
          this.drawTension = Util.RemapNumberClamped(this.nockDistanceTravelled, 0.0f, 0.5f, 0.0f, 1f);
          this.bowDrawLinearMapping.value = this.drawTension;
          this.pulled = (double) this.nockDistanceTravelled > 0.0500000007450581;
          if ((double) this.nockDistanceTravelled > (double) this.lastTickDistance + (double) this.hapticDistanceThreshold || (double) this.nockDistanceTravelled < (double) this.lastTickDistance - (double) this.hapticDistanceThreshold)
          {
            ushort microSecondsDuration = (ushort) Util.RemapNumber(this.nockDistanceTravelled, 0.0f, 0.5f, 100f, 500f);
            hand.TriggerHapticPulse(microSecondsDuration);
            hand.otherHand.TriggerHapticPulse(microSecondsDuration);
            this.drawSound.PlayBowTensionClicks(this.drawTension);
            this.lastTickDistance = this.nockDistanceTravelled;
          }
          if ((double) this.nockDistanceTravelled < 0.5 || (double) Time.time <= (double) this.nextStrainTick)
            return;
          hand.TriggerHapticPulse((ushort) 400);
          hand.otherHand.TriggerHapticPulse((ushort) 400);
          this.drawSound.PlayBowTensionClicks(this.drawTension);
          this.nextStrainTick = Time.time + Random.Range(this.minStrainTickTime, this.maxStrainTickTime);
        }
        else
        {
          this.nockTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
          this.bowDrawLinearMapping.value = 0.0f;
        }
      }
      else
      {
        if (!this.lerpBackToZeroRotation)
          return;
        float t = Util.RemapNumber(Time.time, this.lerpStartTime, this.lerpStartTime + this.lerpDuration, 0.0f, 1f);
        this.pivotTransform.localRotation = Quaternion.Lerp(this.lerpStartRotation, Quaternion.identity, t);
        if ((double) t < 1.0)
          return;
        this.lerpBackToZeroRotation = false;
      }
    }

    public void ArrowReleased()
    {
      this.nocked = false;
      this.hand.HoverUnlock(this.GetComponent<Interactable>());
      this.hand.otherHand.HoverUnlock(this.arrowHand.GetComponent<Interactable>());
      if ((Object) this.releaseSound != (Object) null)
        this.releaseSound.Play();
      this.StartCoroutine(this.ResetDrawAnim());
    }

    [DebuggerHidden]
    private IEnumerator ResetDrawAnim() => (IEnumerator) new Longbow.\u003CResetDrawAnim\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public float GetArrowVelocity() => this.arrowVelocity;

    public void StartRotationLerp()
    {
      this.lerpStartTime = Time.time;
      this.lerpBackToZeroRotation = true;
      this.lerpStartRotation = this.pivotTransform.localRotation;
      Util.ResetTransform(this.nockTransform);
    }

    public void StartNock(ArrowHand currentArrowHand)
    {
      this.arrowHand = currentArrowHand;
      this.hand.HoverLock(this.GetComponent<Interactable>());
      this.nocked = true;
      this.nockLerpStartTime = Time.time;
      this.nockLerpStartRotation = this.pivotTransform.rotation;
      this.arrowSlideSound.Play();
      this.DoHandednessCheck();
    }

    private void EvaluateHandedness()
    {
      if (this.hand.handType == SteamVR_Input_Sources.LeftHand)
      {
        if (this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Left)
          this.possibleHandSwitch = false;
        if (!this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Right)
        {
          this.possibleHandSwitch = true;
          this.timeOfPossibleHandSwitch = Time.time;
        }
        if (!this.possibleHandSwitch || (double) Time.time <= (double) this.timeOfPossibleHandSwitch + (double) this.timeBeforeConfirmingHandSwitch)
          return;
        this.currentHandGuess = Longbow.Handedness.Left;
        this.possibleHandSwitch = false;
      }
      else
      {
        if (this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Right)
          this.possibleHandSwitch = false;
        if (!this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Left)
        {
          this.possibleHandSwitch = true;
          this.timeOfPossibleHandSwitch = Time.time;
        }
        if (!this.possibleHandSwitch || (double) Time.time <= (double) this.timeOfPossibleHandSwitch + (double) this.timeBeforeConfirmingHandSwitch)
          return;
        this.currentHandGuess = Longbow.Handedness.Right;
        this.possibleHandSwitch = false;
      }
    }

    private void DoHandednessCheck()
    {
      if (this.currentHandGuess == Longbow.Handedness.Left)
        this.pivotTransform.localScale = new Vector3(1f, 1f, 1f);
      else
        this.pivotTransform.localScale = new Vector3(1f, -1f, 1f);
    }

    public void ArrowInPosition()
    {
      this.DoHandednessCheck();
      if (!((Object) this.nockSound != (Object) null))
        return;
      this.nockSound.Play();
    }

    public void ReleaseNock()
    {
      this.nocked = false;
      this.hand.HoverUnlock(this.GetComponent<Interactable>());
      this.StartCoroutine(this.ResetDrawAnim());
    }

    private void ShutDown()
    {
      if (!((Object) this.hand != (Object) null) || !((Object) this.hand.otherHand.currentAttachedObject != (Object) null) || (!((Object) this.hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>() != (Object) null) || !((Object) this.hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>().itemPackage == (Object) this.arrowHandItemPackage)))
        return;
      this.hand.otherHand.DetachObject(this.hand.otherHand.currentAttachedObject);
    }

    private void OnHandFocusLost(Hand hand) => this.gameObject.SetActive(false);

    private void OnHandFocusAcquired(Hand hand)
    {
      this.gameObject.SetActive(true);
      this.OnAttachedToHand(hand);
    }

    private void OnDetachedFromHand(Hand hand) => Object.Destroy((Object) this.gameObject);

    private void OnDestroy() => this.ShutDown();

    public enum Handedness
    {
      Left,
      Right,
    }
  }
}
