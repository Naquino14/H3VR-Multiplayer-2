using UnityEngine;

namespace FistVR
{
	public class FVRQuickBeltSlot : MonoBehaviour
	{
		public enum QuickbeltSlotShape
		{
			Sphere,
			Rectalinear
		}

		public enum QuickbeltSlotType
		{
			Standard,
			Backpack,
			None
		}

		public Transform QuickbeltRoot;

		public Transform PoseOverride;

		public FVRPhysicalObject.FVRPhysicalObjectSize SizeLimit;

		public QuickbeltSlotShape Shape;

		public QuickbeltSlotType Type;

		public GameObject HoverGeo;

		private Renderer m_hoverGeoRend;

		public Transform RectBounds;

		public FVRPhysicalObject CurObject;

		public bool IsSelectable = true;

		public bool IsPlayer = true;

		public bool UseStraightAxisAlignment;

		private bool m_isKeepingTrackWithHead;

		private bool m_isHovered;

		[HideInInspector]
		public FVRInteractiveObject HeldObject;

		public bool IsKeepingTrackWithHead
		{
			get
			{
				return m_isKeepingTrackWithHead;
			}
			set
			{
				m_isKeepingTrackWithHead = value;
				if (!m_isKeepingTrackWithHead)
				{
					base.transform.localEulerAngles = Vector3.zero;
				}
			}
		}

		public bool IsHovered
		{
			get
			{
				return m_isHovered;
			}
			set
			{
				bool isHovered = m_isHovered;
				if (value)
				{
					if (!HoverGeo.activeSelf)
					{
						HoverGeo.SetActive(value: true);
					}
				}
				else if (HoverGeo.activeSelf)
				{
					HoverGeo.SetActive(value: false);
				}
				m_isHovered = value;
				if ((!m_isHovered || isHovered) && m_isHovered && !isHovered)
				{
				}
			}
		}

		private void Awake()
		{
			HoverGeo.SetActive(value: false);
			m_hoverGeoRend = HoverGeo.GetComponent<Renderer>();
		}

		public bool IsPointInsideMe(Vector3 v)
		{
			if (!IsSelectable)
			{
				return false;
			}
			return Shape switch
			{
				QuickbeltSlotShape.Sphere => IsPointInsideSphereGeo(v), 
				QuickbeltSlotShape.Rectalinear => IsPointInsideRectBound(v), 
				_ => false, 
			};
		}

		private bool IsPointInsideSphereGeo(Vector3 p)
		{
			if (HoverGeo.transform.InverseTransformPoint(p).magnitude < 0.5f)
			{
				return true;
			}
			return false;
		}

		private bool IsPointInsideRectBound(Vector3 p)
		{
			if (RectBounds == null)
			{
				return false;
			}
			Vector3 vector = RectBounds.InverseTransformPoint(p);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				return false;
			}
			return true;
		}

		private void Update()
		{
			if (!GM.CurrentSceneSettings.IsSpawnLockingEnabled && HeldObject != null && (HeldObject as FVRPhysicalObject).m_isSpawnLock)
			{
				(HeldObject as FVRPhysicalObject).m_isSpawnLock = false;
			}
			if (HeldObject != null)
			{
				if ((HeldObject as FVRPhysicalObject).m_isSpawnLock)
				{
					if (!HoverGeo.activeSelf)
					{
						HoverGeo.SetActive(value: true);
					}
					m_hoverGeoRend.material.SetColor("_RimColor", new Color(0.3f, 0.3f, 1f, 1f));
				}
				else if ((HeldObject as FVRPhysicalObject).m_isHardnessed)
				{
					if (!HoverGeo.activeSelf)
					{
						HoverGeo.SetActive(value: true);
					}
					m_hoverGeoRend.material.SetColor("_RimColor", new Color(0.3f, 1f, 0.3f, 1f));
				}
				else
				{
					if (HoverGeo.activeSelf != IsHovered)
					{
						HoverGeo.SetActive(IsHovered);
					}
					m_hoverGeoRend.material.SetColor("_RimColor", new Color(1f, 1f, 1f, 1f));
				}
			}
			else
			{
				if (HoverGeo.activeSelf != IsHovered)
				{
					HoverGeo.SetActive(IsHovered);
				}
				m_hoverGeoRend.material.SetColor("_RimColor", new Color(1f, 1f, 1f, 1f));
			}
		}

		private void FixedUpdate()
		{
			if (IsPlayer && CurObject != null && CurObject.DoesQuickbeltSlotFollowHead && Shape == QuickbeltSlotShape.Sphere)
			{
				if (CurObject.DoesQuickbeltSlotFollowHead)
				{
					Vector3 forward = Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.transform.forward, base.transform.right);
					PoseOverride.rotation = Quaternion.LookRotation(forward, GM.CurrentPlayerBody.Head.transform.up);
				}
				else
				{
					PoseOverride.localRotation = Quaternion.identity;
				}
			}
		}

		public void MoveContents(Vector3 dir)
		{
			if (CurObject != null && !CurObject.IsHeld)
			{
				CurObject.transform.position = CurObject.transform.position + dir;
				CurObject.RootRigidbody.velocity = Vector3.zero;
			}
		}

		public void MoveContentsInstant(Vector3 dir)
		{
			if (CurObject != null && !CurObject.IsHeld)
			{
				CurObject.transform.position = CurObject.transform.position + dir;
				CurObject.RootRigidbody.velocity = Vector3.zero;
			}
		}

		public void MoveContentsCheap(Vector3 dir)
		{
			if (CurObject != null && !CurObject.IsHeld)
			{
				CurObject.RootRigidbody.position = CurObject.RootRigidbody.position + dir;
				CurObject.RootRigidbody.velocity = Vector3.zero;
			}
		}
	}
}
