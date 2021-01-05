using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PieOMizer : MonoBehaviour
	{
		public Transform Door;

		public LayerMask LM_IngredientDetect;

		public List<FVRObject> HerbIngredients;

		public Transform IngredientDetectCenter;

		public Vector3 IngredientDetectExtends;

		public List<FVRObject> Pie_Output;

		public Transform Pie_Output_Point;

		public AudioEvent AudEvent_BakeSuccess;

		public AudioEvent AudEvent_BakeFailure;

		private bool IsDoorShut()
		{
			if (Vector3.Angle(Door.forward, base.transform.up) < 5f)
			{
				return true;
			}
			return false;
		}

		public void Bake(int v)
		{
			if (!IsDoorShut())
			{
				SM.PlayGenericSound(AudEvent_BakeFailure, base.transform.position);
				return;
			}
			bool flag = false;
			Collider[] array = Physics.OverlapBox(IngredientDetectCenter.position, IngredientDetectExtends, IngredientDetectCenter.rotation, LM_IngredientDetect, QueryTriggerInteraction.Collide);
			List<RotrwMeatCore> list = new List<RotrwMeatCore>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<RotrwMeatCore>() != null)
				{
					list.Add(array[i].GetComponent<RotrwMeatCore>());
					Debug.Log("Detected: " + array[i].gameObject.name);
				}
			}
			int index = 0;
			if (list.Count == 2)
			{
				if (list[0].Type == RotrwMeatCore.CoreType.Tasty)
				{
					switch (list[1].Type)
					{
					case RotrwMeatCore.CoreType.Tasty:
						index = 0;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Shiny:
						index = 1;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Moldy:
						index = 2;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Zippy:
						index = 3;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Weighty:
						index = 4;
						flag = true;
						break;
					}
				}
				else if (list[1].Type == RotrwMeatCore.CoreType.Tasty)
				{
					switch (list[0].Type)
					{
					case RotrwMeatCore.CoreType.Tasty:
						index = 0;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Shiny:
						index = 1;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Moldy:
						index = 2;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Zippy:
						index = 3;
						flag = true;
						break;
					case RotrwMeatCore.CoreType.Weighty:
						index = 4;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				Object.Destroy(list[0].gameObject);
				Object.Destroy(list[1].gameObject);
				list.Clear();
				Object.Instantiate(Pie_Output[index].GetGameObject(), Pie_Output_Point.position, Pie_Output_Point.rotation);
			}
			if (flag)
			{
				if (GM.ZMaster != null)
				{
					GM.ZMaster.FlagM.AddToFlag("s_c", 1);
				}
				SM.PlayGenericSound(AudEvent_BakeSuccess, base.transform.position);
			}
			else
			{
				SM.PlayGenericSound(AudEvent_BakeFailure, base.transform.position);
			}
		}
	}
}
