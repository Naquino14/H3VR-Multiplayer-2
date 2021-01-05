using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class LumberjackMailbox : MonoBehaviour
	{
		public AudioEvent AudEvent_Success;

		public Transform Door;

		public LayerMask LM_IngredientDetect;

		public Transform IngredientDetectCenter;

		public Vector3 IngredientDetectExtends;

		public FVRObject n1;

		public FVRObject n2;

		public FVRObject n3;

		public FVRObject n4;

		public Transform n4pos;

		private bool m_wasDoorShut = true;

		private bool m_hasSpawned;

		private bool IsDoorShut()
		{
			if (Vector3.Angle(Door.forward, base.transform.up) < 2f)
			{
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (!m_hasSpawned)
			{
				bool flag = IsDoorShut();
				if (flag && !m_wasDoorShut)
				{
					Check();
				}
				m_wasDoorShut = flag;
			}
		}

		private void Check()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Collider[] array = Physics.OverlapBox(IngredientDetectCenter.position, IngredientDetectExtends, IngredientDetectCenter.rotation, LM_IngredientDetect, QueryTriggerInteraction.Collide);
			List<Rigidbody> list = new List<Rigidbody>();
			List<FVRPhysicalObject> list2 = new List<FVRPhysicalObject>();
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody attachedRigidbody = array[i].attachedRigidbody;
				if (attachedRigidbody != null && !list.Contains(attachedRigidbody))
				{
					list.Add(attachedRigidbody);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				FVRPhysicalObject component = list[num].gameObject.GetComponent<FVRPhysicalObject>();
				if (component != null && component.ObjectWrapper != null)
				{
					if (component.ObjectWrapper.ItemID == n1.ItemID)
					{
						flag = true;
						list2.Add(component);
					}
					else if (component.ObjectWrapper.ItemID == n2.ItemID)
					{
						flag2 = true;
						list2.Add(component);
					}
					else if (component.ObjectWrapper.ItemID == n3.ItemID)
					{
						flag3 = true;
						list2.Add(component);
					}
				}
			}
			if (flag && flag2 && flag3)
			{
				for (int num2 = list2.Count - 1; num2 >= 0; num2--)
				{
					Object.Destroy(list2[num2].gameObject);
				}
				SM.PlayGenericSound(AudEvent_Success, base.transform.position);
				GameObject gameObject = Object.Instantiate(n4.GetGameObject(), n4pos.position, n4pos.rotation);
				m_hasSpawned = true;
			}
		}
	}
}
