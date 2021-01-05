using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Amplifier : FVRFireArmAttachmentInterface
	{
		[Serializable]
		public class ZoomSetting
		{
			public float Magnification;

			public float AngleBlueStrength;

			public float CutoffSoftness;

			public float AngularOccludeSensitivity;

			public Vector3 ZoomPiecePosRot;
		}

		public ScopeCam ScopeCam;

		public bool DoesFlip;

		public float FlipUp;

		public float FlipDown;

		private float m_curFlip;

		private bool m_flippedUp = true;

		public Transform FlipPart;

		[Header("Zoom Shit")]
		public int m_zoomSettingIndex;

		public List<ZoomSetting> ZoomSettings;

		public bool UsesZoomPiece;

		public Transform ZoomPiece;

		[Header("Zero Shit")]
		public int ZeroDistanceIndex;

		public List<float> ZeroDistances = new List<float>
		{
			100f,
			150f,
			200f,
			250f,
			300f,
			350f,
			400f,
			450f,
			500f,
			600f,
			700f,
			800f,
			900f,
			1000f
		};

		public int ElevationStep;

		public int WindageStep;

		public Transform TargetAimer;

		public Transform BackupMuzzle;

		public FVRFireArm BackupFireArm;

		public Transform UISpawnPoint;

		public OpticUI UI;

		public List<OpticOptionType> OptionTypes;

		public int CurSelectedOptionIndex;

		private bool isOnGrab = true;

		private bool isUIActive;

		protected override void Awake()
		{
			base.Awake();
			UpdateScopeCam();
			GameObject gameObject = UnityEngine.Object.Instantiate(ManagerSingleton<AM>.Instance.Prefab_OpticUI, UISpawnPoint.position, UISpawnPoint.rotation);
			UI = gameObject.GetComponent<OpticUI>();
			UI.UpdateUI(this);
			UI.SetAmp(this);
			gameObject.SetActive(value: false);
			UXGeo_Held = gameObject;
		}

		protected override void Start()
		{
			base.Start();
			Zero();
			UpdateScopeCam();
		}

		private void CheckOnGrab()
		{
			if (GM.Options.ControlOptions.CCM == ControlOptions.CoreControlMode.Standard)
			{
				if (!isOnGrab)
				{
					isOnGrab = true;
					UXGeo_Held = UI.gameObject;
					isUIActive = false;
				}
			}
			else if (isOnGrab)
			{
				isOnGrab = false;
				UXGeo_Held = null;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			CheckOnGrab();
			if (isUIActive || base.IsHeld)
			{
				UI.transform.position = UISpawnPoint.position;
				UI.transform.rotation = UISpawnPoint.rotation;
			}
			if (Attachment != null)
			{
				if (Attachment.curMount != null && Attachment.curMount.Parent != null)
				{
					if (Attachment.curMount.Parent.IsHeld || Attachment.curMount.Parent.IsPivotLocked || (Attachment.curMount.Parent.Bipod != null && Attachment.curMount.Parent.Bipod.IsBipodActive))
					{
						ScopeCam.MagnificationEnabled = true;
					}
					else
					{
						ScopeCam.MagnificationEnabled = false;
					}
				}
				else
				{
					ScopeCam.MagnificationEnabled = false;
				}
			}
			else if (BackupFireArm.IsHeld || BackupFireArm.IsPivotLocked || (BackupFireArm.Bipod != null && BackupFireArm.Bipod.IsBipodActive))
			{
				ScopeCam.MagnificationEnabled = true;
			}
			else
			{
				ScopeCam.MagnificationEnabled = false;
			}
			if (DoesFlip)
			{
				if (m_flippedUp)
				{
					m_curFlip = Mathf.Lerp(m_curFlip, FlipUp, Time.deltaTime * 4f);
				}
				else
				{
					m_curFlip = Mathf.Lerp(m_curFlip, FlipDown, Time.deltaTime * 4f);
				}
				FlipPart.localEulerAngles = new Vector3(0f, 0f, m_curFlip);
			}
		}

		private void UpdateScopeCam()
		{
			ScopeCam.Magnification = ZoomSettings[m_zoomSettingIndex].Magnification;
			ScopeCam.AngleBlurStrength = ZoomSettings[m_zoomSettingIndex].AngleBlueStrength;
			ScopeCam.CutoffSoftness = ZoomSettings[m_zoomSettingIndex].CutoffSoftness;
			ScopeCam.AngularOccludeSensitivity = ZoomSettings[m_zoomSettingIndex].AngularOccludeSensitivity;
			if (UsesZoomPiece)
			{
				ZoomPiece.localEulerAngles = ZoomSettings[m_zoomSettingIndex].ZoomPiecePosRot;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					isUIActive = !isUIActive;
					UI.gameObject.SetActive(isUIActive);
				}
			}
			else if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f)
			{
				if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
				{
					SetCurSettingDown();
					UI.UpdateUI(this);
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f)
				{
					SetCurSettingUp(cycle: false);
					UI.UpdateUI(this);
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
				{
					GoToNextSetting();
					UI.UpdateUI(this);
				}
			}
			base.UpdateInteraction(hand);
		}

		public void SetCurSettingUp(bool cycle)
		{
			switch (OptionTypes[CurSelectedOptionIndex])
			{
			case OpticOptionType.Zero:
				ZeroDistanceIndex++;
				if (cycle)
				{
					if (ZeroDistanceIndex >= ZeroDistances.Count)
					{
						ZeroDistanceIndex = 0;
					}
				}
				else
				{
					ZeroDistanceIndex = Mathf.Clamp(ZeroDistanceIndex, 0, ZeroDistances.Count - 1);
				}
				Zero();
				break;
			case OpticOptionType.Magnification:
				m_zoomSettingIndex++;
				if (cycle)
				{
					if (m_zoomSettingIndex >= ZoomSettings.Count)
					{
						m_zoomSettingIndex = 0;
					}
				}
				else
				{
					m_zoomSettingIndex = Mathf.Clamp(m_zoomSettingIndex, 0, ZoomSettings.Count - 1);
				}
				UpdateScopeCam();
				break;
			case OpticOptionType.ReticleLum:
				break;
			case OpticOptionType.ReticleType:
				break;
			case OpticOptionType.FlipState:
				break;
			case OpticOptionType.ElevationTweak:
				if (!cycle)
				{
					ElevationStep++;
					Zero();
				}
				break;
			case OpticOptionType.WindageTweak:
				if (!cycle)
				{
					WindageStep++;
					Zero();
				}
				break;
			}
		}

		public void SetCurSettingDown()
		{
			switch (OptionTypes[CurSelectedOptionIndex])
			{
			case OpticOptionType.Zero:
				ZeroDistanceIndex--;
				ZeroDistanceIndex = Mathf.Clamp(ZeroDistanceIndex, 0, ZeroDistances.Count - 1);
				Zero();
				break;
			case OpticOptionType.Magnification:
				m_zoomSettingIndex--;
				m_zoomSettingIndex = Mathf.Clamp(m_zoomSettingIndex, 0, ZoomSettings.Count - 1);
				UpdateScopeCam();
				break;
			case OpticOptionType.ReticleLum:
				break;
			case OpticOptionType.ReticleType:
				break;
			case OpticOptionType.FlipState:
				break;
			case OpticOptionType.ElevationTweak:
				ElevationStep--;
				Zero();
				break;
			case OpticOptionType.WindageTweak:
				WindageStep--;
				Zero();
				break;
			}
		}

		private void GoToNextSetting()
		{
			if (OptionTypes.Count >= 2)
			{
				CurSelectedOptionIndex++;
				if (CurSelectedOptionIndex >= OptionTypes.Count)
				{
					CurSelectedOptionIndex = 0;
				}
			}
		}

		public void GoToSetting(int i)
		{
			CurSelectedOptionIndex = i;
		}

		public override void OnAttach()
		{
			base.OnAttach();
			ScopeCam.MagnificationEnabled = true;
			Zero();
		}

		public override void OnDetach()
		{
			base.OnDetach();
			ScopeCam.MagnificationEnabled = false;
		}

		[ContextMenu("PopulateDefaultZoom")]
		public void PopulateDefaultZoom()
		{
			if (ZoomSettings.Count == 0)
			{
				ZoomSetting zoomSetting = new ZoomSetting();
				zoomSetting.Magnification = ScopeCam.Magnification;
				zoomSetting.AngleBlueStrength = ScopeCam.AngleBlurStrength;
				zoomSetting.CutoffSoftness = ScopeCam.CutoffSoftness;
				zoomSetting.AngularOccludeSensitivity = ScopeCam.AngularOccludeSensitivity;
				ZoomSettings.Add(zoomSetting);
			}
		}

		public void Zero()
		{
			if (!(TargetAimer != null))
			{
				return;
			}
			if (Attachment != null && Attachment.curMount != null && Attachment.curMount.Parent != null && Attachment.curMount.Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.Parent as FVRFireArm;
				FireArmRoundType roundType = fVRFireArm.RoundType;
				float num = ZeroDistances[ZeroDistanceIndex];
				float num2 = 0f;
				if (AM.SRoundDisplayDataDic.ContainsKey(roundType))
				{
					FVRFireArmRoundDisplayData fVRFireArmRoundDisplayData = AM.SRoundDisplayDataDic[roundType];
					num2 = fVRFireArmRoundDisplayData.BulletDropCurve.Evaluate(num * 0.001f);
				}
				Vector3 vector = fVRFireArm.MuzzlePos.position + fVRFireArm.GetMuzzle().forward * num + fVRFireArm.GetMuzzle().up * num2;
				Vector3 vector2 = vector - TargetAimer.transform.position;
				vector2 = Vector3.ProjectOnPlane(vector2, base.transform.right);
				vector2 = Quaternion.AngleAxis(0.004166675f * (float)ElevationStep, base.transform.right) * vector2;
				vector2 = Quaternion.AngleAxis(0.004166675f * (float)WindageStep, base.transform.up) * vector2;
				TargetAimer.rotation = Quaternion.LookRotation(vector2, base.transform.up);
				ScopeCam.PointTowards(vector);
				ScopeCam.ScopeCamera.transform.rotation = Quaternion.LookRotation(vector2, base.transform.up);
			}
			else if (BackupFireArm != null)
			{
				FVRFireArm backupFireArm = BackupFireArm;
				FireArmRoundType roundType2 = backupFireArm.RoundType;
				float num3 = ZeroDistances[ZeroDistanceIndex];
				float num4 = 0f;
				if (AM.SRoundDisplayDataDic.ContainsKey(roundType2))
				{
					FVRFireArmRoundDisplayData fVRFireArmRoundDisplayData2 = AM.SRoundDisplayDataDic[roundType2];
					num4 = fVRFireArmRoundDisplayData2.BulletDropCurve.Evaluate(num3 * 0.001f);
				}
				Vector3 vector3 = backupFireArm.MuzzlePos.position + backupFireArm.GetMuzzle().forward * num3 + backupFireArm.GetMuzzle().up * num4;
				Vector3 vector4 = vector3 - TargetAimer.transform.position;
				vector4 = Vector3.ProjectOnPlane(vector4, base.transform.right);
				vector4 = Quaternion.AngleAxis(0.004166675f * (float)ElevationStep, base.transform.right) * vector4;
				vector4 = Quaternion.AngleAxis(0.004166675f * (float)WindageStep, base.transform.up) * vector4;
				TargetAimer.rotation = Quaternion.LookRotation(vector4, base.transform.up);
				ScopeCam.PointTowards(vector3);
			}
			else if (BackupMuzzle != null)
			{
				Vector3 worldPosition = BackupMuzzle.position + BackupMuzzle.forward * ZeroDistances[ZeroDistanceIndex];
				TargetAimer.LookAt(worldPosition, Vector3.up);
			}
			else
			{
				TargetAimer.localRotation = Quaternion.identity;
			}
		}
	}
}
