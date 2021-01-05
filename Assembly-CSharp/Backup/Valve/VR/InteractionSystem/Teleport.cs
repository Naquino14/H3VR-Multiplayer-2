// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Teleport
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class Teleport : MonoBehaviour
  {
    public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(nameof (Teleport));
    public LayerMask traceLayerMask;
    public LayerMask floorFixupTraceLayerMask;
    public float floorFixupMaximumTraceDistance = 1f;
    public Material areaVisibleMaterial;
    public Material areaLockedMaterial;
    public Material areaHighlightedMaterial;
    public Material pointVisibleMaterial;
    public Material pointLockedMaterial;
    public Material pointHighlightedMaterial;
    public Transform destinationReticleTransform;
    public Transform invalidReticleTransform;
    public GameObject playAreaPreviewCorner;
    public GameObject playAreaPreviewSide;
    public Color pointerValidColor;
    public Color pointerInvalidColor;
    public Color pointerLockedColor;
    public bool showPlayAreaMarker = true;
    public float teleportFadeTime = 0.1f;
    public float meshFadeTime = 0.2f;
    public float arcDistance = 10f;
    [Header("Effects")]
    public Transform onActivateObjectTransform;
    public Transform onDeactivateObjectTransform;
    public float activateObjectTime = 1f;
    public float deactivateObjectTime = 1f;
    [Header("Audio Sources")]
    public AudioSource pointerAudioSource;
    public AudioSource loopingAudioSource;
    public AudioSource headAudioSource;
    public AudioSource reticleAudioSource;
    [Header("Sounds")]
    public AudioClip teleportSound;
    public AudioClip pointerStartSound;
    public AudioClip pointerLoopSound;
    public AudioClip pointerStopSound;
    public AudioClip goodHighlightSound;
    public AudioClip badHighlightSound;
    [Header("Debug")]
    public bool debugFloor;
    public bool showOffsetReticle;
    public Transform offsetReticleTransform;
    public MeshRenderer floorDebugSphere;
    public LineRenderer floorDebugLine;
    private LineRenderer pointerLineRenderer;
    private GameObject teleportPointerObject;
    private Transform pointerStartTransform;
    private Hand pointerHand;
    private Valve.VR.InteractionSystem.Player player;
    private TeleportArc teleportArc;
    private bool visible;
    private TeleportMarkerBase[] teleportMarkers;
    private TeleportMarkerBase pointedAtTeleportMarker;
    private TeleportMarkerBase teleportingToMarker;
    private Vector3 pointedAtPosition;
    private Vector3 prevPointedAtPosition;
    private bool teleporting;
    private float currentFadeTime;
    private float meshAlphaPercent = 1f;
    private float pointerShowStartTime;
    private float pointerHideStartTime;
    private bool meshFading;
    private float fullTintAlpha;
    private float invalidReticleMinScale = 0.2f;
    private float invalidReticleMaxScale = 1f;
    private float invalidReticleMinScaleDistance = 0.4f;
    private float invalidReticleMaxScaleDistance = 2f;
    private Vector3 invalidReticleScale = Vector3.one;
    private Quaternion invalidReticleTargetRotation = Quaternion.identity;
    private Transform playAreaPreviewTransform;
    private Transform[] playAreaPreviewCorners;
    private Transform[] playAreaPreviewSides;
    private float loopingAudioMaxVolume;
    private Coroutine hintCoroutine;
    private bool originalHoverLockState;
    private Interactable originalHoveringInteractable;
    private AllowTeleportWhileAttachedToHand allowTeleportWhileAttached;
    private Vector3 startingFeetOffset = Vector3.zero;
    private bool movedFeetFarEnough;
    private SteamVR_Events.Action chaperoneInfoInitializedAction;
    public static SteamVR_Events.Event<float> ChangeScene = new SteamVR_Events.Event<float>();
    public static SteamVR_Events.Event<TeleportMarkerBase> Player = new SteamVR_Events.Event<TeleportMarkerBase>();
    public static SteamVR_Events.Event<TeleportMarkerBase> PlayerPre = new SteamVR_Events.Event<TeleportMarkerBase>();
    private static Teleport _instance;

    public static SteamVR_Events.Action<float> ChangeSceneAction(
      UnityAction<float> action)
    {
      return new SteamVR_Events.Action<float>(Teleport.ChangeScene, action);
    }

    public static SteamVR_Events.Action<TeleportMarkerBase> PlayerAction(
      UnityAction<TeleportMarkerBase> action)
    {
      return new SteamVR_Events.Action<TeleportMarkerBase>(Teleport.Player, action);
    }

    public static SteamVR_Events.Action<TeleportMarkerBase> PlayerPreAction(
      UnityAction<TeleportMarkerBase> action)
    {
      return new SteamVR_Events.Action<TeleportMarkerBase>(Teleport.PlayerPre, action);
    }

    public static Teleport instance
    {
      get
      {
        if ((Object) Teleport._instance == (Object) null)
          Teleport._instance = Object.FindObjectOfType<Teleport>();
        return Teleport._instance;
      }
    }

    private void Awake()
    {
      Teleport._instance = this;
      this.chaperoneInfoInitializedAction = ChaperoneInfo.InitializedAction(new UnityAction(this.OnChaperoneInfoInitialized));
      this.pointerLineRenderer = this.GetComponentInChildren<LineRenderer>();
      this.teleportPointerObject = this.pointerLineRenderer.gameObject;
      this.fullTintAlpha = this.pointVisibleMaterial.GetColor(Shader.PropertyToID("_TintColor")).a;
      this.teleportArc = this.GetComponent<TeleportArc>();
      this.teleportArc.traceLayerMask = (int) this.traceLayerMask;
      this.loopingAudioMaxVolume = this.loopingAudioSource.volume;
      this.playAreaPreviewCorner.SetActive(false);
      this.playAreaPreviewSide.SetActive(false);
      float x = this.invalidReticleTransform.localScale.x;
      this.invalidReticleMinScale *= x;
      this.invalidReticleMaxScale *= x;
    }

    private void Start()
    {
      this.teleportMarkers = Object.FindObjectsOfType<TeleportMarkerBase>();
      this.HidePointer();
      this.player = Valve.VR.InteractionSystem.Player.instance;
      if ((Object) this.player == (Object) null)
      {
        UnityEngine.Debug.LogError((object) "<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        this.CheckForSpawnPoint();
        this.Invoke("ShowTeleportHint", 5f);
      }
    }

    private void OnEnable()
    {
      this.chaperoneInfoInitializedAction.enabled = true;
      this.OnChaperoneInfoInitialized();
    }

    private void OnDisable()
    {
      this.chaperoneInfoInitializedAction.enabled = false;
      this.HidePointer();
    }

    private void CheckForSpawnPoint()
    {
      foreach (TeleportMarkerBase teleportMarker in this.teleportMarkers)
      {
        TeleportPoint teleportPoint = teleportMarker as TeleportPoint;
        if ((bool) (Object) teleportPoint && teleportPoint.playerSpawnPoint)
        {
          this.teleportingToMarker = teleportMarker;
          this.TeleportPlayer();
          break;
        }
      }
    }

    public void HideTeleportPointer()
    {
      if (!((Object) this.pointerHand != (Object) null))
        return;
      this.HidePointer();
    }

    private void Update()
    {
      Hand pointerHand = this.pointerHand;
      Hand newPointerHand = (Hand) null;
      foreach (Hand hand in this.player.hands)
      {
        if (this.visible && this.WasTeleportButtonReleased(hand) && (Object) this.pointerHand == (Object) hand)
          this.TryTeleportPlayer();
        if (this.WasTeleportButtonPressed(hand))
          newPointerHand = hand;
      }
      if ((bool) (Object) this.allowTeleportWhileAttached && !this.allowTeleportWhileAttached.teleportAllowed)
        this.HidePointer();
      else if (!this.visible && (Object) newPointerHand != (Object) null)
        this.ShowPointer(newPointerHand, pointerHand);
      else if (this.visible)
      {
        if ((Object) newPointerHand == (Object) null && !this.IsTeleportButtonDown(this.pointerHand))
          this.HidePointer();
        else if ((Object) newPointerHand != (Object) null)
          this.ShowPointer(newPointerHand, pointerHand);
      }
      if (this.visible)
      {
        this.UpdatePointer();
        if (this.meshFading)
          this.UpdateTeleportColors();
        if (!this.onActivateObjectTransform.gameObject.activeSelf || (double) Time.time - (double) this.pointerShowStartTime <= (double) this.activateObjectTime)
          return;
        this.onActivateObjectTransform.gameObject.SetActive(false);
      }
      else
      {
        if (!this.onDeactivateObjectTransform.gameObject.activeSelf || (double) Time.time - (double) this.pointerHideStartTime <= (double) this.deactivateObjectTime)
          return;
        this.onDeactivateObjectTransform.gameObject.SetActive(false);
      }
    }

    private void UpdatePointer()
    {
      Vector3 position1 = this.pointerStartTransform.position;
      Vector3 forward = this.pointerStartTransform.forward;
      bool flag1 = false;
      bool flag2 = false;
      Vector3 vector3_1 = this.player.trackingOriginTransform.position - this.player.feetPositionGuess;
      Vector3 velocity = forward * this.arcDistance;
      TeleportMarkerBase hitTeleportMarker = (TeleportMarkerBase) null;
      float num1 = Vector3.Dot(forward, Vector3.up);
      float num2 = Vector3.Dot(forward, this.player.hmdTransform.forward);
      bool pointerAtBadAngle = false;
      if ((double) num2 > 0.0 && (double) num1 > 0.75 || (double) num2 < 0.0 && (double) num1 > 0.5)
        pointerAtBadAngle = true;
      this.teleportArc.SetArcData(position1, velocity, true, pointerAtBadAngle);
      RaycastHit hitInfo;
      if (this.teleportArc.DrawArc(out hitInfo))
      {
        flag1 = true;
        hitTeleportMarker = hitInfo.collider.GetComponentInParent<TeleportMarkerBase>();
      }
      if (pointerAtBadAngle)
        hitTeleportMarker = (TeleportMarkerBase) null;
      this.HighlightSelected(hitTeleportMarker);
      Vector3 position2;
      if ((Object) hitTeleportMarker != (Object) null)
      {
        if (hitTeleportMarker.locked)
        {
          this.teleportArc.SetColor(this.pointerLockedColor);
          this.pointerLineRenderer.startColor = this.pointerLockedColor;
          this.pointerLineRenderer.endColor = this.pointerLockedColor;
          this.destinationReticleTransform.gameObject.SetActive(false);
        }
        else
        {
          this.teleportArc.SetColor(this.pointerValidColor);
          this.pointerLineRenderer.startColor = this.pointerValidColor;
          this.pointerLineRenderer.endColor = this.pointerValidColor;
          this.destinationReticleTransform.gameObject.SetActive(hitTeleportMarker.showReticle);
        }
        this.offsetReticleTransform.gameObject.SetActive(true);
        this.invalidReticleTransform.gameObject.SetActive(false);
        this.pointedAtTeleportMarker = hitTeleportMarker;
        this.pointedAtPosition = hitInfo.point;
        if (this.showPlayAreaMarker)
        {
          TeleportArea atTeleportMarker = this.pointedAtTeleportMarker as TeleportArea;
          if ((Object) atTeleportMarker != (Object) null && !atTeleportMarker.locked && (Object) this.playAreaPreviewTransform != (Object) null)
          {
            Vector3 vector3_2 = vector3_1;
            if (!this.movedFeetFarEnough)
            {
              float num3 = Vector3.Distance(vector3_1, this.startingFeetOffset);
              if ((double) num3 < 0.100000001490116)
                vector3_2 = this.startingFeetOffset;
              else if ((double) num3 < 0.400000005960464)
                vector3_2 = Vector3.Lerp(this.startingFeetOffset, vector3_1, (float) (((double) num3 - 0.100000001490116) / 0.300000011920929));
              else
                this.movedFeetFarEnough = true;
            }
            this.playAreaPreviewTransform.position = this.pointedAtPosition + vector3_2;
            flag2 = true;
          }
        }
        position2 = hitInfo.point;
      }
      else
      {
        this.destinationReticleTransform.gameObject.SetActive(false);
        this.offsetReticleTransform.gameObject.SetActive(false);
        this.teleportArc.SetColor(this.pointerInvalidColor);
        this.pointerLineRenderer.startColor = this.pointerInvalidColor;
        this.pointerLineRenderer.endColor = this.pointerInvalidColor;
        this.invalidReticleTransform.gameObject.SetActive(!pointerAtBadAngle);
        Vector3 toDirection = hitInfo.normal;
        if ((double) Vector3.Angle(hitInfo.normal, Vector3.up) < 15.0)
          toDirection = Vector3.up;
        this.invalidReticleTargetRotation = Quaternion.FromToRotation(Vector3.up, toDirection);
        this.invalidReticleTransform.rotation = Quaternion.Slerp(this.invalidReticleTransform.rotation, this.invalidReticleTargetRotation, 0.1f);
        float num3 = Util.RemapNumberClamped(Vector3.Distance(hitInfo.point, this.player.hmdTransform.position), this.invalidReticleMinScaleDistance, this.invalidReticleMaxScaleDistance, this.invalidReticleMinScale, this.invalidReticleMaxScale);
        this.invalidReticleScale.x = num3;
        this.invalidReticleScale.y = num3;
        this.invalidReticleScale.z = num3;
        this.invalidReticleTransform.transform.localScale = this.invalidReticleScale;
        this.pointedAtTeleportMarker = (TeleportMarkerBase) null;
        position2 = !flag1 ? this.teleportArc.GetArcPositionAtTime(this.teleportArc.arcDuration) : hitInfo.point;
        if (this.debugFloor)
        {
          this.floorDebugSphere.gameObject.SetActive(false);
          this.floorDebugLine.gameObject.SetActive(false);
        }
      }
      if ((Object) this.playAreaPreviewTransform != (Object) null)
        this.playAreaPreviewTransform.gameObject.SetActive(flag2);
      if (!this.showOffsetReticle)
        this.offsetReticleTransform.gameObject.SetActive(false);
      this.destinationReticleTransform.position = this.pointedAtPosition;
      this.invalidReticleTransform.position = position2;
      this.onActivateObjectTransform.position = position2;
      this.onDeactivateObjectTransform.position = position2;
      this.offsetReticleTransform.position = position2 - vector3_1;
      this.reticleAudioSource.transform.position = this.pointedAtPosition;
      this.pointerLineRenderer.SetPosition(0, position1);
      this.pointerLineRenderer.SetPosition(1, position2);
    }

    private void FixedUpdate()
    {
      if (!this.visible || !this.debugFloor || (!((Object) (this.pointedAtTeleportMarker as TeleportArea) != (Object) null) || (double) this.floorFixupMaximumTraceDistance <= 0.0))
        return;
      this.floorDebugSphere.gameObject.SetActive(true);
      this.floorDebugLine.gameObject.SetActive(true);
      Vector3 down = Vector3.down;
      down.x = 0.01f;
      RaycastHit hitInfo;
      if (Physics.Raycast(this.pointedAtPosition + 0.05f * down, down, out hitInfo, this.floorFixupMaximumTraceDistance, (int) this.floorFixupTraceLayerMask))
      {
        this.floorDebugSphere.transform.position = hitInfo.point;
        this.floorDebugSphere.material.color = Color.green;
        this.floorDebugLine.startColor = Color.green;
        this.floorDebugLine.endColor = Color.green;
        this.floorDebugLine.SetPosition(0, this.pointedAtPosition);
        this.floorDebugLine.SetPosition(1, hitInfo.point);
      }
      else
      {
        Vector3 position = this.pointedAtPosition + down * this.floorFixupMaximumTraceDistance;
        this.floorDebugSphere.transform.position = position;
        this.floorDebugSphere.material.color = Color.red;
        this.floorDebugLine.startColor = Color.red;
        this.floorDebugLine.endColor = Color.red;
        this.floorDebugLine.SetPosition(0, this.pointedAtPosition);
        this.floorDebugLine.SetPosition(1, position);
      }
    }

    private void OnChaperoneInfoInitialized()
    {
      ChaperoneInfo instance = ChaperoneInfo.instance;
      if (!instance.initialized || !instance.roomscale)
        return;
      if ((Object) this.playAreaPreviewTransform == (Object) null)
      {
        this.playAreaPreviewTransform = new GameObject("PlayAreaPreviewTransform").transform;
        this.playAreaPreviewTransform.parent = this.transform;
        Util.ResetTransform(this.playAreaPreviewTransform);
        this.playAreaPreviewCorner.SetActive(true);
        this.playAreaPreviewCorners = new Transform[4];
        this.playAreaPreviewCorners[0] = this.playAreaPreviewCorner.transform;
        this.playAreaPreviewCorners[1] = Object.Instantiate<Transform>(this.playAreaPreviewCorners[0]);
        this.playAreaPreviewCorners[2] = Object.Instantiate<Transform>(this.playAreaPreviewCorners[0]);
        this.playAreaPreviewCorners[3] = Object.Instantiate<Transform>(this.playAreaPreviewCorners[0]);
        this.playAreaPreviewCorners[0].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewCorners[1].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewCorners[2].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewCorners[3].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewSide.SetActive(true);
        this.playAreaPreviewSides = new Transform[4];
        this.playAreaPreviewSides[0] = this.playAreaPreviewSide.transform;
        this.playAreaPreviewSides[1] = Object.Instantiate<Transform>(this.playAreaPreviewSides[0]);
        this.playAreaPreviewSides[2] = Object.Instantiate<Transform>(this.playAreaPreviewSides[0]);
        this.playAreaPreviewSides[3] = Object.Instantiate<Transform>(this.playAreaPreviewSides[0]);
        this.playAreaPreviewSides[0].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewSides[1].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewSides[2].transform.parent = this.playAreaPreviewTransform;
        this.playAreaPreviewSides[3].transform.parent = this.playAreaPreviewTransform;
      }
      float playAreaSizeX = instance.playAreaSizeX;
      float playAreaSizeZ = instance.playAreaSizeZ;
      this.playAreaPreviewSides[0].localPosition = new Vector3(0.0f, 0.0f, (float) (0.5 * (double) playAreaSizeZ - 0.25));
      this.playAreaPreviewSides[1].localPosition = new Vector3(0.0f, 0.0f, (float) (-0.5 * (double) playAreaSizeZ + 0.25));
      this.playAreaPreviewSides[2].localPosition = new Vector3((float) (0.5 * (double) playAreaSizeX - 0.25), 0.0f, 0.0f);
      this.playAreaPreviewSides[3].localPosition = new Vector3((float) (-0.5 * (double) playAreaSizeX + 0.25), 0.0f, 0.0f);
      this.playAreaPreviewSides[0].localScale = new Vector3(playAreaSizeX - 0.5f, 1f, 1f);
      this.playAreaPreviewSides[1].localScale = new Vector3(playAreaSizeX - 0.5f, 1f, 1f);
      this.playAreaPreviewSides[2].localScale = new Vector3(playAreaSizeZ - 0.5f, 1f, 1f);
      this.playAreaPreviewSides[3].localScale = new Vector3(playAreaSizeZ - 0.5f, 1f, 1f);
      this.playAreaPreviewSides[0].localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
      this.playAreaPreviewSides[1].localRotation = Quaternion.Euler(0.0f, 180f, 0.0f);
      this.playAreaPreviewSides[2].localRotation = Quaternion.Euler(0.0f, 90f, 0.0f);
      this.playAreaPreviewSides[3].localRotation = Quaternion.Euler(0.0f, 270f, 0.0f);
      this.playAreaPreviewCorners[0].localPosition = new Vector3((float) (0.5 * (double) playAreaSizeX - 0.25), 0.0f, (float) (0.5 * (double) playAreaSizeZ - 0.25));
      this.playAreaPreviewCorners[1].localPosition = new Vector3((float) (0.5 * (double) playAreaSizeX - 0.25), 0.0f, (float) (-0.5 * (double) playAreaSizeZ + 0.25));
      this.playAreaPreviewCorners[2].localPosition = new Vector3((float) (-0.5 * (double) playAreaSizeX + 0.25), 0.0f, (float) (-0.5 * (double) playAreaSizeZ + 0.25));
      this.playAreaPreviewCorners[3].localPosition = new Vector3((float) (-0.5 * (double) playAreaSizeX + 0.25), 0.0f, (float) (0.5 * (double) playAreaSizeZ - 0.25));
      this.playAreaPreviewCorners[0].localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
      this.playAreaPreviewCorners[1].localRotation = Quaternion.Euler(0.0f, 90f, 0.0f);
      this.playAreaPreviewCorners[2].localRotation = Quaternion.Euler(0.0f, 180f, 0.0f);
      this.playAreaPreviewCorners[3].localRotation = Quaternion.Euler(0.0f, 270f, 0.0f);
      this.playAreaPreviewTransform.gameObject.SetActive(false);
    }

    private void HidePointer()
    {
      if (this.visible)
        this.pointerHideStartTime = Time.time;
      this.visible = false;
      if ((bool) (Object) this.pointerHand)
      {
        if (this.ShouldOverrideHoverLock())
        {
          if (this.originalHoverLockState)
            this.pointerHand.HoverLock(this.originalHoveringInteractable);
          else
            this.pointerHand.HoverUnlock((Interactable) null);
        }
        this.loopingAudioSource.Stop();
        this.PlayAudioClip(this.pointerAudioSource, this.pointerStopSound);
      }
      this.teleportPointerObject.SetActive(false);
      this.teleportArc.Hide();
      foreach (TeleportMarkerBase teleportMarker in this.teleportMarkers)
      {
        if ((Object) teleportMarker != (Object) null && teleportMarker.markerActive && (Object) teleportMarker.gameObject != (Object) null)
          teleportMarker.gameObject.SetActive(false);
      }
      this.destinationReticleTransform.gameObject.SetActive(false);
      this.invalidReticleTransform.gameObject.SetActive(false);
      this.offsetReticleTransform.gameObject.SetActive(false);
      if ((Object) this.playAreaPreviewTransform != (Object) null)
        this.playAreaPreviewTransform.gameObject.SetActive(false);
      if (this.onActivateObjectTransform.gameObject.activeSelf)
        this.onActivateObjectTransform.gameObject.SetActive(false);
      this.onDeactivateObjectTransform.gameObject.SetActive(true);
      this.pointerHand = (Hand) null;
    }

    private void ShowPointer(Hand newPointerHand, Hand oldPointerHand)
    {
      if (!this.visible)
      {
        this.pointedAtTeleportMarker = (TeleportMarkerBase) null;
        this.pointerShowStartTime = Time.time;
        this.visible = true;
        this.meshFading = true;
        this.teleportPointerObject.SetActive(false);
        this.teleportArc.Show();
        foreach (TeleportMarkerBase teleportMarker in this.teleportMarkers)
        {
          if (teleportMarker.markerActive && teleportMarker.ShouldActivate(this.player.feetPositionGuess))
          {
            teleportMarker.gameObject.SetActive(true);
            teleportMarker.Highlight(false);
          }
        }
        this.startingFeetOffset = this.player.trackingOriginTransform.position - this.player.feetPositionGuess;
        this.movedFeetFarEnough = false;
        if (this.onDeactivateObjectTransform.gameObject.activeSelf)
          this.onDeactivateObjectTransform.gameObject.SetActive(false);
        this.onActivateObjectTransform.gameObject.SetActive(true);
        this.loopingAudioSource.clip = this.pointerLoopSound;
        this.loopingAudioSource.loop = true;
        this.loopingAudioSource.Play();
        this.loopingAudioSource.volume = 0.0f;
      }
      if ((bool) (Object) oldPointerHand && this.ShouldOverrideHoverLock())
      {
        if (this.originalHoverLockState)
          oldPointerHand.HoverLock(this.originalHoveringInteractable);
        else
          oldPointerHand.HoverUnlock((Interactable) null);
      }
      this.pointerHand = newPointerHand;
      if (this.visible && (Object) oldPointerHand != (Object) this.pointerHand)
        this.PlayAudioClip(this.pointerAudioSource, this.pointerStartSound);
      if (!(bool) (Object) this.pointerHand)
        return;
      this.pointerStartTransform = this.GetPointerStartTransform(this.pointerHand);
      if ((Object) this.pointerHand.currentAttachedObject != (Object) null)
        this.allowTeleportWhileAttached = this.pointerHand.currentAttachedObject.GetComponent<AllowTeleportWhileAttachedToHand>();
      this.originalHoverLockState = this.pointerHand.hoverLocked;
      this.originalHoveringInteractable = this.pointerHand.hoveringInteractable;
      if (this.ShouldOverrideHoverLock())
        this.pointerHand.HoverLock((Interactable) null);
      this.pointerAudioSource.transform.SetParent(this.pointerStartTransform);
      this.pointerAudioSource.transform.localPosition = Vector3.zero;
      this.loopingAudioSource.transform.SetParent(this.pointerStartTransform);
      this.loopingAudioSource.transform.localPosition = Vector3.zero;
    }

    private void UpdateTeleportColors()
    {
      float num = Time.time - this.pointerShowStartTime;
      if ((double) num > (double) this.meshFadeTime)
      {
        this.meshAlphaPercent = 1f;
        this.meshFading = false;
      }
      else
        this.meshAlphaPercent = Mathf.Lerp(0.0f, 1f, num / this.meshFadeTime);
      foreach (TeleportMarkerBase teleportMarker in this.teleportMarkers)
        teleportMarker.SetAlpha(this.fullTintAlpha * this.meshAlphaPercent, this.meshAlphaPercent);
    }

    private void PlayAudioClip(AudioSource source, AudioClip clip)
    {
      source.clip = clip;
      source.Play();
    }

    private void PlayPointerHaptic(bool validLocation)
    {
      if (!((Object) this.pointerHand != (Object) null))
        return;
      if (validLocation)
        this.pointerHand.TriggerHapticPulse((ushort) 800);
      else
        this.pointerHand.TriggerHapticPulse((ushort) 100);
    }

    private void TryTeleportPlayer()
    {
      if (!this.visible || this.teleporting || (!((Object) this.pointedAtTeleportMarker != (Object) null) || this.pointedAtTeleportMarker.locked))
        return;
      this.teleportingToMarker = this.pointedAtTeleportMarker;
      this.InitiateTeleportFade();
      this.CancelTeleportHint();
    }

    private void InitiateTeleportFade()
    {
      this.teleporting = true;
      this.currentFadeTime = this.teleportFadeTime;
      TeleportPoint teleportingToMarker = this.teleportingToMarker as TeleportPoint;
      if ((Object) teleportingToMarker != (Object) null && teleportingToMarker.teleportType == TeleportPoint.TeleportPointType.SwitchToNewScene)
      {
        this.currentFadeTime *= 3f;
        Teleport.ChangeScene.Send(this.currentFadeTime);
      }
      SteamVR_Fade.Start(Color.clear, 0.0f);
      SteamVR_Fade.Start(Color.black, this.currentFadeTime);
      this.headAudioSource.transform.SetParent(this.player.hmdTransform);
      this.headAudioSource.transform.localPosition = Vector3.zero;
      this.PlayAudioClip(this.headAudioSource, this.teleportSound);
      this.Invoke("TeleportPlayer", this.currentFadeTime);
    }

    private void TeleportPlayer()
    {
      this.teleporting = false;
      Teleport.PlayerPre.Send(this.pointedAtTeleportMarker);
      SteamVR_Fade.Start(Color.clear, this.currentFadeTime);
      TeleportPoint teleportingToMarker = this.teleportingToMarker as TeleportPoint;
      Vector3 vector3_1 = this.pointedAtPosition;
      if ((Object) teleportingToMarker != (Object) null)
      {
        vector3_1 = teleportingToMarker.transform.position;
        if (teleportingToMarker.teleportType == TeleportPoint.TeleportPointType.SwitchToNewScene)
        {
          teleportingToMarker.TeleportToScene();
          return;
        }
      }
      RaycastHit hitInfo;
      if ((Object) (this.teleportingToMarker as TeleportArea) != (Object) null && (double) this.floorFixupMaximumTraceDistance > 0.0 && Physics.Raycast(vector3_1 + 0.05f * Vector3.down, Vector3.down, out hitInfo, this.floorFixupMaximumTraceDistance, (int) this.floorFixupTraceLayerMask))
        vector3_1 = hitInfo.point;
      if (this.teleportingToMarker.ShouldMovePlayer())
      {
        Vector3 vector3_2 = this.player.trackingOriginTransform.position - this.player.feetPositionGuess;
        this.player.trackingOriginTransform.position = vector3_1 + vector3_2;
        if (this.player.leftHand.currentAttachedObjectInfo.HasValue)
          this.player.leftHand.ResetAttachedTransform(this.player.leftHand.currentAttachedObjectInfo.Value);
        if (this.player.rightHand.currentAttachedObjectInfo.HasValue)
          this.player.rightHand.ResetAttachedTransform(this.player.rightHand.currentAttachedObjectInfo.Value);
      }
      else
        this.teleportingToMarker.TeleportPlayer(this.pointedAtPosition);
      Teleport.Player.Send(this.pointedAtTeleportMarker);
    }

    private void HighlightSelected(TeleportMarkerBase hitTeleportMarker)
    {
      if ((Object) this.pointedAtTeleportMarker != (Object) hitTeleportMarker)
      {
        if ((Object) this.pointedAtTeleportMarker != (Object) null)
          this.pointedAtTeleportMarker.Highlight(false);
        if ((Object) hitTeleportMarker != (Object) null)
        {
          hitTeleportMarker.Highlight(true);
          this.prevPointedAtPosition = this.pointedAtPosition;
          this.PlayPointerHaptic(!hitTeleportMarker.locked);
          this.PlayAudioClip(this.reticleAudioSource, this.goodHighlightSound);
          this.loopingAudioSource.volume = this.loopingAudioMaxVolume;
        }
        else
        {
          if (!((Object) this.pointedAtTeleportMarker != (Object) null))
            return;
          this.PlayAudioClip(this.reticleAudioSource, this.badHighlightSound);
          this.loopingAudioSource.volume = 0.0f;
        }
      }
      else
      {
        if (!((Object) hitTeleportMarker != (Object) null) || (double) Vector3.Distance(this.prevPointedAtPosition, this.pointedAtPosition) <= 1.0)
          return;
        this.prevPointedAtPosition = this.pointedAtPosition;
        this.PlayPointerHaptic(!hitTeleportMarker.locked);
      }
    }

    public void ShowTeleportHint()
    {
      this.CancelTeleportHint();
      this.hintCoroutine = this.StartCoroutine(this.TeleportHintCoroutine());
    }

    public void CancelTeleportHint()
    {
      if (this.hintCoroutine != null)
      {
        ControllerButtonHints.HideTextHint(this.player.leftHand, (ISteamVR_Action_In_Source) this.teleportAction);
        ControllerButtonHints.HideTextHint(this.player.rightHand, (ISteamVR_Action_In_Source) this.teleportAction);
        this.StopCoroutine(this.hintCoroutine);
        this.hintCoroutine = (Coroutine) null;
      }
      this.CancelInvoke("ShowTeleportHint");
    }

    [DebuggerHidden]
    private IEnumerator TeleportHintCoroutine() => (IEnumerator) new Teleport.\u003CTeleportHintCoroutine\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public bool IsEligibleForTeleport(Hand hand)
    {
      if ((Object) hand == (Object) null || !hand.gameObject.activeInHierarchy || (Object) hand.hoveringInteractable != (Object) null)
        return false;
      if ((Object) hand.noSteamVRFallbackCamera == (Object) null)
      {
        if (!hand.isActive)
          return false;
        if ((Object) hand.currentAttachedObject != (Object) null)
        {
          AllowTeleportWhileAttachedToHand component = hand.currentAttachedObject.GetComponent<AllowTeleportWhileAttachedToHand>();
          return (Object) component != (Object) null && component.teleportAllowed;
        }
      }
      return true;
    }

    private bool ShouldOverrideHoverLock() => !(bool) (Object) this.allowTeleportWhileAttached || this.allowTeleportWhileAttached.overrideHoverLock;

    private bool WasTeleportButtonReleased(Hand hand)
    {
      if (!this.IsEligibleForTeleport(hand))
        return false;
      return (Object) hand.noSteamVRFallbackCamera != (Object) null ? Input.GetKeyUp(KeyCode.T) : this.teleportAction.GetStateUp(hand.handType);
    }

    private bool IsTeleportButtonDown(Hand hand)
    {
      if (!this.IsEligibleForTeleport(hand))
        return false;
      return (Object) hand.noSteamVRFallbackCamera != (Object) null ? Input.GetKey(KeyCode.T) : this.teleportAction.GetState(hand.handType);
    }

    private bool WasTeleportButtonPressed(Hand hand)
    {
      if (!this.IsEligibleForTeleport(hand))
        return false;
      return (Object) hand.noSteamVRFallbackCamera != (Object) null ? Input.GetKeyDown(KeyCode.T) : this.teleportAction.GetStateDown(hand.handType);
    }

    private Transform GetPointerStartTransform(Hand hand) => (Object) hand.noSteamVRFallbackCamera != (Object) null ? hand.noSteamVRFallbackCamera.transform : hand.transform;
  }
}
