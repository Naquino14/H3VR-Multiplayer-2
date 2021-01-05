using UnityEngine;

namespace FistVR
{
	public class FVRFireArmAttachmentInterface : FVRInteractiveObject
	{
		public FVRFireArmAttachment Attachment;

		public bool IsLocked;

		public bool ForceInteractable;

		public FVRFireArmAttachmentMount[] SubMounts;

		private Collider m_col;

		private bool m_hasCollider;

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
			if (ForceInteractable)
			{
				return true;
			}
			if (Attachment != null)
			{
				if (Attachment.IsPickUpLocked)
				{
					return false;
				}
				return base.IsInteractable();
			}
			return false;
		}

		public override bool IsDistantGrabbable()
		{
			if (Attachment != null)
			{
				if (Attachment.IsPickUpLocked)
				{
					return false;
				}
				return base.IsInteractable();
			}
			return false;
		}

		public virtual void OnAttach()
		{
			Attachment.ClearQuickbeltState();
			SM.PlayCoreSoundOverrides(FVRPooledAudioType.GenericClose, Attachment.AudClipAttach, base.transform.position, new Vector2(0.2f, 0.2f), new Vector2(1f, 1.1f));
			for (int i = 0; i < SubMounts.Length; i++)
			{
				SubMounts[i].Parent = Attachment.curMount.Parent;
				if (!Attachment.curMount.SubMounts.Contains(SubMounts[i]))
				{
					Attachment.curMount.SubMounts.Add(SubMounts[i]);
					for (int j = 0; j < SubMounts[i].AttachmentsList.Count; j++)
					{
						Attachment.curMount.Parent.RegisterAttachment(SubMounts[i].AttachmentsList[j]);
					}
				}
			}
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm && Attachment is MuzzleDevice)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.RegisterMuzzleDevice(Attachment as MuzzleDevice);
			}
			Attachment.Sensor.SetTriggerState(b: false);
		}

		public virtual void OnDetach()
		{
			if (Attachment.curMount.GetRootMount().Parent is FVRFireArm && Attachment is MuzzleDevice)
			{
				FVRFireArm fVRFireArm = Attachment.curMount.GetRootMount().Parent as FVRFireArm;
				fVRFireArm.DeRegisterMuzzleDevice(Attachment as MuzzleDevice);
			}
			SM.PlayCoreSoundOverrides(FVRPooledAudioType.GenericClose, Attachment.AudClipDettach, base.transform.position, new Vector2(0.2f, 0.2f), new Vector2(1f, 1.1f));
			if (Attachment.curMount != null)
			{
				for (int i = 0; i < SubMounts.Length; i++)
				{
					if (!(SubMounts[i] != null))
					{
						continue;
					}
					SubMounts[i].Parent = null;
					if (!Attachment.curMount.SubMounts.Contains(SubMounts[i]))
					{
						continue;
					}
					Attachment.curMount.SubMounts.Remove(SubMounts[i]);
					for (int j = 0; j < SubMounts[i].AttachmentsList.Count; j++)
					{
						if (SubMounts[i].AttachmentsList[j] != null)
						{
							Attachment.curMount.Parent.DeRegisterAttachment(SubMounts[i].AttachmentsList[j]);
						}
					}
				}
			}
			Attachment.Sensor.SetTriggerState(b: true);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.AXButtonDown)
				{
					flag = true;
				}
			}
			else if (hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.25f && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) <= 45f)
			{
				flag = true;
			}
			if (flag && !IsLocked && Attachment != null && Attachment.curMount != null && !HasAttachmentsOnIt() && Attachment.CanDetach())
			{
				DetachRoutine(hand);
			}
		}

		public void DetachRoutine(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			hand.ForceSetInteractable(Attachment);
			Attachment.BeginInteraction(hand);
		}

		public bool HasAttachmentsOnIt()
		{
			if (SubMounts.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < SubMounts.Length; i++)
			{
				if (SubMounts[i].HasAttachmentsOnIt())
				{
					return true;
				}
			}
			return false;
		}
	}
}
