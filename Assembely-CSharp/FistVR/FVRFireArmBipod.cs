using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmBipod : MonoBehaviour
	{
		public enum BipodStyle
		{
			Independent,
			Galil,
			Vepr,
			RPK,
			Stoner63
		}

		[Header("Bipod Params")]
		public FVRPhysicalObject FireArm;

		public BipodStyle Style;

		public Transform[] BipodLegs;

		public float FoldedXRot;

		public float UnFoldedXRot = 90f;

		public List<Vector3> FoldedRPK;

		public List<Vector3> UnFoldedRPK;

		public List<Vector3> FoldedPosStroner;

		public List<Vector3> UnFoldedPosStroner;

		public Transform OverrideTranslationFrame;

		public Transform[] BipodFeet;

		public Transform GroundFollower;

		public Transform GroundContactReference;

		public Transform PointToOverride;

		private bool m_isBipodExpanded;

		private bool m_isBipodActive;

		public LayerMask LM_BipodTouch;

		private RaycastHit m_hit;

		private Vector3 m_savedGroundPoint;

		public Vector3 GroundFollowerTargetPoint = Vector3.zero;

		public float RecoilDamping = 0.4f;

		public float RecoilFactor;

		[Header("GalilStyleParams")]
		public Transform BaseClip;

		public float[] ExpandedLegYAngle;

		[Header("ExtendyBits")]
		public bool UsesExtendyBits;

		public Transform Bit1;

		public Transform Bit2;

		public Vector3 LocalPosContracted1;

		public Vector3 LocalPosContracted2;

		public Vector3 LocalPosExpanded1;

		public Vector3 LocalPosExpanded2;

		public AudioEvent AudEvent_BOpen;

		public AudioEvent AudEvent_BClose;

		[Header("MultiLength")]
		public bool UsesMultiLength;

		private int m_mlIndex;

		public Transform MLBit1;

		public Transform MLBit2;

		public List<Vector3> MLDistances = new List<Vector3>();

		public List<float> MLHeights = new List<float>();

		public bool IsBipodActive => m_isBipodActive;

		protected void Awake()
		{
			if (FireArm != null)
			{
				FireArm.Bipod = this;
			}
			Contract(playSound: false);
		}

		public void NextML()
		{
			if (UsesMultiLength)
			{
				m_mlIndex++;
				if (m_mlIndex >= MLDistances.Count)
				{
					m_mlIndex = 0;
				}
				UpdateML();
			}
		}

		public void PrevML()
		{
			if (UsesMultiLength)
			{
				if (m_mlIndex < 0)
				{
					m_mlIndex = MLDistances.Count - 1;
				}
				UpdateML();
			}
		}

		private void UpdateML()
		{
			MLBit1.localPosition = MLDistances[m_mlIndex];
			MLBit2.localPosition = MLDistances[m_mlIndex];
			GroundContactReference.localPosition = new Vector3(0f, MLHeights[m_mlIndex], 0f);
		}

		private Vector3 GetUp()
		{
			if (OverrideTranslationFrame != null)
			{
				return OverrideTranslationFrame.up;
			}
			return base.transform.up;
		}

		private Vector3 GetForward()
		{
			if (OverrideTranslationFrame != null)
			{
				return OverrideTranslationFrame.forward;
			}
			return base.transform.forward;
		}

		private Vector3 GetRight()
		{
			if (OverrideTranslationFrame != null)
			{
				return OverrideTranslationFrame.right;
			}
			return base.transform.right;
		}

		public void Toggle()
		{
			if (m_isBipodExpanded)
			{
				Contract(playSound: true);
			}
			else
			{
				Expand();
			}
		}

		public Transform GetPointTo()
		{
			if (PointToOverride != null)
			{
				return PointToOverride;
			}
			return GroundFollower;
		}

		public void Expand()
		{
			if (FireArm == null)
			{
				return;
			}
			FireArm.ClearQuickbeltState();
			m_isBipodExpanded = true;
			if (FireArm is FVRFireArm)
			{
				(FireArm as FVRFireArm).PlayAudioAsHandling(AudEvent_BOpen, base.transform.position);
			}
			switch (Style)
			{
			case BipodStyle.Independent:
			{
				for (int j = 0; j < BipodFeet.Length; j++)
				{
					BipodLegs[j].transform.localEulerAngles = new Vector3(UnFoldedXRot, 0f, 0f);
					BipodFeet[j].gameObject.SetActive(value: true);
				}
				break;
			}
			case BipodStyle.Galil:
			{
				BaseClip.transform.localEulerAngles = new Vector3(UnFoldedXRot, 0f, 0f);
				for (int l = 0; l < BipodFeet.Length; l++)
				{
					BipodFeet[l].gameObject.SetActive(value: true);
				}
				for (int m = 0; m < BipodLegs.Length; m++)
				{
					BipodLegs[m].transform.localEulerAngles = new Vector3(0f, ExpandedLegYAngle[m], 0f);
				}
				break;
			}
			case BipodStyle.Vepr:
			{
				for (int n = 0; n < BipodFeet.Length; n++)
				{
					BipodLegs[n].transform.localEulerAngles = new Vector3(UnFoldedXRot, 0f, 0f);
					BipodFeet[n].gameObject.SetActive(value: true);
				}
				break;
			}
			case BipodStyle.RPK:
			{
				for (int k = 0; k < BipodFeet.Length; k++)
				{
					BipodLegs[k].transform.localEulerAngles = UnFoldedRPK[k];
					BipodFeet[k].gameObject.SetActive(value: true);
				}
				break;
			}
			case BipodStyle.Stoner63:
			{
				for (int i = 0; i < BipodFeet.Length; i++)
				{
					BipodLegs[i].transform.localEulerAngles = UnFoldedRPK[i];
					BipodLegs[i].transform.localPosition = UnFoldedPosStroner[i];
					BipodFeet[i].gameObject.SetActive(value: true);
				}
				break;
			}
			}
			if (UsesExtendyBits)
			{
				Bit1.localPosition = LocalPosExpanded1;
				Bit2.localPosition = LocalPosExpanded2;
			}
		}

		public void Contract(bool playSound)
		{
			if (m_isBipodActive)
			{
				Deactivate();
			}
			m_isBipodExpanded = false;
			if (playSound && FireArm != null && FireArm is FVRFireArm)
			{
				(FireArm as FVRFireArm).PlayAudioAsHandling(AudEvent_BClose, base.transform.position);
			}
			switch (Style)
			{
			case BipodStyle.Independent:
			{
				for (int j = 0; j < BipodFeet.Length; j++)
				{
					BipodFeet[j].gameObject.SetActive(value: false);
					BipodLegs[j].transform.localEulerAngles = new Vector3(FoldedXRot, 0f, 0f);
				}
				break;
			}
			case BipodStyle.Galil:
			{
				BaseClip.transform.localEulerAngles = new Vector3(FoldedXRot, 0f, 0f);
				for (int l = 0; l < BipodFeet.Length; l++)
				{
					BipodFeet[l].gameObject.SetActive(value: false);
				}
				for (int m = 0; m < BipodLegs.Length; m++)
				{
					BipodLegs[m].transform.localEulerAngles = Vector3.zero;
				}
				break;
			}
			case BipodStyle.Vepr:
			{
				for (int n = 0; n < BipodFeet.Length; n++)
				{
					BipodFeet[n].gameObject.SetActive(value: false);
					BipodLegs[n].transform.localEulerAngles = new Vector3(FoldedXRot, 0f, 0f);
				}
				break;
			}
			case BipodStyle.RPK:
			{
				for (int k = 0; k < BipodFeet.Length; k++)
				{
					BipodLegs[k].transform.localEulerAngles = FoldedRPK[k];
					BipodFeet[k].gameObject.SetActive(value: true);
				}
				break;
			}
			case BipodStyle.Stoner63:
			{
				for (int i = 0; i < BipodFeet.Length; i++)
				{
					BipodLegs[i].transform.localEulerAngles = FoldedRPK[i];
					BipodLegs[i].transform.localPosition = FoldedPosStroner[i];
					BipodFeet[i].gameObject.SetActive(value: true);
				}
				break;
			}
			}
			if (UsesExtendyBits)
			{
				Bit1.localPosition = LocalPosContracted1;
				Bit2.localPosition = LocalPosContracted2;
			}
		}

		public void Activate()
		{
			m_isBipodActive = true;
			if (FireArm != null)
			{
				FireArm.BipodActivated();
			}
		}

		public void Deactivate()
		{
			m_isBipodActive = false;
			if (FireArm != null)
			{
				FireArm.BipodDeactivated();
			}
		}

		public void UpdateBipod()
		{
			if (!m_isBipodExpanded)
			{
				return;
			}
			GroundFollower.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(FireArm.transform.forward, Vector3.up), Vector3.up);
			switch (Style)
			{
			case BipodStyle.Independent:
			{
				for (int i = 0; i < BipodLegs.Length; i++)
				{
					Vector3 forward = Vector3.ProjectOnPlane(-Vector3.up, BipodLegs[i].transform.right);
					if (Vector3.Dot(forward.normalized, -GetUp()) > 0f)
					{
						BipodLegs[i].rotation = Quaternion.LookRotation(forward, FireArm.transform.forward);
					}
				}
				break;
			}
			case BipodStyle.Galil:
			{
				Vector3 forward = Vector3.ProjectOnPlane(-Vector3.up, BaseClip.transform.right);
				if (Vector3.Dot(forward.normalized, -GetUp()) > 0f)
				{
					BaseClip.rotation = Quaternion.LookRotation(forward, FireArm.transform.forward);
				}
				break;
			}
			}
			if (!m_isBipodActive && FireArm.AltGrip == null)
			{
				float f = Vector3.Dot(-GetUp(), Vector3.up);
				float num = Vector3.Dot(GetRight(), Vector3.up);
				if (!(Mathf.Abs(f) > 0.1f) || num > 0.2f)
				{
				}
				bool flag = false;
				float num2 = 0f;
				for (int j = 0; j < BipodFeet.Length; j++)
				{
					Vector3 vector = BipodFeet[j].transform.position - BipodFeet[j].transform.parent.position;
					float magnitude = vector.magnitude;
					Vector3 normalized = vector.normalized;
					if (!Physics.Raycast(BipodFeet[j].transform.parent.position, normalized, out m_hit, magnitude + 0.05f, LM_BipodTouch, QueryTriggerInteraction.Ignore) || (!(m_hit.collider.attachedRigidbody == null) && (!(m_hit.collider.attachedRigidbody != null) || !m_hit.collider.attachedRigidbody.isKinematic)))
					{
						continue;
					}
					Vector3 point = m_hit.point;
					if (flag)
					{
						if (point.y < num2)
						{
							num2 = point.y;
						}
					}
					else
					{
						num2 = point.y;
						flag = true;
					}
				}
				if (flag)
				{
					m_savedGroundPoint = new Vector3(GroundFollower.position.x, num2, GroundFollower.position.z);
					Activate();
				}
			}
			if (m_isBipodActive)
			{
				float f2 = Vector3.Distance(GroundFollower.position, GroundContactReference.position);
				GroundFollowerTargetPoint = m_savedGroundPoint + Vector3.up * Mathf.Abs(f2) - GroundFollower.forward * RecoilFactor * RecoilDamping;
			}
			if (!m_isBipodActive)
			{
				return;
			}
			bool flag2 = false;
			float num3 = 0f;
			for (int k = 0; k < BipodFeet.Length; k++)
			{
				Vector3 vector2 = BipodFeet[k].transform.position - BipodFeet[k].transform.parent.position;
				float magnitude2 = vector2.magnitude;
				Vector3 normalized2 = vector2.normalized;
				if (!Physics.Raycast(BipodFeet[k].transform.parent.position, normalized2, out m_hit, magnitude2 + 0.15f, LM_BipodTouch, QueryTriggerInteraction.Ignore) || !(Vector3.Angle(m_hit.normal, Vector3.up) < 75f))
				{
					continue;
				}
				Vector3 point2 = m_hit.point;
				if (flag2)
				{
					if (point2.y < num3)
					{
						num3 = point2.y;
					}
				}
				else
				{
					num3 = point2.y;
					flag2 = true;
				}
			}
			if (flag2)
			{
				float num4 = Vector3.Distance(new Vector3(m_savedGroundPoint.x, 0f, m_savedGroundPoint.z), new Vector3(GroundFollower.position.x, 0f, GroundFollower.position.z));
				if (num4 > 0.05f)
				{
					m_savedGroundPoint = new Vector3(GroundContactReference.position.x, num3, GroundContactReference.position.z);
				}
				else
				{
					m_savedGroundPoint.y = num3;
				}
			}
			else
			{
				Deactivate();
			}
			if (Vector3.Angle(GetRight(), Vector3.up) < 60f || Vector3.Angle(GetRight(), Vector3.up) > 120f)
			{
				Deactivate();
			}
		}

		public Vector3 GetBipodRootWorld()
		{
			return base.transform.position;
		}

		public Vector3 GetOffsetSavedWorldPoint()
		{
			float f = Vector3.Distance(GroundFollower.position, GroundContactReference.position);
			return m_savedGroundPoint + Vector3.up * Mathf.Abs(f);
		}

		[ContextMenu("Config")]
		public void Config()
		{
			FireArm = base.transform.root.GetComponent<FVRPhysicalObject>();
		}
	}
}
