// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Player
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class Player : MonoBehaviour
  {
    [Tooltip("Virtual transform corresponding to the meatspace tracking origin. Devices are tracked relative to this.")]
    public Transform trackingOriginTransform;
    [Tooltip("List of possible transforms for the head/HMD, including the no-SteamVR fallback camera.")]
    public Transform[] hmdTransforms;
    [Tooltip("List of possible Hands, including no-SteamVR fallback Hands.")]
    public Hand[] hands;
    [Tooltip("Reference to the physics collider that follows the player's HMD position.")]
    public Collider headCollider;
    [Tooltip("These objects are enabled when SteamVR is available")]
    public GameObject rigSteamVR;
    [Tooltip("These objects are enabled when SteamVR is not available, or when the user toggles out of VR")]
    public GameObject rig2DFallback;
    [Tooltip("The audio listener for this player")]
    public Transform audioListener;
    [Tooltip("This action lets you know when the player has placed the headset on their head")]
    public SteamVR_Action_Boolean headsetOnHead = SteamVR_Input.GetBooleanAction("HeadsetOnHead");
    public bool allowToggleTo2D = true;
    private static Player _instance;

    public static Player instance
    {
      get
      {
        if ((Object) Player._instance == (Object) null)
          Player._instance = Object.FindObjectOfType<Player>();
        return Player._instance;
      }
    }

    public int handCount
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.hands.Length; ++index)
        {
          if (this.hands[index].gameObject.activeInHierarchy)
            ++num;
        }
        return num;
      }
    }

    public Hand GetHand(int i)
    {
      for (int index = 0; index < this.hands.Length; ++index)
      {
        if (this.hands[index].gameObject.activeInHierarchy)
        {
          if (i <= 0)
            return this.hands[index];
          --i;
        }
      }
      return (Hand) null;
    }

    public Hand leftHand
    {
      get
      {
        for (int index = 0; index < this.hands.Length; ++index)
        {
          if (this.hands[index].gameObject.activeInHierarchy && this.hands[index].handType == SteamVR_Input_Sources.LeftHand)
            return this.hands[index];
        }
        return (Hand) null;
      }
    }

    public Hand rightHand
    {
      get
      {
        for (int index = 0; index < this.hands.Length; ++index)
        {
          if (this.hands[index].gameObject.activeInHierarchy && this.hands[index].handType == SteamVR_Input_Sources.RightHand)
            return this.hands[index];
        }
        return (Hand) null;
      }
    }

    public float scale => this.transform.lossyScale.x;

    public Transform hmdTransform
    {
      get
      {
        if (this.hmdTransforms != null)
        {
          for (int index = 0; index < this.hmdTransforms.Length; ++index)
          {
            if (this.hmdTransforms[index].gameObject.activeInHierarchy)
              return this.hmdTransforms[index];
          }
        }
        return (Transform) null;
      }
    }

    public float eyeHeight
    {
      get
      {
        Transform hmdTransform = this.hmdTransform;
        return (bool) (Object) hmdTransform ? Vector3.Project(hmdTransform.position - this.trackingOriginTransform.position, this.trackingOriginTransform.up).magnitude / this.trackingOriginTransform.lossyScale.x : 0.0f;
      }
    }

    public Vector3 feetPositionGuess
    {
      get
      {
        Transform hmdTransform = this.hmdTransform;
        return (bool) (Object) hmdTransform ? this.trackingOriginTransform.position + Vector3.ProjectOnPlane(hmdTransform.position - this.trackingOriginTransform.position, this.trackingOriginTransform.up) : this.trackingOriginTransform.position;
      }
    }

    public Vector3 bodyDirectionGuess
    {
      get
      {
        Transform hmdTransform = this.hmdTransform;
        if (!(bool) (Object) hmdTransform)
          return this.trackingOriginTransform.forward;
        Vector3 vector3 = Vector3.ProjectOnPlane(hmdTransform.forward, this.trackingOriginTransform.up);
        if ((double) Vector3.Dot(hmdTransform.up, this.trackingOriginTransform.up) < 0.0)
          vector3 = -vector3;
        return vector3;
      }
    }

    private void Awake()
    {
      if (!((Object) this.trackingOriginTransform == (Object) null))
        return;
      this.trackingOriginTransform = this.transform;
    }

    [DebuggerHidden]
    private IEnumerator Start() => (IEnumerator) new Player.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    protected virtual void Update()
    {
      if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess || !((SteamVR_Action) this.headsetOnHead != (SteamVR_Action) null))
        return;
      if (this.headsetOnHead.GetStateDown(SteamVR_Input_Sources.Head))
      {
        UnityEngine.Debug.Log((object) "<b>SteamVR Interaction System</b> Headset placed on head");
      }
      else
      {
        if (!this.headsetOnHead.GetStateUp(SteamVR_Input_Sources.Head))
          return;
        UnityEngine.Debug.Log((object) "<b>SteamVR Interaction System</b> Headset removed");
      }
    }

    private void OnDrawGizmos()
    {
      if ((Object) this != (Object) Player.instance)
        return;
      Gizmos.color = Color.white;
      Gizmos.DrawIcon(this.feetPositionGuess, "vr_interaction_system_feet.png");
      Gizmos.color = Color.cyan;
      Gizmos.DrawLine(this.feetPositionGuess, this.feetPositionGuess + this.trackingOriginTransform.up * this.eyeHeight);
      Gizmos.color = Color.blue;
      Vector3 bodyDirectionGuess = this.bodyDirectionGuess;
      Vector3 vector3_1 = Vector3.Cross(this.trackingOriginTransform.up, bodyDirectionGuess);
      Vector3 from = this.feetPositionGuess + this.trackingOriginTransform.up * this.eyeHeight * 0.75f;
      Vector3 vector3_2 = from + bodyDirectionGuess * 0.33f;
      Gizmos.DrawLine(from, vector3_2);
      Gizmos.DrawLine(vector3_2, vector3_2 - 0.033f * (bodyDirectionGuess + vector3_1));
      Gizmos.DrawLine(vector3_2, vector3_2 - 0.033f * (bodyDirectionGuess - vector3_1));
      Gizmos.color = Color.red;
      int handCount = this.handCount;
      for (int i = 0; i < handCount; ++i)
      {
        Hand hand = this.GetHand(i);
        if (hand.handType == SteamVR_Input_Sources.LeftHand)
          Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_left_hand.png");
        else if (hand.handType == SteamVR_Input_Sources.RightHand)
          Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_right_hand.png");
      }
    }

    public void Draw2DDebug()
    {
      if (!this.allowToggleTo2D || !SteamVR.active)
        return;
      int num1 = 100;
      int num2 = 25;
      int num3 = Screen.width / 2 - num1 / 2;
      int num4 = Screen.height - num2 - 10;
      string text = !this.rigSteamVR.activeSelf ? "VR" : "2D Debug";
      if (!GUI.Button(new Rect((float) num3, (float) num4, (float) num1, (float) num2), text))
        return;
      if (this.rigSteamVR.activeSelf)
        this.ActivateRig(this.rig2DFallback);
      else
        this.ActivateRig(this.rigSteamVR);
    }

    private void ActivateRig(GameObject rig)
    {
      this.rigSteamVR.SetActive((Object) rig == (Object) this.rigSteamVR);
      this.rig2DFallback.SetActive((Object) rig == (Object) this.rig2DFallback);
      if (!(bool) (Object) this.audioListener)
        return;
      this.audioListener.transform.parent = this.hmdTransform;
      this.audioListener.transform.localPosition = Vector3.zero;
      this.audioListener.transform.localRotation = Quaternion.identity;
    }

    public void PlayerShotSelf()
    {
    }
  }
}
