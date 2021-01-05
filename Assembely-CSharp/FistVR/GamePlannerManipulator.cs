using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GamePlannerManipulator : MonoBehaviour
	{
		public GamePlannerPanel Panel;

		public Transform ControlledObject;

		public LayerMask LM_CastMask;

		private RaycastHit m_hit;

		public List<GameObject> ManipModeButtonGroups = new List<GameObject>();

		public void SetControlledObject(Transform t)
		{
			ControlledObject = t;
		}

		public void AxisPress(string s)
		{
			switch (s)
			{
			case "Nudge_XPlus":
				MoveControlledObject(Vector3.right);
				break;
			case "Nudge_XMinus":
				MoveControlledObject(Vector3.left);
				break;
			case "Nudge_YPlus":
				MoveControlledObject(Vector3.up);
				break;
			case "Nudge_YMinus":
				MoveControlledObject(Vector3.down);
				break;
			case "Nudge_ZPlus":
				MoveControlledObject(Vector3.forward);
				break;
			case "Nudge_ZMinus":
				MoveControlledObject(Vector3.back);
				break;
			case "Cast_XPlus":
				ShuntControlledObject(Vector3.right);
				break;
			case "Cast_XMinus":
				ShuntControlledObject(Vector3.left);
				break;
			case "Cast_YPlus":
				ShuntControlledObject(Vector3.up);
				break;
			case "Cast_YMinus":
				ShuntControlledObject(Vector3.down);
				break;
			case "Cast_ZPlus":
				ShuntControlledObject(Vector3.forward);
				break;
			case "Cast_ZMinus":
				ShuntControlledObject(Vector3.back);
				break;
			case "Rotate_YPlus":
				RotateControlledObject(Vector3.up, isWorld: true);
				break;
			case "Rotate_YMinus":
				RotateControlledObject(Vector3.down, isWorld: true);
				break;
			}
		}

		private void MoveControlledObject(Vector3 dir)
		{
			if (Panel.ManipAxis_Nudge != 0)
			{
				dir = ControlledObject.right * dir.x + ControlledObject.up * dir.y + ControlledObject.forward * dir.z;
			}
			Vector3 vector = dir * Panel.GetManipNudgeInterval();
			ControlledObject.position += vector;
			base.transform.position = ControlledObject.position;
		}

		private void RotateControlledObject(Vector3 dir, bool isWorld)
		{
			ControlledObject.Rotate(dir * Panel.GetManipRotateInterval());
		}

		public void ResetRotation()
		{
			if (ControlledObject != null)
			{
				ControlledObject.rotation = Quaternion.identity;
			}
		}

		private void ShuntControlledObject(Vector3 dir)
		{
			Bounds bounds = default(Bounds);
			Collider component = ControlledObject.GetComponent<Collider>();
			bool flag = false;
			if (component != null)
			{
				bounds = new Bounds(component.bounds.center, component.bounds.size);
				Debug.Log("base" + bounds);
				flag = true;
			}
			IEnumerator enumerator = ControlledObject.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					Collider component2 = transform.GetComponent<Collider>();
					if (flag)
					{
						if (component2 != null)
						{
							bounds.Encapsulate(component2.bounds);
							Debug.Log(bounds);
						}
					}
					else
					{
						bounds = new Bounds(component.bounds.center, component.bounds.size);
						Debug.Log("base" + bounds);
						flag = true;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			float manipShuntInterval = Panel.GetManipShuntInterval();
			if (Panel.ManipAxis_Nudge != 0)
			{
				dir = ControlledObject.right * dir.x + ControlledObject.up * dir.y + ControlledObject.forward * dir.z;
			}
			Vector3 vector = manipShuntInterval * dir;
			if (Physics.BoxCast(bounds.center, bounds.extents * 0.5f, dir, out m_hit, Quaternion.identity, manipShuntInterval, LM_CastMask, QueryTriggerInteraction.Ignore))
			{
				vector = m_hit.distance * dir;
				ControlledObject.position += vector;
				base.transform.position = ControlledObject.position;
			}
			else
			{
				ControlledObject.position += vector;
				base.transform.position = ControlledObject.position;
			}
		}

		private void Start()
		{
		}

		public void UpdateManipulatorFrame(GamePlannerPanel.ManipulatorMode mode)
		{
			for (int i = 0; i < ManipModeButtonGroups.Count; i++)
			{
				if (mode == (GamePlannerPanel.ManipulatorMode)i)
				{
					ManipModeButtonGroups[i].SetActive(value: true);
				}
				else
				{
					ManipModeButtonGroups[i].SetActive(value: false);
				}
			}
			switch (Panel.ManipMode)
			{
			case GamePlannerPanel.ManipulatorMode.Nudge:
				if (Panel.ManipAxis_Nudge == GamePlannerPanel.ManipulatorAxis.World)
				{
					base.transform.rotation = Quaternion.identity;
				}
				else
				{
					base.transform.rotation = ControlledObject.rotation;
				}
				break;
			case GamePlannerPanel.ManipulatorMode.Shunt:
				if (Panel.ManipAxis_Shunt == GamePlannerPanel.ManipulatorAxis.World)
				{
					base.transform.rotation = Quaternion.identity;
				}
				else
				{
					base.transform.rotation = ControlledObject.rotation;
				}
				break;
			case GamePlannerPanel.ManipulatorMode.Rotate:
				if (Panel.ManipAxis_Rotate == GamePlannerPanel.ManipulatorAxis.World)
				{
					base.transform.rotation = Quaternion.identity;
				}
				else
				{
					base.transform.rotation = ControlledObject.rotation;
				}
				break;
			}
		}

		private void Update()
		{
			if (!(ControlledObject != null))
			{
			}
		}
	}
}
