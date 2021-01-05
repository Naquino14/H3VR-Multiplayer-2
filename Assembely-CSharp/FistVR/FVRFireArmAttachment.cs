using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmAttachment : FVRPhysicalObject
	{
		[Header("Attachment Params")]
		public FVRFireArmAttachementMountType Type;

		public FVRFireArmAttachmentMount curMount;

		public FVRFireArmAttachmentSensor Sensor;

		public FVRFireArmAttachmentInterface AttachmentInterface;

		public AudioEvent AudClipAttach;

		public AudioEvent AudClipDettach;

		public bool IsBiDirectional = true;

		public bool CanScaleToMount = true;

		private Collider m_col;

		private bool m_hasCollider;

		protected bool m_isInSnappingMode;

		protected override void Awake()
		{
			base.Awake();
			m_col = GetComponent<Collider>();
			if (m_col != null)
			{
				m_hasCollider = true;
			}
		}

		public void SetTriggerState(bool b)
		{
			if (m_hasCollider)
			{
				m_col.enabled = b;
			}
		}

		public override bool IsInteractable()
		{
			if (curMount != null)
			{
				return false;
			}
			return true;
		}

		public virtual bool CanAttach()
		{
			return true;
		}

		public virtual bool CanDetach()
		{
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (curMount != null)
			{
				DetachFromMount();
			}
			base.BeginInteraction(hand);
		}

		public FVRPhysicalObject GetRootObject()
		{
			if (curMount != null)
			{
				if (curMount.MyObject is FVRFireArmAttachment)
				{
					return (curMount.MyObject as FVRFireArmAttachment).GetRootObject();
				}
				return curMount.MyObject;
			}
			return this;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			SetSnapping(b: false);
			base.EndInteraction(hand);
			if (Sensor.CurHoveredMount != null)
			{
				AttachToMount(Sensor.CurHoveredMount, playSound: true);
			}
		}

		protected override Vector3 GetGrabPos()
		{
			if (Sensor.CurHoveredMount != null)
			{
				return base.transform.position;
			}
			return base.GetGrabPos();
		}

		protected override Quaternion GetGrabRot()
		{
			if (Sensor.CurHoveredMount != null)
			{
				return base.transform.rotation;
			}
			return base.GetGrabRot();
		}

		public void ScaleToMount(FVRFireArmAttachmentMount mount)
		{
			float scaleModifier = mount.GetRootMount().ScaleModifier;
			base.transform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
		}

		protected override Vector3 GetPosTarget()
		{
			if (Sensor.CurHoveredMount != null)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Sensor.CurHoveredMount.Point_Front.position, Sensor.CurHoveredMount.Point_Rear.position, base.m_handPos);
				if (Vector3.Distance(closestValidPoint, base.m_handPos) < 0.15f)
				{
					return closestValidPoint;
				}
				return base.GetPosTarget();
			}
			return base.GetPosTarget();
		}

		protected override Quaternion GetRotTarget()
		{
			if (Sensor.CurHoveredMount != null)
			{
				if (IsBiDirectional)
				{
					if (Vector3.Dot(base.transform.forward, Sensor.CurHoveredMount.transform.forward) >= 0f)
					{
						return Sensor.CurHoveredMount.transform.rotation;
					}
					return Quaternion.LookRotation(-Sensor.CurHoveredMount.transform.forward, Sensor.CurHoveredMount.transform.up);
				}
				return Sensor.CurHoveredMount.transform.rotation;
			}
			return base.GetRotTarget();
		}

		protected virtual void UpdateSnappingBasedOnDistance()
		{
			if (Sensor.CurHoveredMount != null)
			{
				Vector3 zero = Vector3.zero;
				zero = ((Type != FVRFireArmAttachementMountType.Suppressor) ? GetClosestValidPoint(Sensor.CurHoveredMount.Point_Front.position, Sensor.CurHoveredMount.Point_Rear.position, base.transform.position) : GetClosestValidPoint(Sensor.CurHoveredMount.Point_Front.position, (Sensor.CurHoveredMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, base.transform.position));
				if (Vector3.Distance(zero, base.transform.position) < 0.08f)
				{
					SetSnapping(b: true);
				}
				else
				{
					SetSnapping(b: false);
				}
			}
			else
			{
				SetSnapping(b: false);
			}
		}

		protected override void FVRFixedUpdate()
		{
			if (base.IsHeld)
			{
				UpdateSnappingBasedOnDistance();
			}
			base.FVRFixedUpdate();
		}

		protected virtual void SetSnapping(bool b)
		{
			if (m_isInSnappingMode != b)
			{
				m_isInSnappingMode = b;
				if (m_isInSnappingMode)
				{
					SetAllCollidersToLayer(triggersToo: false, "NoCol");
				}
				else
				{
					SetAllCollidersToLayer(triggersToo: false, "Default");
				}
			}
		}

		public virtual void AttachToMount(FVRFireArmAttachmentMount m, bool playSound)
		{
			if (playSound)
			{
			}
			curMount = m;
			StoreAndDestroyRigidbody();
			if (curMount.GetRootMount().ParentToThis)
			{
				SetParentage(curMount.GetRootMount().transform);
			}
			else
			{
				SetParentage(curMount.MyObject.transform);
			}
			if (IsBiDirectional)
			{
				if (Vector3.Dot(base.transform.forward, curMount.transform.forward) >= 0f)
				{
					base.transform.rotation = curMount.transform.rotation;
				}
				else
				{
					base.transform.rotation = Quaternion.LookRotation(-curMount.transform.forward, curMount.transform.up);
				}
			}
			else
			{
				base.transform.rotation = curMount.transform.rotation;
			}
			base.transform.position = GetClosestValidPoint(curMount.Point_Front.position, curMount.Point_Rear.position, base.transform.position);
			if (curMount.Parent != null)
			{
				curMount.Parent.RegisterAttachment(this);
			}
			curMount.RegisterAttachment(this);
			if (curMount.Parent != null && curMount.Parent.QuickbeltSlot != null)
			{
				SetAllCollidersToLayer(triggersToo: false, "NoCol");
			}
			else
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			if (AttachmentInterface != null)
			{
				AttachmentInterface.OnAttach();
				AttachmentInterface.gameObject.SetActive(value: true);
			}
			SetTriggerState(b: false);
		}

		public void DetachFromMount()
		{
			if (AttachmentInterface != null)
			{
				AttachmentInterface.OnDetach();
				AttachmentInterface.gameObject.SetActive(value: false);
			}
			SetTriggerState(b: true);
			SetParentage(null);
			curMount.DeRegisterAttachment(this);
			if (curMount.Parent != null)
			{
				curMount.Parent.DeRegisterAttachment(this);
			}
			curMount = null;
			RecoverRigidbody();
		}

		public override void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			int num = 0;
			if (AttachmentInterface != null && AttachmentInterface is Amplifier)
			{
				empty = "m_zoomSettingIndex";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					num = Convert.ToInt32(empty2);
					(AttachmentInterface as Amplifier).m_zoomSettingIndex = num;
				}
				empty = "ZeroDistanceIndex";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					num = Convert.ToInt32(empty2);
					(AttachmentInterface as Amplifier).ZeroDistanceIndex = num;
				}
				empty = "ElevationStep";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					num = Convert.ToInt32(empty2);
					(AttachmentInterface as Amplifier).ElevationStep = num;
				}
				empty = "WindageStep";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					num = Convert.ToInt32(empty2);
					(AttachmentInterface as Amplifier).WindageStep = num;
				}
			}
		}

		public override Dictionary<string, string> GetFlagDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (AttachmentInterface != null && AttachmentInterface is Amplifier)
			{
				dictionary.Add("m_zoomSettingIndex", (AttachmentInterface as Amplifier).m_zoomSettingIndex.ToString());
				dictionary.Add("ZeroDistanceIndex", (AttachmentInterface as Amplifier).ZeroDistanceIndex.ToString());
				dictionary.Add("ElevationStep", (AttachmentInterface as Amplifier).ElevationStep.ToString());
				dictionary.Add("WindageStep", (AttachmentInterface as Amplifier).WindageStep.ToString());
			}
			return dictionary;
		}
	}
}
