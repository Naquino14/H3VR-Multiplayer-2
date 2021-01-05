using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_ReticleContact : MonoBehaviour
	{
		public enum ContactState
		{
			Disabled,
			Icon,
			Arrow
		}

		public enum ContactType
		{
			None = -1,
			Unknown,
			Hold,
			Health,
			Supply,
			Enemy
		}

		public List<Mesh> Meshes_Arrow;

		public List<Mesh> Meshes_Icon;

		public List<Material> Mats_Arrow;

		public List<Material> Mats_Icon;

		public List<Material> Mats_Arrow_Visited;

		public List<Material> Mats_Icon_Visited;

		public Transform Icon;

		public Transform Vertical;

		public MeshFilter M_Arrow;

		public MeshFilter M_Icon;

		public MeshFilter M_Vertical;

		public MeshFilter M_BasePlate;

		public Renderer R_Arrow;

		public Renderer R_Icon;

		public Renderer R_Vertical;

		public Renderer R_BasePlate;

		public ContactType Type = ContactType.None;

		public ContactState State;

		public Transform TrackedTransform;

		private float m_range = 50f;

		public bool UsesVerticality = true;

		public bool ShowArrows = true;

		private bool m_isVisited;

		public void InitContact(ContactType type, Transform trackedTransform, float range)
		{
			TrackedTransform = trackedTransform;
			m_range = range;
			SetContactType(type);
		}

		public void SetVisited(bool b)
		{
			m_isVisited = b;
			if (!m_isVisited)
			{
				R_Arrow.material = Mats_Arrow[(int)Type];
				R_Icon.material = Mats_Icon[(int)Type];
			}
			else
			{
				R_Arrow.material = Mats_Arrow_Visited[(int)Type];
				R_Icon.material = Mats_Icon_Visited[(int)Type];
			}
		}

		public bool Tick(Vector3 cPoint)
		{
			if (TrackedTransform == null)
			{
				return false;
			}
			Vector3 vector = TrackedTransform.position - cPoint;
			Vector3 lpXZ = vector;
			lpXZ.y = 0f;
			if (Type == ContactType.Enemy && lpXZ.magnitude > m_range * 1.1f)
			{
				return false;
			}
			if (lpXZ.magnitude > m_range)
			{
				UpdateContactAsArrow(vector, lpXZ);
			}
			else
			{
				UpdateContactAsIcon(vector, lpXZ);
			}
			return true;
		}

		private void UpdateContactAsArrow(Vector3 lp, Vector3 lpXZ)
		{
			SetContactState(ContactState.Arrow);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.LookRotation(lpXZ, Vector3.up);
		}

		private void UpdateContactAsIcon(Vector3 lp, Vector3 lpXZ)
		{
			SetContactState(ContactState.Icon);
			base.transform.localPosition = lpXZ * (1f / m_range);
			float y = lp.y / m_range;
			if (!UsesVerticality)
			{
				y = 0f;
			}
			Vertical.localScale = new Vector3(1f, lp.y / m_range, 1f);
			Icon.localPosition = new Vector3(0f, y, 0f);
		}

		private void SetContactType(ContactType t)
		{
			if (Type != t)
			{
				Type = t;
				M_Arrow.mesh = Meshes_Arrow[(int)Type];
				M_Icon.mesh = Meshes_Icon[(int)Type];
				R_Arrow.material = Mats_Arrow[(int)Type];
				R_Icon.material = Mats_Icon[(int)Type];
			}
		}

		private void SetContactState(ContactState s)
		{
			if (State != s)
			{
				State = s;
				switch (State)
				{
				case ContactState.Icon:
					R_Arrow.enabled = false;
					R_Icon.enabled = true;
					R_Vertical.enabled = UsesVerticality;
					R_BasePlate.enabled = UsesVerticality;
					break;
				case ContactState.Arrow:
					R_Arrow.enabled = ShowArrows;
					R_Icon.enabled = false;
					R_Vertical.enabled = false;
					R_BasePlate.enabled = false;
					break;
				}
			}
		}
	}
}
