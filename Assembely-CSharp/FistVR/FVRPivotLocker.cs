using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRPivotLocker : FVRPhysicalObject
	{
		[Header("PivotLockerStuff")]
		public Transform TestingBox;

		public Transform SavedPose;

		private FVRPhysicalObject m_obj;

		public List<Transform> AxisTools;

		private string m_axis = "X";

		private float m_axisSensitivity = 1f;

		public override bool IsDistantGrabbable()
		{
			if (m_obj != null)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}

		public override bool IsInteractable()
		{
			if (m_obj != null)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public void ToggleLock()
		{
			if (m_obj != null)
			{
				UnlockObject();
			}
			else
			{
				TryToLockObject();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_obj != null)
			{
				if (!m_obj.RootRigidbody.isKinematic)
				{
					m_obj.RootRigidbody.isKinematic = true;
				}
				if (m_obj.transform.parent != null)
				{
					m_obj.transform.SetParent(null);
				}
			}
		}

		private void UnlockObject()
		{
			m_obj.IsPivotLocked = false;
			m_obj.RootRigidbody.isKinematic = false;
			m_obj = null;
		}

		public void BTN_SetAxis(string a)
		{
			m_axis = a;
		}

		public void BTN_NudgeAxis(float a)
		{
			switch (m_axis)
			{
			case "X":
				SavedPose.localEulerAngles += new Vector3(a, 0f, 0f);
				break;
			case "Y":
				SavedPose.localEulerAngles += new Vector3(0f, a, 0f);
				break;
			case "Z":
				SavedPose.localEulerAngles += new Vector3(0f, 0f, a);
				break;
			}
			UpdatePose();
		}

		public void BTN_Set0()
		{
			switch (m_axis)
			{
			case "X":
				SavedPose.localEulerAngles = new Vector3(0f, SavedPose.localEulerAngles.y, SavedPose.localEulerAngles.z);
				break;
			case "Y":
				SavedPose.localEulerAngles = new Vector3(SavedPose.localEulerAngles.x, 0f, SavedPose.localEulerAngles.z);
				break;
			case "Z":
				SavedPose.localEulerAngles = new Vector3(SavedPose.localEulerAngles.x, SavedPose.localEulerAngles.y, 0f);
				break;
			}
			UpdatePose();
		}

		public void BTN_SetAxisToolsSide(int s)
		{
			switch (s)
			{
			case 0:
			{
				for (int j = 0; j < AxisTools.Count; j++)
				{
					AxisTools[j].gameObject.SetActive(value: true);
					AxisTools[j].localPosition = new Vector3(0f - Mathf.Abs(AxisTools[j].localPosition.x), AxisTools[j].localPosition.y, AxisTools[j].localPosition.z);
				}
				break;
			}
			case 1:
			{
				for (int k = 0; k < AxisTools.Count; k++)
				{
					AxisTools[k].gameObject.SetActive(value: true);
					AxisTools[k].localPosition = new Vector3(Mathf.Abs(AxisTools[k].localPosition.x), AxisTools[k].localPosition.y, AxisTools[k].localPosition.z);
				}
				break;
			}
			case 2:
			{
				for (int i = 0; i < AxisTools.Count; i++)
				{
					AxisTools[i].gameObject.SetActive(value: false);
				}
				break;
			}
			}
		}

		public void BTN_SetAxisSensitivity(float s)
		{
			m_axisSensitivity = s;
		}

		public void SetXYZZero()
		{
			if (!(m_obj == null))
			{
				m_obj.transform.rotation = base.transform.rotation;
				m_obj.RootRigidbody.rotation = base.transform.rotation;
				m_obj.RootRigidbody.isKinematic = true;
				m_obj.PivotLockPos = m_obj.transform.position;
				m_obj.PivotLockRot = m_obj.transform.rotation;
				SavedPose.position = m_obj.transform.position;
				SavedPose.rotation = m_obj.transform.rotation;
				UpdatePose();
			}
		}

		public void SlideOnAxis(Vector3 amount)
		{
			amount *= m_axisSensitivity;
			SavedPose.localPosition += amount;
			UpdatePose();
		}

		public void RotateOnAxis(string Axis, float amount)
		{
			amount *= m_axisSensitivity;
			switch (Axis)
			{
			case "X":
				m_obj.transform.rotation = m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.right);
				break;
			case "Y":
				m_obj.transform.rotation = m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.up);
				break;
			case "Z":
				m_obj.transform.rotation = m_obj.transform.rotation * Quaternion.AngleAxis(amount, Vector3.forward);
				break;
			}
			m_obj.PivotLockPos = m_obj.transform.position;
			m_obj.PivotLockRot = m_obj.transform.rotation;
			SavedPose.position = m_obj.transform.position;
			SavedPose.rotation = m_obj.transform.rotation;
			UpdatePose();
		}

		private void TryToLockObject()
		{
			if (GM.CurrentMovementManager.Hands[0].CurrentInteractable != null && GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject && IsInsideMyBox(GM.CurrentMovementManager.Hands[0].transform.position))
			{
				LockObject(GM.CurrentMovementManager.Hands[0].CurrentInteractable as FVRPhysicalObject);
			}
			if (!(m_obj != null) && GM.CurrentMovementManager.Hands[1].CurrentInteractable != null && GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject && IsInsideMyBox(GM.CurrentMovementManager.Hands[1].transform.position))
			{
				LockObject(GM.CurrentMovementManager.Hands[1].CurrentInteractable as FVRPhysicalObject);
			}
		}

		private void LockObject(FVRPhysicalObject o)
		{
			base.RootRigidbody.isKinematic = true;
			m_obj = o;
			m_obj.IsPivotLocked = true;
			m_obj.PivotLockPos = m_obj.transform.position;
			m_obj.PivotLockRot = m_obj.transform.rotation;
			m_obj.RootRigidbody.isKinematic = true;
			SavedPose.position = m_obj.transform.position;
			SavedPose.rotation = m_obj.transform.rotation;
		}

		private void UpdatePose()
		{
			if (!(m_obj == null))
			{
				m_obj.PivotLockPos = SavedPose.position;
				m_obj.PivotLockRot = SavedPose.rotation;
				m_obj.transform.position = SavedPose.position;
				m_obj.transform.rotation = SavedPose.rotation;
			}
		}

		public bool IsInsideMyBox(Vector3 pos)
		{
			bool result = true;
			Vector3 vector = TestingBox.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}

		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
			result.x *= Mathf.Sign(result.x * (m[2, 1] - m[1, 2]));
			result.y *= Mathf.Sign(result.y * (m[0, 2] - m[2, 0]));
			result.z *= Mathf.Sign(result.z * (m[1, 0] - m[0, 1]));
			return result;
		}

		public static void AlignChild(Transform main, Transform child, Transform alignTo)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(child.position, child.rotation, Vector3.one);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(alignTo.position, alignTo.rotation, Vector3.one);
			Matrix4x4 m = matrix4x2 * matrix4x.inverse * main.localToWorldMatrix;
			main.position = m.GetColumn(3);
			main.rotation = QuaternionFromMatrix(m);
		}
	}
}
