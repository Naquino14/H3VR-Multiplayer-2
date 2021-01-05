// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.CircularDrive
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class CircularDrive : MonoBehaviour
  {
    [Tooltip("The axis around which the circular drive will rotate in local space")]
    public CircularDrive.Axis_t axisOfRotation;
    [Tooltip("Child GameObject which has the Collider component to initiate interaction, only needs to be set if there is more than one Collider child")]
    public Collider childCollider;
    [Tooltip("A LinearMapping component to drive, if not specified one will be dynamically added to this GameObject")]
    public LinearMapping linearMapping;
    [Tooltip("If true, the drive will stay manipulating as long as the button is held down, if false, it will stop if the controller moves out of the collider")]
    public bool hoverLock;
    [Header("Limited Rotation")]
    [Tooltip("If true, the rotation will be limited to [minAngle, maxAngle], if false, the rotation is unlimited")]
    public bool limited;
    public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);
    public UnityEvent onFrozenDistanceThreshold;
    [Header("Limited Rotation Min")]
    [Tooltip("If limited is true, the specifies the lower limit, otherwise value is unused")]
    public float minAngle = -45f;
    [Tooltip("If limited, set whether drive will freeze its angle when the min angle is reached")]
    public bool freezeOnMin;
    [Tooltip("If limited, event invoked when minAngle is reached")]
    public UnityEvent onMinAngle;
    [Header("Limited Rotation Max")]
    [Tooltip("If limited is true, the specifies the upper limit, otherwise value is unused")]
    public float maxAngle = 45f;
    [Tooltip("If limited, set whether drive will freeze its angle when the max angle is reached")]
    public bool freezeOnMax;
    [Tooltip("If limited, event invoked when maxAngle is reached")]
    public UnityEvent onMaxAngle;
    [Tooltip("If limited is true, this forces the starting angle to be startAngle, clamped to [minAngle, maxAngle]")]
    public bool forceStart;
    [Tooltip("If limited is true and forceStart is true, the starting angle will be this, clamped to [minAngle, maxAngle]")]
    public float startAngle;
    [Tooltip("If true, the transform of the GameObject this component is on will be rotated accordingly")]
    public bool rotateGameObject = true;
    [Tooltip("If true, the path of the Hand (red) and the projected value (green) will be drawn")]
    public bool debugPath;
    [Tooltip("If debugPath is true, this is the maximum number of GameObjects to create to draw the path")]
    public int dbgPathLimit = 50;
    [Tooltip("If not null, the TextMesh will display the linear value and the angular value of this circular drive")]
    public TextMesh debugText;
    [Tooltip("The output angle value of the drive in degrees, unlimited will increase or decrease without bound, take the 360 modulus to find number of rotations")]
    public float outAngle;
    private Quaternion start;
    private Vector3 worldPlaneNormal = new Vector3(1f, 0.0f, 0.0f);
    private Vector3 localPlaneNormal = new Vector3(1f, 0.0f, 0.0f);
    private Vector3 lastHandProjected;
    private Color red = new Color(1f, 0.0f, 0.0f);
    private Color green = new Color(0.0f, 1f, 0.0f);
    private GameObject[] dbgHandObjects;
    private GameObject[] dbgProjObjects;
    private GameObject dbgObjectsParent;
    private int dbgObjectCount;
    private int dbgObjectIndex;
    private bool driving;
    private float minMaxAngularThreshold = 1f;
    private bool frozen;
    private float frozenAngle;
    private Vector3 frozenHandWorldPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 frozenSqDistanceMinMaxThreshold = new Vector2(0.0f, 0.0f);
    private Hand handHoverLocked;
    private Interactable interactable;
    private GrabTypes grabbedWithType;

    private void Freeze(Hand hand)
    {
      this.frozen = true;
      this.frozenAngle = this.outAngle;
      this.frozenHandWorldPos = hand.hoverSphereTransform.position;
      this.frozenSqDistanceMinMaxThreshold.x = this.frozenDistanceMinMaxThreshold.x * this.frozenDistanceMinMaxThreshold.x;
      this.frozenSqDistanceMinMaxThreshold.y = this.frozenDistanceMinMaxThreshold.y * this.frozenDistanceMinMaxThreshold.y;
    }

    private void UnFreeze()
    {
      this.frozen = false;
      this.frozenHandWorldPos.Set(0.0f, 0.0f, 0.0f);
    }

    private void Awake() => this.interactable = this.GetComponent<Interactable>();

    private void Start()
    {
      if ((Object) this.childCollider == (Object) null)
        this.childCollider = this.GetComponentInChildren<Collider>();
      if ((Object) this.linearMapping == (Object) null)
        this.linearMapping = this.GetComponent<LinearMapping>();
      if ((Object) this.linearMapping == (Object) null)
        this.linearMapping = this.gameObject.AddComponent<LinearMapping>();
      this.worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f);
      this.worldPlaneNormal[(int) this.axisOfRotation] = 1f;
      this.localPlaneNormal = this.worldPlaneNormal;
      if ((bool) (Object) this.transform.parent)
        this.worldPlaneNormal = this.transform.parent.localToWorldMatrix.MultiplyVector(this.worldPlaneNormal).normalized;
      if (this.limited)
      {
        this.start = Quaternion.identity;
        this.outAngle = this.transform.localEulerAngles[(int) this.axisOfRotation];
        if (this.forceStart)
          this.outAngle = Mathf.Clamp(this.startAngle, this.minAngle, this.maxAngle);
      }
      else
      {
        this.start = Quaternion.AngleAxis(this.transform.localEulerAngles[(int) this.axisOfRotation], this.localPlaneNormal);
        this.outAngle = 0.0f;
      }
      if ((bool) (Object) this.debugText)
      {
        this.debugText.alignment = TextAlignment.Left;
        this.debugText.anchor = TextAnchor.UpperLeft;
      }
      this.UpdateAll();
    }

    private void OnDisable()
    {
      if (!(bool) (Object) this.handHoverLocked)
        return;
      this.handHoverLocked.HideGrabHint();
      this.handHoverLocked.HoverUnlock(this.interactable);
      this.handHoverLocked = (Hand) null;
    }

    [DebuggerHidden]
    private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount) => (IEnumerator) new CircularDrive.\u003CHapticPulses\u003Ec__Iterator0()
    {
      hand = hand,
      flMagnitude = flMagnitude,
      nCount = nCount
    };

    private void OnHandHoverBegin(Hand hand) => hand.ShowGrabHint();

    private void OnHandHoverEnd(Hand hand)
    {
      hand.HideGrabHint();
      if (this.driving && (bool) (Object) hand)
        this.StartCoroutine(this.HapticPulses(hand, 1f, 10));
      this.driving = false;
      this.handHoverLocked = (Hand) null;
    }

    private void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      bool flag = !hand.IsGrabbingWithType(this.grabbedWithType);
      if (this.grabbedWithType == GrabTypes.None && grabStarting != GrabTypes.None)
      {
        this.grabbedWithType = grabStarting;
        this.lastHandProjected = this.ComputeToTransformProjected(hand.hoverSphereTransform);
        if (this.hoverLock)
        {
          hand.HoverLock(this.interactable);
          this.handHoverLocked = hand;
        }
        this.driving = true;
        this.ComputeAngle(hand);
        this.UpdateAll();
        hand.HideGrabHint();
      }
      else if (this.grabbedWithType != GrabTypes.None && flag)
      {
        if (this.hoverLock)
        {
          hand.HoverUnlock(this.interactable);
          this.handHoverLocked = (Hand) null;
        }
        this.driving = false;
        this.grabbedWithType = GrabTypes.None;
      }
      if (!this.driving || flag || !((Object) hand.hoveringInteractable == (Object) this.interactable))
        return;
      this.ComputeAngle(hand);
      this.UpdateAll();
    }

    private Vector3 ComputeToTransformProjected(Transform xForm)
    {
      Vector3 normalized = (xForm.position - this.transform.position).normalized;
      Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);
      if ((double) normalized.sqrMagnitude > 0.0)
        toTransformProjected = Vector3.ProjectOnPlane(normalized, this.worldPlaneNormal).normalized;
      else
        UnityEngine.Debug.LogFormat("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", (object) this.gameObject.ToString());
      if (this.debugPath && this.dbgPathLimit > 0)
        this.DrawDebugPath(xForm, toTransformProjected);
      return toTransformProjected;
    }

    private void DrawDebugPath(Transform xForm, Vector3 toTransformProjected)
    {
      if (this.dbgObjectCount == 0)
      {
        this.dbgObjectsParent = new GameObject("Circular Drive Debug");
        this.dbgHandObjects = new GameObject[this.dbgPathLimit];
        this.dbgProjObjects = new GameObject[this.dbgPathLimit];
        this.dbgObjectCount = this.dbgPathLimit;
        this.dbgObjectIndex = 0;
      }
      GameObject gameObject1 = (GameObject) null;
      GameObject gameObject2;
      if ((bool) (Object) this.dbgHandObjects[this.dbgObjectIndex])
      {
        gameObject2 = this.dbgHandObjects[this.dbgObjectIndex];
      }
      else
      {
        gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject2.transform.SetParent(this.dbgObjectsParent.transform);
        this.dbgHandObjects[this.dbgObjectIndex] = gameObject2;
      }
      gameObject2.name = string.Format("actual_{0}", (object) (int) ((1.0 - (double) this.red.r) * 10.0));
      gameObject2.transform.position = xForm.position;
      gameObject2.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
      gameObject2.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
      gameObject2.gameObject.GetComponent<Renderer>().material.color = this.red;
      if ((double) this.red.r > 0.100000001490116)
        this.red.r -= 0.1f;
      else
        this.red.r = 1f;
      gameObject1 = (GameObject) null;
      GameObject gameObject3;
      if ((bool) (Object) this.dbgProjObjects[this.dbgObjectIndex])
      {
        gameObject3 = this.dbgProjObjects[this.dbgObjectIndex];
      }
      else
      {
        gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject3.transform.SetParent(this.dbgObjectsParent.transform);
        this.dbgProjObjects[this.dbgObjectIndex] = gameObject3;
      }
      gameObject3.name = string.Format("projed_{0}", (object) (int) ((1.0 - (double) this.green.g) * 10.0));
      gameObject3.transform.position = this.transform.position + toTransformProjected * 0.25f;
      gameObject3.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
      gameObject3.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
      gameObject3.gameObject.GetComponent<Renderer>().material.color = this.green;
      if ((double) this.green.g > 0.100000001490116)
        this.green.g -= 0.1f;
      else
        this.green.g = 1f;
      this.dbgObjectIndex = (this.dbgObjectIndex + 1) % this.dbgObjectCount;
    }

    private void UpdateLinearMapping()
    {
      if (this.limited)
      {
        this.linearMapping.value = (float) (((double) this.outAngle - (double) this.minAngle) / ((double) this.maxAngle - (double) this.minAngle));
      }
      else
      {
        float f = this.outAngle / 360f;
        this.linearMapping.value = f - Mathf.Floor(f);
      }
      this.UpdateDebugText();
    }

    private void UpdateGameObject()
    {
      if (!this.rotateGameObject)
        return;
      this.transform.localRotation = this.start * Quaternion.AngleAxis(this.outAngle, this.localPlaneNormal);
    }

    private void UpdateDebugText()
    {
      if (!(bool) (Object) this.debugText)
        return;
      this.debugText.text = string.Format("Linear: {0}\nAngle:  {1}\n", (object) this.linearMapping.value, (object) this.outAngle);
    }

    private void UpdateAll()
    {
      this.UpdateLinearMapping();
      this.UpdateGameObject();
      this.UpdateDebugText();
    }

    private void ComputeAngle(Hand hand)
    {
      Vector3 transformProjected = this.ComputeToTransformProjected(hand.hoverSphereTransform);
      if (transformProjected.Equals((object) this.lastHandProjected))
        return;
      float num1 = Vector3.Angle(this.lastHandProjected, transformProjected);
      if ((double) num1 <= 0.0)
        return;
      if (this.frozen)
      {
        float sqrMagnitude = (hand.hoverSphereTransform.position - this.frozenHandWorldPos).sqrMagnitude;
        if ((double) sqrMagnitude <= (double) this.frozenSqDistanceMinMaxThreshold.x)
          return;
        this.outAngle = this.frozenAngle + Random.Range(-1f, 1f);
        float flMagnitude = Util.RemapNumberClamped(sqrMagnitude, this.frozenSqDistanceMinMaxThreshold.x, this.frozenSqDistanceMinMaxThreshold.y, 0.0f, 1f);
        if ((double) flMagnitude > 0.0)
          this.StartCoroutine(this.HapticPulses(hand, flMagnitude, 10));
        else
          this.StartCoroutine(this.HapticPulses(hand, 0.5f, 10));
        if ((double) sqrMagnitude < (double) this.frozenSqDistanceMinMaxThreshold.y)
          return;
        this.onFrozenDistanceThreshold.Invoke();
      }
      else
      {
        float num2 = Vector3.Dot(this.worldPlaneNormal, Vector3.Cross(this.lastHandProjected, transformProjected).normalized);
        float num3 = num1;
        if ((double) num2 < 0.0)
          num3 = -num3;
        if (this.limited)
        {
          float num4 = Mathf.Clamp(this.outAngle + num3, this.minAngle, this.maxAngle);
          if ((double) this.outAngle == (double) this.minAngle)
          {
            if ((double) num4 <= (double) this.minAngle || (double) num1 >= (double) this.minMaxAngularThreshold)
              return;
            this.outAngle = num4;
            this.lastHandProjected = transformProjected;
          }
          else if ((double) this.outAngle == (double) this.maxAngle)
          {
            if ((double) num4 >= (double) this.maxAngle || (double) num1 >= (double) this.minMaxAngularThreshold)
              return;
            this.outAngle = num4;
            this.lastHandProjected = transformProjected;
          }
          else if ((double) num4 == (double) this.minAngle)
          {
            this.outAngle = num4;
            this.lastHandProjected = transformProjected;
            this.onMinAngle.Invoke();
            if (!this.freezeOnMin)
              return;
            this.Freeze(hand);
          }
          else if ((double) num4 == (double) this.maxAngle)
          {
            this.outAngle = num4;
            this.lastHandProjected = transformProjected;
            this.onMaxAngle.Invoke();
            if (!this.freezeOnMax)
              return;
            this.Freeze(hand);
          }
          else
          {
            this.outAngle = num4;
            this.lastHandProjected = transformProjected;
          }
        }
        else
        {
          this.outAngle += num3;
          this.lastHandProjected = transformProjected;
        }
      }
    }

    public enum Axis_t
    {
      XAxis,
      YAxis,
      ZAxis,
    }
  }
}
