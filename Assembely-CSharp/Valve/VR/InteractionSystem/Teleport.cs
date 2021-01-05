using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class Teleport : MonoBehaviour
	{
		public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

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

		private Player player;

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

		public static Teleport instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType<Teleport>();
				}
				return _instance;
			}
		}

		public static SteamVR_Events.Action<float> ChangeSceneAction(UnityAction<float> action)
		{
			return new SteamVR_Events.Action<float>(ChangeScene, action);
		}

		public static SteamVR_Events.Action<TeleportMarkerBase> PlayerAction(UnityAction<TeleportMarkerBase> action)
		{
			return new SteamVR_Events.Action<TeleportMarkerBase>(Player, action);
		}

		public static SteamVR_Events.Action<TeleportMarkerBase> PlayerPreAction(UnityAction<TeleportMarkerBase> action)
		{
			return new SteamVR_Events.Action<TeleportMarkerBase>(PlayerPre, action);
		}

		private void Awake()
		{
			_instance = this;
			chaperoneInfoInitializedAction = ChaperoneInfo.InitializedAction(OnChaperoneInfoInitialized);
			pointerLineRenderer = GetComponentInChildren<LineRenderer>();
			teleportPointerObject = pointerLineRenderer.gameObject;
			int nameID = Shader.PropertyToID("_TintColor");
			fullTintAlpha = pointVisibleMaterial.GetColor(nameID).a;
			teleportArc = GetComponent<TeleportArc>();
			teleportArc.traceLayerMask = traceLayerMask;
			loopingAudioMaxVolume = loopingAudioSource.volume;
			playAreaPreviewCorner.SetActive(value: false);
			playAreaPreviewSide.SetActive(value: false);
			float x = invalidReticleTransform.localScale.x;
			invalidReticleMinScale *= x;
			invalidReticleMaxScale *= x;
		}

		private void Start()
		{
			teleportMarkers = Object.FindObjectsOfType<TeleportMarkerBase>();
			HidePointer();
			player = Valve.VR.InteractionSystem.Player.instance;
			if (player == null)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
				Object.Destroy(base.gameObject);
			}
			else
			{
				CheckForSpawnPoint();
				Invoke("ShowTeleportHint", 5f);
			}
		}

		private void OnEnable()
		{
			chaperoneInfoInitializedAction.enabled = true;
			OnChaperoneInfoInitialized();
		}

		private void OnDisable()
		{
			chaperoneInfoInitializedAction.enabled = false;
			HidePointer();
		}

		private void CheckForSpawnPoint()
		{
			TeleportMarkerBase[] array = teleportMarkers;
			foreach (TeleportMarkerBase teleportMarkerBase in array)
			{
				TeleportPoint teleportPoint = teleportMarkerBase as TeleportPoint;
				if ((bool)teleportPoint && teleportPoint.playerSpawnPoint)
				{
					teleportingToMarker = teleportMarkerBase;
					TeleportPlayer();
					break;
				}
			}
		}

		public void HideTeleportPointer()
		{
			if (pointerHand != null)
			{
				HidePointer();
			}
		}

		private void Update()
		{
			Hand oldPointerHand = pointerHand;
			Hand hand = null;
			Hand[] hands = player.hands;
			foreach (Hand hand2 in hands)
			{
				if (visible && WasTeleportButtonReleased(hand2) && pointerHand == hand2)
				{
					TryTeleportPlayer();
				}
				if (WasTeleportButtonPressed(hand2))
				{
					hand = hand2;
				}
			}
			if ((bool)allowTeleportWhileAttached && !allowTeleportWhileAttached.teleportAllowed)
			{
				HidePointer();
			}
			else if (!visible && hand != null)
			{
				ShowPointer(hand, oldPointerHand);
			}
			else if (visible)
			{
				if (hand == null && !IsTeleportButtonDown(pointerHand))
				{
					HidePointer();
				}
				else if (hand != null)
				{
					ShowPointer(hand, oldPointerHand);
				}
			}
			if (visible)
			{
				UpdatePointer();
				if (meshFading)
				{
					UpdateTeleportColors();
				}
				if (onActivateObjectTransform.gameObject.activeSelf && Time.time - pointerShowStartTime > activateObjectTime)
				{
					onActivateObjectTransform.gameObject.SetActive(value: false);
				}
			}
			else if (onDeactivateObjectTransform.gameObject.activeSelf && Time.time - pointerHideStartTime > deactivateObjectTime)
			{
				onDeactivateObjectTransform.gameObject.SetActive(value: false);
			}
		}

		private void UpdatePointer()
		{
			Vector3 position = pointerStartTransform.position;
			Vector3 forward = pointerStartTransform.forward;
			bool flag = false;
			bool active = false;
			Vector3 vector = player.trackingOriginTransform.position - player.feetPositionGuess;
			Vector3 velocity = forward * arcDistance;
			TeleportMarkerBase teleportMarkerBase = null;
			float num = Vector3.Dot(forward, Vector3.up);
			float num2 = Vector3.Dot(forward, player.hmdTransform.forward);
			bool flag2 = false;
			if ((num2 > 0f && num > 0.75f) || (num2 < 0f && num > 0.5f))
			{
				flag2 = true;
			}
			teleportArc.SetArcData(position, velocity, gravity: true, flag2);
			if (teleportArc.DrawArc(out var hitInfo))
			{
				flag = true;
				teleportMarkerBase = hitInfo.collider.GetComponentInParent<TeleportMarkerBase>();
			}
			if (flag2)
			{
				teleportMarkerBase = null;
			}
			HighlightSelected(teleportMarkerBase);
			Vector3 vector3;
			if (teleportMarkerBase != null)
			{
				if (teleportMarkerBase.locked)
				{
					teleportArc.SetColor(pointerLockedColor);
					pointerLineRenderer.startColor = pointerLockedColor;
					pointerLineRenderer.endColor = pointerLockedColor;
					destinationReticleTransform.gameObject.SetActive(value: false);
				}
				else
				{
					teleportArc.SetColor(pointerValidColor);
					pointerLineRenderer.startColor = pointerValidColor;
					pointerLineRenderer.endColor = pointerValidColor;
					destinationReticleTransform.gameObject.SetActive(teleportMarkerBase.showReticle);
				}
				offsetReticleTransform.gameObject.SetActive(value: true);
				invalidReticleTransform.gameObject.SetActive(value: false);
				pointedAtTeleportMarker = teleportMarkerBase;
				pointedAtPosition = hitInfo.point;
				if (showPlayAreaMarker)
				{
					TeleportArea teleportArea = pointedAtTeleportMarker as TeleportArea;
					if (teleportArea != null && !teleportArea.locked && playAreaPreviewTransform != null)
					{
						Vector3 vector2 = vector;
						if (!movedFeetFarEnough)
						{
							float num3 = Vector3.Distance(vector, startingFeetOffset);
							if (num3 < 0.1f)
							{
								vector2 = startingFeetOffset;
							}
							else if (num3 < 0.4f)
							{
								vector2 = Vector3.Lerp(startingFeetOffset, vector, (num3 - 0.1f) / 0.3f);
							}
							else
							{
								movedFeetFarEnough = true;
							}
						}
						playAreaPreviewTransform.position = pointedAtPosition + vector2;
						active = true;
					}
				}
				vector3 = hitInfo.point;
			}
			else
			{
				destinationReticleTransform.gameObject.SetActive(value: false);
				offsetReticleTransform.gameObject.SetActive(value: false);
				teleportArc.SetColor(pointerInvalidColor);
				pointerLineRenderer.startColor = pointerInvalidColor;
				pointerLineRenderer.endColor = pointerInvalidColor;
				invalidReticleTransform.gameObject.SetActive(!flag2);
				Vector3 toDirection = hitInfo.normal;
				float num4 = Vector3.Angle(hitInfo.normal, Vector3.up);
				if (num4 < 15f)
				{
					toDirection = Vector3.up;
				}
				invalidReticleTargetRotation = Quaternion.FromToRotation(Vector3.up, toDirection);
				invalidReticleTransform.rotation = Quaternion.Slerp(invalidReticleTransform.rotation, invalidReticleTargetRotation, 0.1f);
				float num5 = Vector3.Distance(hitInfo.point, player.hmdTransform.position);
				float num6 = Util.RemapNumberClamped(num5, invalidReticleMinScaleDistance, invalidReticleMaxScaleDistance, invalidReticleMinScale, invalidReticleMaxScale);
				invalidReticleScale.x = num6;
				invalidReticleScale.y = num6;
				invalidReticleScale.z = num6;
				invalidReticleTransform.transform.localScale = invalidReticleScale;
				pointedAtTeleportMarker = null;
				vector3 = ((!flag) ? teleportArc.GetArcPositionAtTime(teleportArc.arcDuration) : hitInfo.point);
				if (debugFloor)
				{
					floorDebugSphere.gameObject.SetActive(value: false);
					floorDebugLine.gameObject.SetActive(value: false);
				}
			}
			if (playAreaPreviewTransform != null)
			{
				playAreaPreviewTransform.gameObject.SetActive(active);
			}
			if (!showOffsetReticle)
			{
				offsetReticleTransform.gameObject.SetActive(value: false);
			}
			destinationReticleTransform.position = pointedAtPosition;
			invalidReticleTransform.position = vector3;
			onActivateObjectTransform.position = vector3;
			onDeactivateObjectTransform.position = vector3;
			offsetReticleTransform.position = vector3 - vector;
			reticleAudioSource.transform.position = pointedAtPosition;
			pointerLineRenderer.SetPosition(0, position);
			pointerLineRenderer.SetPosition(1, vector3);
		}

		private void FixedUpdate()
		{
			if (!visible || !debugFloor)
			{
				return;
			}
			TeleportArea teleportArea = pointedAtTeleportMarker as TeleportArea;
			if (teleportArea != null && floorFixupMaximumTraceDistance > 0f)
			{
				floorDebugSphere.gameObject.SetActive(value: true);
				floorDebugLine.gameObject.SetActive(value: true);
				Vector3 down = Vector3.down;
				down.x = 0.01f;
				if (Physics.Raycast(pointedAtPosition + 0.05f * down, down, out var hitInfo, floorFixupMaximumTraceDistance, floorFixupTraceLayerMask))
				{
					floorDebugSphere.transform.position = hitInfo.point;
					floorDebugSphere.material.color = Color.green;
					floorDebugLine.startColor = Color.green;
					floorDebugLine.endColor = Color.green;
					floorDebugLine.SetPosition(0, pointedAtPosition);
					floorDebugLine.SetPosition(1, hitInfo.point);
				}
				else
				{
					Vector3 position = pointedAtPosition + down * floorFixupMaximumTraceDistance;
					floorDebugSphere.transform.position = position;
					floorDebugSphere.material.color = Color.red;
					floorDebugLine.startColor = Color.red;
					floorDebugLine.endColor = Color.red;
					floorDebugLine.SetPosition(0, pointedAtPosition);
					floorDebugLine.SetPosition(1, position);
				}
			}
		}

		private void OnChaperoneInfoInitialized()
		{
			ChaperoneInfo instance = ChaperoneInfo.instance;
			if (instance.initialized && instance.roomscale)
			{
				if (playAreaPreviewTransform == null)
				{
					playAreaPreviewTransform = new GameObject("PlayAreaPreviewTransform").transform;
					playAreaPreviewTransform.parent = base.transform;
					Util.ResetTransform(playAreaPreviewTransform);
					playAreaPreviewCorner.SetActive(value: true);
					playAreaPreviewCorners = new Transform[4];
					playAreaPreviewCorners[0] = playAreaPreviewCorner.transform;
					playAreaPreviewCorners[1] = Object.Instantiate(playAreaPreviewCorners[0]);
					playAreaPreviewCorners[2] = Object.Instantiate(playAreaPreviewCorners[0]);
					playAreaPreviewCorners[3] = Object.Instantiate(playAreaPreviewCorners[0]);
					playAreaPreviewCorners[0].transform.parent = playAreaPreviewTransform;
					playAreaPreviewCorners[1].transform.parent = playAreaPreviewTransform;
					playAreaPreviewCorners[2].transform.parent = playAreaPreviewTransform;
					playAreaPreviewCorners[3].transform.parent = playAreaPreviewTransform;
					playAreaPreviewSide.SetActive(value: true);
					playAreaPreviewSides = new Transform[4];
					playAreaPreviewSides[0] = playAreaPreviewSide.transform;
					playAreaPreviewSides[1] = Object.Instantiate(playAreaPreviewSides[0]);
					playAreaPreviewSides[2] = Object.Instantiate(playAreaPreviewSides[0]);
					playAreaPreviewSides[3] = Object.Instantiate(playAreaPreviewSides[0]);
					playAreaPreviewSides[0].transform.parent = playAreaPreviewTransform;
					playAreaPreviewSides[1].transform.parent = playAreaPreviewTransform;
					playAreaPreviewSides[2].transform.parent = playAreaPreviewTransform;
					playAreaPreviewSides[3].transform.parent = playAreaPreviewTransform;
				}
				float playAreaSizeX = instance.playAreaSizeX;
				float playAreaSizeZ = instance.playAreaSizeZ;
				playAreaPreviewSides[0].localPosition = new Vector3(0f, 0f, 0.5f * playAreaSizeZ - 0.25f);
				playAreaPreviewSides[1].localPosition = new Vector3(0f, 0f, -0.5f * playAreaSizeZ + 0.25f);
				playAreaPreviewSides[2].localPosition = new Vector3(0.5f * playAreaSizeX - 0.25f, 0f, 0f);
				playAreaPreviewSides[3].localPosition = new Vector3(-0.5f * playAreaSizeX + 0.25f, 0f, 0f);
				playAreaPreviewSides[0].localScale = new Vector3(playAreaSizeX - 0.5f, 1f, 1f);
				playAreaPreviewSides[1].localScale = new Vector3(playAreaSizeX - 0.5f, 1f, 1f);
				playAreaPreviewSides[2].localScale = new Vector3(playAreaSizeZ - 0.5f, 1f, 1f);
				playAreaPreviewSides[3].localScale = new Vector3(playAreaSizeZ - 0.5f, 1f, 1f);
				playAreaPreviewSides[0].localRotation = Quaternion.Euler(0f, 0f, 0f);
				playAreaPreviewSides[1].localRotation = Quaternion.Euler(0f, 180f, 0f);
				playAreaPreviewSides[2].localRotation = Quaternion.Euler(0f, 90f, 0f);
				playAreaPreviewSides[3].localRotation = Quaternion.Euler(0f, 270f, 0f);
				playAreaPreviewCorners[0].localPosition = new Vector3(0.5f * playAreaSizeX - 0.25f, 0f, 0.5f * playAreaSizeZ - 0.25f);
				playAreaPreviewCorners[1].localPosition = new Vector3(0.5f * playAreaSizeX - 0.25f, 0f, -0.5f * playAreaSizeZ + 0.25f);
				playAreaPreviewCorners[2].localPosition = new Vector3(-0.5f * playAreaSizeX + 0.25f, 0f, -0.5f * playAreaSizeZ + 0.25f);
				playAreaPreviewCorners[3].localPosition = new Vector3(-0.5f * playAreaSizeX + 0.25f, 0f, 0.5f * playAreaSizeZ - 0.25f);
				playAreaPreviewCorners[0].localRotation = Quaternion.Euler(0f, 0f, 0f);
				playAreaPreviewCorners[1].localRotation = Quaternion.Euler(0f, 90f, 0f);
				playAreaPreviewCorners[2].localRotation = Quaternion.Euler(0f, 180f, 0f);
				playAreaPreviewCorners[3].localRotation = Quaternion.Euler(0f, 270f, 0f);
				playAreaPreviewTransform.gameObject.SetActive(value: false);
			}
		}

		private void HidePointer()
		{
			if (visible)
			{
				pointerHideStartTime = Time.time;
			}
			visible = false;
			if ((bool)pointerHand)
			{
				if (ShouldOverrideHoverLock())
				{
					if (originalHoverLockState)
					{
						pointerHand.HoverLock(originalHoveringInteractable);
					}
					else
					{
						pointerHand.HoverUnlock(null);
					}
				}
				loopingAudioSource.Stop();
				PlayAudioClip(pointerAudioSource, pointerStopSound);
			}
			teleportPointerObject.SetActive(value: false);
			teleportArc.Hide();
			TeleportMarkerBase[] array = teleportMarkers;
			foreach (TeleportMarkerBase teleportMarkerBase in array)
			{
				if (teleportMarkerBase != null && teleportMarkerBase.markerActive && teleportMarkerBase.gameObject != null)
				{
					teleportMarkerBase.gameObject.SetActive(value: false);
				}
			}
			destinationReticleTransform.gameObject.SetActive(value: false);
			invalidReticleTransform.gameObject.SetActive(value: false);
			offsetReticleTransform.gameObject.SetActive(value: false);
			if (playAreaPreviewTransform != null)
			{
				playAreaPreviewTransform.gameObject.SetActive(value: false);
			}
			if (onActivateObjectTransform.gameObject.activeSelf)
			{
				onActivateObjectTransform.gameObject.SetActive(value: false);
			}
			onDeactivateObjectTransform.gameObject.SetActive(value: true);
			pointerHand = null;
		}

		private void ShowPointer(Hand newPointerHand, Hand oldPointerHand)
		{
			if (!visible)
			{
				pointedAtTeleportMarker = null;
				pointerShowStartTime = Time.time;
				visible = true;
				meshFading = true;
				teleportPointerObject.SetActive(value: false);
				teleportArc.Show();
				TeleportMarkerBase[] array = teleportMarkers;
				foreach (TeleportMarkerBase teleportMarkerBase in array)
				{
					if (teleportMarkerBase.markerActive && teleportMarkerBase.ShouldActivate(player.feetPositionGuess))
					{
						teleportMarkerBase.gameObject.SetActive(value: true);
						teleportMarkerBase.Highlight(highlight: false);
					}
				}
				startingFeetOffset = player.trackingOriginTransform.position - player.feetPositionGuess;
				movedFeetFarEnough = false;
				if (onDeactivateObjectTransform.gameObject.activeSelf)
				{
					onDeactivateObjectTransform.gameObject.SetActive(value: false);
				}
				onActivateObjectTransform.gameObject.SetActive(value: true);
				loopingAudioSource.clip = pointerLoopSound;
				loopingAudioSource.loop = true;
				loopingAudioSource.Play();
				loopingAudioSource.volume = 0f;
			}
			if ((bool)oldPointerHand && ShouldOverrideHoverLock())
			{
				if (originalHoverLockState)
				{
					oldPointerHand.HoverLock(originalHoveringInteractable);
				}
				else
				{
					oldPointerHand.HoverUnlock(null);
				}
			}
			pointerHand = newPointerHand;
			if (visible && oldPointerHand != pointerHand)
			{
				PlayAudioClip(pointerAudioSource, pointerStartSound);
			}
			if ((bool)pointerHand)
			{
				pointerStartTransform = GetPointerStartTransform(pointerHand);
				if (pointerHand.currentAttachedObject != null)
				{
					allowTeleportWhileAttached = pointerHand.currentAttachedObject.GetComponent<AllowTeleportWhileAttachedToHand>();
				}
				originalHoverLockState = pointerHand.hoverLocked;
				originalHoveringInteractable = pointerHand.hoveringInteractable;
				if (ShouldOverrideHoverLock())
				{
					pointerHand.HoverLock(null);
				}
				pointerAudioSource.transform.SetParent(pointerStartTransform);
				pointerAudioSource.transform.localPosition = Vector3.zero;
				loopingAudioSource.transform.SetParent(pointerStartTransform);
				loopingAudioSource.transform.localPosition = Vector3.zero;
			}
		}

		private void UpdateTeleportColors()
		{
			float num = Time.time - pointerShowStartTime;
			if (num > meshFadeTime)
			{
				meshAlphaPercent = 1f;
				meshFading = false;
			}
			else
			{
				meshAlphaPercent = Mathf.Lerp(0f, 1f, num / meshFadeTime);
			}
			TeleportMarkerBase[] array = teleportMarkers;
			foreach (TeleportMarkerBase teleportMarkerBase in array)
			{
				teleportMarkerBase.SetAlpha(fullTintAlpha * meshAlphaPercent, meshAlphaPercent);
			}
		}

		private void PlayAudioClip(AudioSource source, AudioClip clip)
		{
			source.clip = clip;
			source.Play();
		}

		private void PlayPointerHaptic(bool validLocation)
		{
			if (pointerHand != null)
			{
				if (validLocation)
				{
					pointerHand.TriggerHapticPulse(800);
				}
				else
				{
					pointerHand.TriggerHapticPulse(100);
				}
			}
		}

		private void TryTeleportPlayer()
		{
			if (visible && !teleporting && pointedAtTeleportMarker != null && !pointedAtTeleportMarker.locked)
			{
				teleportingToMarker = pointedAtTeleportMarker;
				InitiateTeleportFade();
				CancelTeleportHint();
			}
		}

		private void InitiateTeleportFade()
		{
			teleporting = true;
			currentFadeTime = teleportFadeTime;
			TeleportPoint teleportPoint = teleportingToMarker as TeleportPoint;
			if (teleportPoint != null && teleportPoint.teleportType == TeleportPoint.TeleportPointType.SwitchToNewScene)
			{
				currentFadeTime *= 3f;
				ChangeScene.Send(currentFadeTime);
			}
			SteamVR_Fade.Start(Color.clear, 0f);
			SteamVR_Fade.Start(Color.black, currentFadeTime);
			headAudioSource.transform.SetParent(player.hmdTransform);
			headAudioSource.transform.localPosition = Vector3.zero;
			PlayAudioClip(headAudioSource, teleportSound);
			Invoke("TeleportPlayer", currentFadeTime);
		}

		private void TeleportPlayer()
		{
			teleporting = false;
			PlayerPre.Send(pointedAtTeleportMarker);
			SteamVR_Fade.Start(Color.clear, currentFadeTime);
			TeleportPoint teleportPoint = teleportingToMarker as TeleportPoint;
			Vector3 vector = pointedAtPosition;
			if (teleportPoint != null)
			{
				vector = teleportPoint.transform.position;
				if (teleportPoint.teleportType == TeleportPoint.TeleportPointType.SwitchToNewScene)
				{
					teleportPoint.TeleportToScene();
					return;
				}
			}
			TeleportArea teleportArea = teleportingToMarker as TeleportArea;
			if (teleportArea != null && floorFixupMaximumTraceDistance > 0f && Physics.Raycast(vector + 0.05f * Vector3.down, Vector3.down, out var hitInfo, floorFixupMaximumTraceDistance, floorFixupTraceLayerMask))
			{
				vector = hitInfo.point;
			}
			if (teleportingToMarker.ShouldMovePlayer())
			{
				Vector3 vector2 = player.trackingOriginTransform.position - player.feetPositionGuess;
				player.trackingOriginTransform.position = vector + vector2;
				if (player.leftHand.currentAttachedObjectInfo.HasValue)
				{
					player.leftHand.ResetAttachedTransform(player.leftHand.currentAttachedObjectInfo.Value);
				}
				if (player.rightHand.currentAttachedObjectInfo.HasValue)
				{
					player.rightHand.ResetAttachedTransform(player.rightHand.currentAttachedObjectInfo.Value);
				}
			}
			else
			{
				teleportingToMarker.TeleportPlayer(pointedAtPosition);
			}
			Player.Send(pointedAtTeleportMarker);
		}

		private void HighlightSelected(TeleportMarkerBase hitTeleportMarker)
		{
			if (pointedAtTeleportMarker != hitTeleportMarker)
			{
				if (pointedAtTeleportMarker != null)
				{
					pointedAtTeleportMarker.Highlight(highlight: false);
				}
				if (hitTeleportMarker != null)
				{
					hitTeleportMarker.Highlight(highlight: true);
					prevPointedAtPosition = pointedAtPosition;
					PlayPointerHaptic(!hitTeleportMarker.locked);
					PlayAudioClip(reticleAudioSource, goodHighlightSound);
					loopingAudioSource.volume = loopingAudioMaxVolume;
				}
				else if (pointedAtTeleportMarker != null)
				{
					PlayAudioClip(reticleAudioSource, badHighlightSound);
					loopingAudioSource.volume = 0f;
				}
			}
			else if (hitTeleportMarker != null && Vector3.Distance(prevPointedAtPosition, pointedAtPosition) > 1f)
			{
				prevPointedAtPosition = pointedAtPosition;
				PlayPointerHaptic(!hitTeleportMarker.locked);
			}
		}

		public void ShowTeleportHint()
		{
			CancelTeleportHint();
			hintCoroutine = StartCoroutine(TeleportHintCoroutine());
		}

		public void CancelTeleportHint()
		{
			if (hintCoroutine != null)
			{
				ControllerButtonHints.HideTextHint(player.leftHand, teleportAction);
				ControllerButtonHints.HideTextHint(player.rightHand, teleportAction);
				StopCoroutine(hintCoroutine);
				hintCoroutine = null;
			}
			CancelInvoke("ShowTeleportHint");
		}

		private IEnumerator TeleportHintCoroutine()
		{
			float prevBreakTime = Time.time;
			float prevHapticPulseTime = Time.time;
			while (true)
			{
				bool pulsed = false;
				Hand[] hands = player.hands;
				foreach (Hand hand in hands)
				{
					bool flag = IsEligibleForTeleport(hand);
					bool flag2 = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, teleportAction));
					if (flag)
					{
						if (!flag2)
						{
							ControllerButtonHints.ShowTextHint(hand, teleportAction, "Teleport");
							prevBreakTime = Time.time;
							prevHapticPulseTime = Time.time;
						}
						if (Time.time > prevHapticPulseTime + 0.05f)
						{
							pulsed = true;
							hand.TriggerHapticPulse(500);
						}
					}
					else if (!flag && flag2)
					{
						ControllerButtonHints.HideTextHint(hand, teleportAction);
					}
				}
				if (Time.time > prevBreakTime + 3f)
				{
					yield return new WaitForSeconds(3f);
					prevBreakTime = Time.time;
				}
				if (pulsed)
				{
					prevHapticPulseTime = Time.time;
				}
				yield return null;
			}
		}

		public bool IsEligibleForTeleport(Hand hand)
		{
			if (hand == null)
			{
				return false;
			}
			if (!hand.gameObject.activeInHierarchy)
			{
				return false;
			}
			if (hand.hoveringInteractable != null)
			{
				return false;
			}
			if (hand.noSteamVRFallbackCamera == null)
			{
				if (!hand.isActive)
				{
					return false;
				}
				if (hand.currentAttachedObject != null)
				{
					AllowTeleportWhileAttachedToHand component = hand.currentAttachedObject.GetComponent<AllowTeleportWhileAttachedToHand>();
					if (component != null && component.teleportAllowed)
					{
						return true;
					}
					return false;
				}
			}
			return true;
		}

		private bool ShouldOverrideHoverLock()
		{
			if (!allowTeleportWhileAttached || allowTeleportWhileAttached.overrideHoverLock)
			{
				return true;
			}
			return false;
		}

		private bool WasTeleportButtonReleased(Hand hand)
		{
			if (IsEligibleForTeleport(hand))
			{
				if (hand.noSteamVRFallbackCamera != null)
				{
					return Input.GetKeyUp(KeyCode.T);
				}
				return teleportAction.GetStateUp(hand.handType);
			}
			return false;
		}

		private bool IsTeleportButtonDown(Hand hand)
		{
			if (IsEligibleForTeleport(hand))
			{
				if (hand.noSteamVRFallbackCamera != null)
				{
					return Input.GetKey(KeyCode.T);
				}
				return teleportAction.GetState(hand.handType);
			}
			return false;
		}

		private bool WasTeleportButtonPressed(Hand hand)
		{
			if (IsEligibleForTeleport(hand))
			{
				if (hand.noSteamVRFallbackCamera != null)
				{
					return Input.GetKeyDown(KeyCode.T);
				}
				return teleportAction.GetStateDown(hand.handType);
			}
			return false;
		}

		private Transform GetPointerStartTransform(Hand hand)
		{
			if (hand.noSteamVRFallbackCamera != null)
			{
				return hand.noSteamVRFallbackCamera.transform;
			}
			return hand.transform;
		}
	}
}
