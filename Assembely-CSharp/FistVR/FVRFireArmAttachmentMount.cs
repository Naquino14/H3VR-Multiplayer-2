using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmAttachmentMount : MonoBehaviour
	{
		public FVRPhysicalObject Parent;

		public List<FVRFireArmAttachmentMount> SubMounts;

		public Transform Point_Front;

		public Transform Point_Rear;

		public FVRFireArmAttachementMountType Type;

		public float ScaleModifier = 1f;

		public List<FVRFireArmAttachment> AttachmentsList = new List<FVRFireArmAttachment>();

		private HashSet<FVRFireArmAttachment> AttachmentsHash = new HashSet<FVRFireArmAttachment>();

		public FVRPhysicalObject MyObject;

		public bool HasHoverDisablePiece;

		public GameObject DisableOnHover;

		private int m_maxAttachments = 10;

		public bool ParentToThis;

		private void Awake()
		{
			if (Type == FVRFireArmAttachementMountType.Suppressor || Type.ToString().Contains("Bayonet"))
			{
				m_maxAttachments = 1;
			}
			if (HasHoverDisablePiece && DisableOnHover == null)
			{
				HasHoverDisablePiece = false;
			}
		}

		public bool isMountableOn(FVRFireArmAttachment possibleAttachment)
		{
			if (Parent == null)
			{
				return false;
			}
			if (AttachmentsList.Count >= m_maxAttachments)
			{
				return false;
			}
			if (possibleAttachment.AttachmentInterface != null && possibleAttachment.AttachmentInterface is AttachableBipodInterface && GetRootMount().MyObject.Bipod != null)
			{
				return false;
			}
			if (possibleAttachment is Suppressor)
			{
				if (GetRootMount().MyObject is SingleActionRevolver && !(GetRootMount().MyObject as SingleActionRevolver).AllowsSuppressor)
				{
					return false;
				}
				if (GetRootMount().MyObject is Revolver && !(GetRootMount().MyObject as Revolver).AllowsSuppressor)
				{
					return false;
				}
			}
			if (possibleAttachment is AttachableMeleeWeapon && GetRootMount().MyObject is FVRFireArm)
			{
				FVRFireArm fVRFireArm = GetRootMount().MyObject as FVRFireArm;
				if (fVRFireArm.CurrentAttachableMeleeWeapon != null)
				{
					return false;
				}
			}
			return true;
		}

		public void BeginHover()
		{
			if (HasHoverDisablePiece && DisableOnHover.activeSelf)
			{
				DisableOnHover.SetActive(value: false);
			}
		}

		public void EndHover()
		{
			CheckStatus();
		}

		private void CheckStatus()
		{
			if (HasHoverDisablePiece && AttachmentsList.Count <= 0 && !DisableOnHover.activeSelf)
			{
				DisableOnHover.SetActive(value: true);
			}
		}

		public bool CanThisRescale()
		{
			FVRFireArmAttachment component = MyObject.GetComponent<FVRFireArmAttachment>();
			if (component != null)
			{
				if (!component.CanScaleToMount)
				{
					return false;
				}
				return component.curMount.CanThisRescale();
			}
			return true;
		}

		public FVRFireArmAttachmentMount GetRootMount()
		{
			FVRFireArmAttachment component = MyObject.GetComponent<FVRFireArmAttachment>();
			if (component != null)
			{
				return component.curMount.GetRootMount();
			}
			return this;
		}

		public void RegisterAttachment(FVRFireArmAttachment attachment)
		{
			if (AttachmentsHash.Add(attachment))
			{
				AttachmentsList.Add(attachment);
				if (HasHoverDisablePiece && DisableOnHover.activeSelf)
				{
					DisableOnHover.SetActive(value: false);
				}
			}
		}

		public void DeRegisterAttachment(FVRFireArmAttachment attachment)
		{
			if (AttachmentsHash.Remove(attachment))
			{
				AttachmentsList.Remove(attachment);
			}
		}

		public bool HasAttachmentsOnIt()
		{
			if (AttachmentsList.Count == 0)
			{
				return false;
			}
			return true;
		}
	}
}
