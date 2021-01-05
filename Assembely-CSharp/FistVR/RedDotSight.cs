using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class RedDotSight : FVRFireArmAttachmentInterface
	{
		private int m_zeroDistanceIndex = 3;

		private float[] m_zeroDistances = new float[7]
		{
			2f,
			5f,
			10f,
			15f,
			25f,
			50f,
			100f
		};

		public Transform TargetAimer;

		public Text ZeroingText;

		private float m_staticDistance = 10f;

		public Transform BackupMuzzle;

		public bool UsesBrightnessSettings;

		public int CurrentBrightnessSetting;

		public Renderer BrightnessRend;

		[ColorUsage(true, true, 0f, 30f, 0.125f, 3f)]
		public List<Color> Colors;

		public RedDotSight MigrateFromObj;

		protected override void Awake()
		{
			base.Awake();
			Zero();
		}

		private void CycleBrightness()
		{
			CurrentBrightnessSetting++;
			if (CurrentBrightnessSetting >= Colors.Count)
			{
				CurrentBrightnessSetting = 0;
			}
			UpdateBrightness();
		}

		private void UpdateBrightness()
		{
			if (UsesBrightnessSettings)
			{
				BrightnessRend.material.SetColor("_Color", Colors[CurrentBrightnessSetting]);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f)
			{
				if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
				{
					DecreaseZeroDistance();
					Zero();
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f)
				{
					IncreaseZeroDistance();
					Zero();
				}
				else if (UsesBrightnessSettings && !(Vector2.Angle(touchpadAxes, Vector2.up) <= 45f))
				{
				}
			}
			base.UpdateInteraction(hand);
		}

		private void IncreaseZeroDistance()
		{
			m_zeroDistanceIndex++;
			m_zeroDistanceIndex = Mathf.Clamp(m_zeroDistanceIndex, 0, m_zeroDistances.Length - 1);
		}

		private void DecreaseZeroDistance()
		{
			m_zeroDistanceIndex--;
			m_zeroDistanceIndex = Mathf.Clamp(m_zeroDistanceIndex, 0, m_zeroDistances.Length - 1);
		}

		private void Zero()
		{
			if (Attachment != null && Attachment.curMount != null && Attachment.curMount.Parent != null && Attachment.curMount.Parent is FVRFireArm)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.Parent as FVRFireArm;
				Vector3 worldPosition = fVRFireArm.MuzzlePos.position + fVRFireArm.MuzzlePos.forward * m_zeroDistances[m_zeroDistanceIndex];
				TargetAimer.LookAt(worldPosition, Vector3.up);
			}
			else if (BackupMuzzle != null)
			{
				Vector3 worldPosition2 = BackupMuzzle.position + BackupMuzzle.forward * m_zeroDistances[m_zeroDistanceIndex];
				TargetAimer.LookAt(worldPosition2, Vector3.up);
			}
			else
			{
				TargetAimer.localRotation = Quaternion.identity;
			}
			ZeroingText.text = "Zero Distance: " + m_zeroDistances[m_zeroDistanceIndex] + "m";
		}

		public override void OnAttach()
		{
			base.OnAttach();
			Zero();
		}

		public override void OnDetach()
		{
			base.OnDetach();
			Zero();
		}

		[ContextMenu("MigrateFrom")]
		public void MigrateFrom()
		{
			UsesBrightnessSettings = true;
			Colors = MigrateFromObj.Colors;
			MigrateFromObj = null;
		}
	}
}
