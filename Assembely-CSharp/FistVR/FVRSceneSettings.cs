using System.Collections.Generic;
using LIV.SDK.Unity;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace FistVR
{
	public class FVRSceneSettings : MonoBehaviour
	{
		public delegate void PlayerDeath(bool killedSelf);

		public delegate void PlayerTookDamage(float percentOfLife);

		public delegate void BotKill(Damage dam);

		public delegate void SosigKill(Sosig s);

		public delegate void ShotFired(FVRFireArm firearm);

		public delegate void BotShotFired(wwBotWurstModernGun gun);

		public delegate void PowerupUse(PowerupType type);

		public delegate void FVRObjectPickedUp(FVRPhysicalObject obj);

		public delegate void FireArmReloaded(FVRObject obj);

		public delegate void PerceiveableSound(float loudness, float maxDistanceHeard, Vector3 pos, int iffcode);

		public delegate void SuppressingEvent(Vector3 pos, Vector3 dir, int iffcode, float intensity, float range);

		[Header("PlayerAffordance")]
		public bool IsSpawnLockingEnabled = true;

		public bool AreHitboxesEnabled;

		public bool DoesDamageGetRegistered;

		public float MaxPointingDistance = 1f;

		public float MaxProjectileRange = 500f;

		public bool ForcesCasingDespawn;

		[Header("Locomotion Options")]
		public bool IsLocoTeleportAllowed = true;

		public bool IsLocoSlideAllowed = true;

		public bool IsLocoDashAllowed = true;

		public bool IsLocoTouchpadAllowed = true;

		public bool IsLocoArmSwingerAllowed = true;

		public bool DoesTeleportUseCooldown;

		public bool DoesAllowAirControl;

		public bool UsesMaxSpeedClamp;

		public float MaxSpeedClamp = 3f;

		[Header("Player Catching Options")]
		public bool UsesPlayerCatcher = true;

		public float CatchHeight = -50f;

		[Header("Player Respawn Options")]
		public int DefaultPlayerIFF;

		public bool DoesPlayerRespawnOnDeath = true;

		public float PlayerDeathFade = 3f;

		public float PlayerRespawnLoadDelay = 3.5f;

		public string SceneToLoadOnDeath = string.Empty;

		public bool DoesUseHealthBar;

		public bool IsQuickbeltSwappingAllowed = true;

		public bool AreQuickbeltSlotsEnabled;

		public bool ConfigQuickbeltOnLoad;

		public int QuickbeltToConfig;

		public bool IsSceneLowLight;

		public bool IsAmmoInfinite;

		public bool AllowsInfiniteAmmoMags = true;

		public bool UsesUnlockSystem;

		public Transform DeathResetPoint;

		public GameObject OnDeathSendMessageTarget;

		public string OneDeathSendMessage;

		public List<GameObject> ShotEventReceivers;

		public int HowManyToShotReceivePerFrame = 10;

		private HashSet<GameObject> m_shotEventReceiversInQueue = new HashSet<GameObject>();

		private Queue<GameObject> m_shotEventReceiversQueue = new Queue<GameObject>();

		private Vector3 m_curNotifyPos = Vector3.zero;

		[Header("QuitReceivers")]
		public List<GameObject> QuitReceivers;

		[Header("Audio Stuff")]
		public FVRSoundEnvironment DefaultSoundEnvironment;

		public float BaseLoudness = 5f;

		public bool UsesWeaponHandlingAISound;

		public float MaxImpactSoundEventDistance = 15f;

		[Header("Post FX Volumes")]
		public PostProcessVolume Vol_AO;

		public PostProcessVolume Vol_Bloom;

		public PostProcessVolume Vol_CC;

		public List<GameObject> Post_SnakeEyes;

		public List<GameObject> Post_MoleEye;

		public List<GameObject> Post_Blort;

		public List<GameObject> Post_Dlort;

		public List<GameObject> Post_FarOutMan;

		public List<GameObject> Post_BadTrip;

		[Header("PowerUpStuff")]
		public Transform PowerupPoint_HomeTown;

		public Transform PowerupPoint_InverseHomeTown;

		public List<Transform> PowerPoints_WheredIGo_TP;

		public List<Transform> PowerPoints_WheredIGo_Grav;

		private Camera m_specCam;

		private Camera m_previewCam;

		private PostProcessLayer m_postLayer;

		private Vector3 smoothVel = Vector3.zero;

		private Vector3 m_camForward = Vector3.zero;

		private Vector3 m_camForwardVel = Vector3.zero;

		private Vector3 m_camUp = Vector3.zero;

		private Vector3 m_camUpVel = Vector3.zero;

		private Vector3 localPosHead = Vector3.one;

		private SosigLink m_attachedLink;

		private Transform m_camObjectPoint;

		private float[] TPCDistances = new float[5]
		{
			0.5f,
			0.75f,
			1f,
			1.25f,
			1.5f
		};

		private float[] TPCSideOffsets = new float[5]
		{
			0.18f,
			0.25f,
			0.35f,
			0.45f,
			0.55f
		};

		private RaycastHit m_hit;

		private int numSuppressingEventsThisFrame;

		public event PlayerDeath PlayerDeathEvent;

		public event PlayerTookDamage PlayerTookDamageEvent;

		public event BotKill KillEvent;

		public event SosigKill SosigKillEvent;

		public event ShotFired ShotFiredEvent;

		public event BotShotFired BotShotFiredEvent;

		public event PowerupUse PowerupUseEvent;

		public event FVRObjectPickedUp ObjectPickedUpEvent;

		public event FireArmReloaded FireArmReloadedEvent;

		public event PerceiveableSound PerceiveableSoundEvent;

		public event SuppressingEvent SuppressingEventEvent;

		private void Awake()
		{
			ManagerBootStrap.BootStrap();
		}

		private void Start()
		{
			GM.RefreshQuality();
			SM.WarmupGenericPools();
			SM.SetReverbEnvironment(DefaultSoundEnvironment);
			Invoke("ConfigLiv", 0.1f);
			if (ConfigQuickbeltOnLoad)
			{
				Invoke("DelayedConfig", 0.2f);
			}
			if (PowerupPoint_HomeTown == null)
			{
				PowerupPoint_HomeTown = DeathResetPoint;
			}
			if (PowerupPoint_InverseHomeTown == null)
			{
				PowerupPoint_HomeTown = PowerupPoint_InverseHomeTown;
			}
			if (PowerPoints_WheredIGo_TP.Count < 1)
			{
				PowerPoints_WheredIGo_TP.Add(PowerupPoint_InverseHomeTown);
			}
			if (PowerPoints_WheredIGo_Grav.Count < 1)
			{
				PowerPoints_WheredIGo_Grav.Add(PowerupPoint_InverseHomeTown);
			}
			Debug.Log("Scene delta is:" + Time.fixedDeltaTime);
		}

		private void DelayedConfig()
		{
			GM.CurrentPlayerBody.ConfigureQuickbelt(QuickbeltToConfig);
		}

		public void SetAttachedLink(SosigLink l)
		{
			m_attachedLink = l;
		}

		public void SetCamObjectPoint(Transform t)
		{
			m_camObjectPoint = t;
		}

		public Transform GetCamObjectPoint()
		{
			return m_camObjectPoint;
		}

		private void UpdateSpectatorCamera()
		{
			if (m_specCam != null)
			{
				switch (GM.Options.ControlOptions.CamQual)
				{
				case ControlOptions.DesktopRenderQuality.Low:
					m_postLayer.enabled = false;
					m_specCam.allowMSAA = false;
					break;
				case ControlOptions.DesktopRenderQuality.Med:
					m_postLayer.enabled = false;
					m_specCam.allowMSAA = true;
					break;
				case ControlOptions.DesktopRenderQuality.High:
					m_postLayer.enabled = true;
					m_specCam.allowMSAA = true;
					break;
				}
			}
			ControlOptions.DesktopCameraMode camMode = GM.Options.ControlOptions.CamMode;
			float num = 0.032f;
			if (GM.Options.ControlOptions.CamEye == ControlOptions.DesktopCameraEye.Left)
			{
				num = 0f - num;
			}
			float camFOV = GM.Options.ControlOptions.CamFOV;
			float camSmoothingLinear = GM.Options.ControlOptions.CamSmoothingLinear;
			float camSmoothingRotational = GM.Options.ControlOptions.CamSmoothingRotational;
			float camLeveling = GM.Options.ControlOptions.CamLeveling;
			Vector3 up = Vector3.up;
			Vector3 b = Vector3.up;
			switch (camMode)
			{
			case ControlOptions.DesktopCameraMode.Default:
				if (m_specCam != null && m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: false);
				}
				break;
			case ControlOptions.DesktopCameraMode.HDSpectator:
				if (m_specCam == null)
				{
					GameObject gameObject2 = Object.Instantiate(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
					m_specCam = gameObject2.GetComponent<Camera>();
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					m_camUp = GM.CurrentPlayerBody.Head.up;
					localPosHead = GM.CurrentPlayerBody.Head.localPosition;
					m_postLayer = gameObject2.GetComponent<PostProcessLayer>();
				}
				if (!m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: true);
				}
				if (camSmoothingLinear < 0.1f)
				{
					m_specCam.transform.position = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.right * num;
				}
				else
				{
					localPosHead = Vector3.SmoothDamp(localPosHead, GM.CurrentPlayerBody.Head.localPosition + GM.CurrentPlayerBody.transform.InverseTransformDirection(GM.CurrentPlayerBody.Head.right) * num, ref smoothVel, camSmoothingLinear * 0.1f);
					m_specCam.transform.position = GM.CurrentPlayerBody.transform.TransformPoint(localPosHead);
				}
				if (camSmoothingRotational < 0.1f)
				{
					m_camUp = GM.CurrentPlayerBody.Head.up;
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					up = GM.CurrentPlayerBody.Head.up;
				}
				else
				{
					up = GM.CurrentPlayerBody.Head.up;
					m_camForward = Vector3.SmoothDamp(m_camForward, GM.CurrentPlayerBody.Head.forward, ref m_camForwardVel, camSmoothingRotational * 0.1f);
					m_camUp = Vector3.SmoothDamp(m_camUp, GM.CurrentPlayerBody.Head.up, ref m_camUpVel, camSmoothingRotational * 0.1f);
				}
				b = Vector3.up;
				if (Vector3.Dot(m_camUp, -Vector3.up) > 0f)
				{
					b = -Vector3.up;
				}
				up = Vector3.Slerp(m_camUp, b, camLeveling * 0.25f);
				m_specCam.transform.rotation = Quaternion.LookRotation(m_camForward, up);
				m_specCam.fieldOfView = Mathf.MoveTowards(m_specCam.fieldOfView, camFOV, Time.deltaTime * 15f);
				break;
			case ControlOptions.DesktopCameraMode.SpawnedObject:
				if (m_specCam == null)
				{
					GameObject gameObject3 = Object.Instantiate(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
					m_specCam = gameObject3.GetComponent<Camera>();
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					m_camUp = GM.CurrentPlayerBody.Head.up;
					localPosHead = GM.CurrentPlayerBody.Head.localPosition;
					m_postLayer = gameObject3.GetComponent<PostProcessLayer>();
				}
				if (!m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: true);
				}
				if (m_camObjectPoint != null)
				{
					if (camSmoothingLinear < 0.1f)
					{
						m_specCam.transform.position = m_camObjectPoint.position;
					}
					else
					{
						localPosHead = Vector3.SmoothDamp(localPosHead, m_camObjectPoint.position, ref smoothVel, camSmoothingLinear * 0.1f);
						m_specCam.transform.position = localPosHead;
					}
					if (camSmoothingRotational < 0.1f)
					{
						m_camUp = m_camObjectPoint.up;
						m_camForward = m_camObjectPoint.forward;
						up = m_camObjectPoint.up;
					}
					else
					{
						up = m_camObjectPoint.up;
						m_camForward = Vector3.SmoothDamp(m_camForward, m_camObjectPoint.forward, ref m_camForwardVel, camSmoothingRotational * 0.1f);
						m_camUp = Vector3.SmoothDamp(m_camUp, m_camObjectPoint.up, ref m_camUpVel, camSmoothingRotational * 0.1f);
					}
					if (Vector3.Dot(m_camUp, -Vector3.up) > 0f)
					{
						b = -Vector3.up;
					}
					up = Vector3.Slerp(m_camUp, b, camLeveling * 0.25f);
					m_specCam.transform.rotation = Quaternion.LookRotation(m_camForward, up);
					m_specCam.fieldOfView = Mathf.MoveTowards(m_specCam.fieldOfView, camFOV, Time.deltaTime * 25f);
				}
				else if (m_specCam != null && m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: false);
				}
				break;
			case ControlOptions.DesktopCameraMode.ThirdPerson:
			{
				if (m_specCam == null)
				{
					GameObject gameObject4 = Object.Instantiate(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
					m_specCam = gameObject4.GetComponent<Camera>();
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					m_camUp = GM.CurrentPlayerBody.Head.up;
					localPosHead = GM.CurrentPlayerBody.Head.localPosition;
					m_postLayer = gameObject4.GetComponent<PostProcessLayer>();
				}
				if (!m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: true);
				}
				Vector3 zero = Vector3.zero;
				if (camSmoothingLinear < 0.1f)
				{
					zero = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.right * num;
				}
				else
				{
					localPosHead = Vector3.SmoothDamp(localPosHead, GM.CurrentPlayerBody.Head.localPosition + GM.CurrentPlayerBody.transform.InverseTransformDirection(GM.CurrentPlayerBody.Head.right) * num, ref smoothVel, camSmoothingLinear * 0.1f);
					zero = GM.CurrentPlayerBody.transform.TransformPoint(localPosHead);
				}
				if (camSmoothingRotational < 0.1f)
				{
					m_camUp = GM.CurrentPlayerBody.Head.up;
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					up = GM.CurrentPlayerBody.Head.up;
				}
				else
				{
					up = GM.CurrentPlayerBody.Head.up;
					m_camForward = Vector3.SmoothDamp(m_camForward, GM.CurrentPlayerBody.Head.forward, ref m_camForwardVel, camSmoothingRotational * 0.1f);
					m_camUp = Vector3.SmoothDamp(m_camUp, GM.CurrentPlayerBody.Head.up, ref m_camUpVel, camSmoothingRotational * 0.1f);
				}
				b = Vector3.up;
				if (Vector3.Dot(m_camUp, -Vector3.up) > 0f)
				{
					b = -Vector3.up;
				}
				up = Vector3.Slerp(m_camUp, b, camLeveling * 0.25f);
				m_specCam.transform.rotation = Quaternion.LookRotation(m_camForward, up);
				m_specCam.fieldOfView = Mathf.MoveTowards(m_specCam.fieldOfView, camFOV, Time.deltaTime * 25f);
				int tPCDistanceIndex = GM.Options.ControlOptions.TPCDistanceIndex;
				int tPCLateralIndex = GM.Options.ControlOptions.TPCLateralIndex;
				float num2 = TPCDistances[tPCDistanceIndex];
				if (Physics.Raycast(zero, -m_camForward, out m_hit, num2, GM.CurrentMovementManager.LM_PointSearch, QueryTriggerInteraction.Ignore))
				{
					num2 = m_hit.distance;
				}
				Vector3 vector = zero + -m_camForward * num2;
				float num3 = TPCSideOffsets[tPCLateralIndex];
				Vector3 vector2 = m_specCam.transform.right;
				if (GM.Options.ControlOptions.CamEye == ControlOptions.DesktopCameraEye.Left)
				{
					vector2 = -vector2;
				}
				if (Physics.Raycast(vector, vector2, out m_hit, num3, GM.CurrentMovementManager.LM_PointSearch, QueryTriggerInteraction.Ignore))
				{
					num3 = m_hit.distance;
				}
				vector += vector2 * num3;
				m_specCam.transform.position = vector;
				break;
			}
			case ControlOptions.DesktopCameraMode.SosigView:
				if (m_specCam == null)
				{
					GameObject gameObject = Object.Instantiate(ManagerSingleton<GM>.Instance.SpectatorCamPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
					m_specCam = gameObject.GetComponent<Camera>();
					m_camForward = GM.CurrentPlayerBody.Head.forward;
					m_camUp = GM.CurrentPlayerBody.Head.up;
					localPosHead = GM.CurrentPlayerBody.Head.localPosition;
					m_postLayer = gameObject.GetComponent<PostProcessLayer>();
				}
				if (!m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: true);
				}
				if (m_attachedLink != null)
				{
					if (camSmoothingLinear < 0.1f)
					{
						m_specCam.transform.position = m_attachedLink.transform.position;
					}
					else
					{
						localPosHead = Vector3.SmoothDamp(localPosHead, m_attachedLink.transform.position, ref smoothVel, camSmoothingLinear * 0.1f);
						m_specCam.transform.position = localPosHead;
					}
					if (camSmoothingRotational < 0.1f)
					{
						m_camUp = m_attachedLink.transform.up;
						m_camForward = m_attachedLink.transform.forward;
						up = m_attachedLink.transform.up;
					}
					else
					{
						up = m_attachedLink.transform.up;
						m_camForward = Vector3.SmoothDamp(m_camForward, m_attachedLink.transform.forward, ref m_camForwardVel, camSmoothingRotational * 0.1f);
						m_camUp = Vector3.SmoothDamp(m_camUp, m_attachedLink.transform.up, ref m_camUpVel, camSmoothingRotational * 0.1f);
					}
					if (Vector3.Dot(m_camUp, -Vector3.up) > 0f)
					{
						b = -Vector3.up;
					}
					up = Vector3.Slerp(m_camUp, b, camLeveling * 0.25f);
					m_specCam.transform.rotation = Quaternion.LookRotation(m_camForward, up);
					m_specCam.fieldOfView = Mathf.MoveTowards(m_specCam.fieldOfView, camFOV, Time.deltaTime * 25f);
				}
				else if (m_specCam != null && m_specCam.gameObject.activeSelf)
				{
					m_specCam.gameObject.SetActive(value: false);
				}
				break;
			}
			if (m_specCam != null && m_specCam.gameObject.activeSelf && GM.Options.ControlOptions.PCamMode == ControlOptions.PreviewCamMode.Enabled)
			{
				if (m_previewCam == null)
				{
					GameObject gameObject5 = Object.Instantiate(ManagerSingleton<GM>.Instance.SpectatorCamPreviewPrefab, GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
					m_previewCam = gameObject5.GetComponent<Camera>();
				}
				m_previewCam.fieldOfView = m_specCam.fieldOfView;
				m_previewCam.transform.position = m_specCam.transform.position;
				m_previewCam.transform.rotation = m_specCam.transform.rotation;
				if (!m_previewCam.gameObject.activeSelf)
				{
					m_previewCam.gameObject.SetActive(value: true);
				}
			}
			else if (m_previewCam != null && m_previewCam.gameObject.activeSelf)
			{
				m_previewCam.gameObject.SetActive(value: false);
			}
		}

		private void ConfigLiv()
		{
			Debug.Log("Prepping LIV Prefab");
			LIV.SDK.Unity.LIV component = Object.Instantiate(ManagerSingleton<GM>.Instance.LivPrefab, Vector3.zero, Quaternion.identity).GetComponent<LIV.SDK.Unity.LIV>();
			component.TrackedSpaceOrigin = GM.CurrentPlayerRoot.transform;
			component.HMDCamera = GM.CurrentPlayerBody.EyeCam;
			component.enabled = true;
			component.gameObject.SetActive(value: true);
			Debug.Log("LIV Prefab Deployed");
		}

		private void OnDestroy()
		{
			FXM.ClearDecalPools();
			SM.ClearGenericPools();
		}

		public void SetSnakeEyes(bool b)
		{
			for (int i = 0; i < Post_SnakeEyes.Count; i++)
			{
				Post_SnakeEyes[i].SetActive(b);
			}
		}

		public void SetMoleEye(bool b)
		{
			for (int i = 0; i < Post_MoleEye.Count; i++)
			{
				Post_MoleEye[i].SetActive(b);
			}
		}

		public void SetBlort(bool b)
		{
			for (int i = 0; i < Post_Blort.Count; i++)
			{
				Post_Blort[i].SetActive(b);
			}
		}

		public void SetDlort(bool b)
		{
			for (int i = 0; i < Post_Dlort.Count; i++)
			{
				Post_Dlort[i].SetActive(b);
			}
		}

		public void SetFarOutMan(bool b)
		{
			for (int i = 0; i < Post_FarOutMan.Count; i++)
			{
				Post_FarOutMan[i].SetActive(b);
			}
		}

		public void SetBadTrip(bool b)
		{
			for (int i = 0; i < Post_BadTrip.Count; i++)
			{
				Post_BadTrip[i].SetActive(b);
			}
		}

		public void Init()
		{
		}

		public void PingReceivers(Vector3 pos)
		{
			if (ShotEventReceivers.Count < 1)
			{
				return;
			}
			m_curNotifyPos = pos;
			for (int i = 0; i < ShotEventReceivers.Count; i++)
			{
				if (ShotEventReceivers[i] != null && !m_shotEventReceiversInQueue.Contains(ShotEventReceivers[i]))
				{
					m_shotEventReceiversInQueue.Add(ShotEventReceivers[i]);
					m_shotEventReceiversQueue.Enqueue(ShotEventReceivers[i]);
				}
			}
		}

		public void UpdateGlobalPostVolumes()
		{
			Debug.Log("Updating Global Post Volumes:");
			Debug.Log("AO:" + GM.Options.PerformanceOptions.IsPostEnabled_AO);
			Debug.Log("CC:" + GM.Options.PerformanceOptions.IsPostEnabled_CC);
			Debug.Log("BL:" + GM.Options.PerformanceOptions.IsPostEnabled_Bloom);
			if (Vol_AO != null)
			{
				if (GM.Options.PerformanceOptions.IsPostEnabled_AO)
				{
					Vol_AO.enabled = true;
				}
				else
				{
					Vol_AO.enabled = false;
				}
			}
			if (Vol_CC != null)
			{
				if (GM.Options.PerformanceOptions.IsPostEnabled_CC)
				{
					Vol_CC.enabled = true;
				}
				else
				{
					Vol_CC.enabled = false;
				}
			}
			if (Vol_Bloom != null)
			{
				if (GM.Options.PerformanceOptions.IsPostEnabled_Bloom)
				{
					Vol_Bloom.enabled = true;
				}
				else
				{
					Vol_Bloom.enabled = false;
				}
			}
		}

		public void Update()
		{
			numSuppressingEventsThisFrame = 0;
			for (int i = 0; i < HowManyToShotReceivePerFrame; i++)
			{
				if (m_shotEventReceiversQueue.Count > 0)
				{
					GameObject gameObject = m_shotEventReceiversQueue.Dequeue();
					m_shotEventReceiversInQueue.Remove(gameObject);
					if (gameObject != null)
					{
						gameObject.SendMessage("ShotEvent", m_curNotifyPos, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			if (UsesPlayerCatcher)
			{
				float y = GM.CurrentPlayerBody.transform.position.y;
				if (y < CatchHeight)
				{
					GM.CurrentMovementManager.TeleportToPoint(DeathResetPoint.position, isAbsolute: true, DeathResetPoint.forward);
					GM.CurrentMovementManager.CleanupFlagsForModeSwitch();
				}
			}
			UpdateSpectatorCamera();
		}

		public void OnPlayerDeath(bool killedSelf)
		{
			if (this.PlayerDeathEvent != null)
			{
				this.PlayerDeathEvent(killedSelf);
			}
		}

		public void OnPlayerTookDamage(float percentOfLife)
		{
			if (this.PlayerTookDamageEvent != null)
			{
				this.PlayerTookDamageEvent(percentOfLife);
			}
		}

		public void OnBotKill(Damage dam)
		{
			if (this.KillEvent != null)
			{
				this.KillEvent(dam);
			}
		}

		public void OnSosigKill(Sosig s)
		{
			if (this.SosigKillEvent != null)
			{
				this.SosigKillEvent(s);
			}
		}

		public void OnShotFired(FVRFireArm firearm)
		{
			if (this.ShotFiredEvent != null)
			{
				this.ShotFiredEvent(firearm);
			}
		}

		public void OnBotShotFired(wwBotWurstModernGun gun)
		{
			if (this.BotShotFiredEvent != null)
			{
				this.BotShotFiredEvent(gun);
			}
		}

		public void OnPowerupUse(PowerupType type)
		{
			if (this.PowerupUseEvent != null)
			{
				this.PowerupUseEvent(type);
			}
		}

		public void OnFVRObjectPickedUp(FVRPhysicalObject obj)
		{
			if (this.ObjectPickedUpEvent != null)
			{
				this.ObjectPickedUpEvent(obj);
			}
		}

		public void OnFireArmReloaded(FVRObject obj)
		{
			if (this.FireArmReloadedEvent != null)
			{
				this.FireArmReloadedEvent(obj);
			}
		}

		public void OnPerceiveableSound(float loudness, float maxDistanceHeard, Vector3 pos, int iffcode)
		{
			if (this.PerceiveableSoundEvent != null)
			{
				this.PerceiveableSoundEvent(loudness, maxDistanceHeard, pos, iffcode);
			}
		}

		public void OnSuppressingEvent(Vector3 pos, Vector3 dir, int iffcode, float intensity, float range)
		{
			if (this.SuppressingEventEvent != null && numSuppressingEventsThisFrame < 3)
			{
				numSuppressingEventsThisFrame++;
				this.SuppressingEventEvent(pos, dir, iffcode, intensity, range);
			}
		}
	}
}
