using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Whizzbangadinger : MonoBehaviour
	{
		public Transform Door;

		public LayerMask LM_IngredientDetect;

		public Transform IngredientDetectCenter;

		public Vector3 IngredientDetectExtends;

		public List<FVRObject> Bangers_Small;

		public List<FVRObject> Bangers_Medium;

		public List<FVRObject> Bangers_Large;

		public AudioEvent AudEvent_DingSuccess;

		public AudioEvent AudEvent_DingFailure;

		public Transform SpawnPoint;

		public Transform Accordian;

		private float m_accordianLerp;

		private bool m_isAnimating;

		public AnimationCurve AccordianCurve;

		public float AccordianSpeed = 1f;

		public AudioEvent AudEvent_Accordian;

		public BangerDetonator Detonator;

		private bool IsDoorShut()
		{
			if (Vector3.Angle(Door.forward, -base.transform.right) < 2f)
			{
				return true;
			}
			return false;
		}

		private void SuccessSound()
		{
			SM.PlayGenericSound(AudEvent_DingSuccess, base.transform.position);
		}

		private void FailSound()
		{
			SM.PlayGenericSound(AudEvent_DingFailure, base.transform.position);
		}

		public void Ding(int v)
		{
			if (!IsDoorShut())
			{
				FailSound();
				return;
			}
			bool flag = false;
			bool flag2 = false;
			RotrwBangerJunk rotrwBangerJunk = null;
			RotrwBangerJunk rotrwBangerJunk2 = null;
			List<FVRPhysicalObject> list = new List<FVRPhysicalObject>();
			Collider[] array = Physics.OverlapBox(IngredientDetectCenter.position, IngredientDetectExtends, IngredientDetectCenter.rotation, LM_IngredientDetect, QueryTriggerInteraction.Collide);
			List<Rigidbody> list2 = new List<Rigidbody>();
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody attachedRigidbody = array[i].attachedRigidbody;
				if (attachedRigidbody != null && !list2.Contains(attachedRigidbody))
				{
					list2.Add(attachedRigidbody);
				}
			}
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				Rigidbody rigidbody = list2[num];
				RotrwBangerJunk component = rigidbody.gameObject.GetComponent<RotrwBangerJunk>();
				if (component != null)
				{
					if (component.Type == RotrwBangerJunk.BangerJunkType.Bucket || component.Type == RotrwBangerJunk.BangerJunkType.CoffeeCan || component.Type == RotrwBangerJunk.BangerJunkType.TinCan)
					{
						flag = true;
						if (rotrwBangerJunk == null)
						{
							rotrwBangerJunk = component;
						}
						else if (component.ContainerSize > rotrwBangerJunk.ContainerSize)
						{
							rotrwBangerJunk = component;
						}
						list2.RemoveAt(num);
					}
					else if (component.Type == RotrwBangerJunk.BangerJunkType.BangSnaps || component.Type == RotrwBangerJunk.BangerJunkType.EggTimer || component.Type == RotrwBangerJunk.BangerJunkType.FishFinder || component.Type == RotrwBangerJunk.BangerJunkType.Radio)
					{
						flag2 = true;
						if (rotrwBangerJunk2 == null)
						{
							rotrwBangerJunk2 = component;
						}
						list2.RemoveAt(num);
					}
				}
			}
			if (!flag || !flag2)
			{
				FailSound();
				return;
			}
			int num2 = 2;
			if (rotrwBangerJunk.ContainerSize == 1)
			{
				num2 = 5;
			}
			else if (rotrwBangerJunk.ContainerSize == 2)
			{
				num2 = 9999;
			}
			int num3 = 0;
			for (int j = 0; j < list2.Count; j++)
			{
				if (num3 >= num2)
				{
					break;
				}
				FVRPhysicalObject component2 = list2[j].gameObject.GetComponent<FVRPhysicalObject>();
				if (component2.ObjectWrapper != null)
				{
					num3++;
					list.Add(component2);
				}
			}
			if (num3 > 0)
			{
				int num4 = 0;
				FVRObject fVRObject = null;
				int num5 = 0;
				int index = 0;
				num5 = rotrwBangerJunk.MatIndex;
				switch (rotrwBangerJunk.Type)
				{
				case RotrwBangerJunk.BangerJunkType.TinCan:
					num4 = 0;
					break;
				case RotrwBangerJunk.BangerJunkType.CoffeeCan:
					num4 = 1;
					break;
				case RotrwBangerJunk.BangerJunkType.Bucket:
					num4 = 2;
					break;
				}
				switch (rotrwBangerJunk2.Type)
				{
				case RotrwBangerJunk.BangerJunkType.BangSnaps:
					index = 0;
					break;
				case RotrwBangerJunk.BangerJunkType.EggTimer:
					index = 1;
					break;
				case RotrwBangerJunk.BangerJunkType.Radio:
					index = 2;
					break;
				case RotrwBangerJunk.BangerJunkType.FishFinder:
					index = 3;
					break;
				}
				switch (num4)
				{
				case 0:
					fVRObject = Bangers_Small[index];
					break;
				case 1:
					fVRObject = Bangers_Medium[index];
					break;
				case 2:
					fVRObject = Bangers_Large[index];
					break;
				}
				GameObject gameObject = Object.Instantiate(fVRObject.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
				Banger component3 = gameObject.GetComponent<Banger>();
				if (GM.ZMaster != null)
				{
					GM.ZMaster.FlagM.AddToFlag("s_c", 1);
				}
				if (num5 > 0)
				{
					component3.SetMat(num5);
				}
				for (int k = 0; k < list.Count; k++)
				{
					component3.LoadPayload(list[k]);
				}
				component3.Complete();
				m_accordianLerp = 0f;
				m_isAnimating = true;
				SM.PlayGenericSound(AudEvent_Accordian, base.transform.position);
				SuccessSound();
				if (component3.BType == Banger.BangerType.Remote)
				{
					Detonator.RegisterBanger(component3);
				}
				Object.Destroy(rotrwBangerJunk.gameObject);
				Object.Destroy(rotrwBangerJunk2.gameObject);
				for (int num6 = list.Count - 1; num6 >= 0; num6--)
				{
					Object.Destroy(list[num6].gameObject);
				}
				list.Clear();
			}
			else
			{
				FailSound();
			}
		}

		private void Update()
		{
			Accordianing();
		}

		private void Accordianing()
		{
			if (m_isAnimating)
			{
				m_accordianLerp += Time.deltaTime;
				float y = AccordianCurve.Evaluate(m_accordianLerp);
				Accordian.localScale = new Vector3(1f, y, 1f);
				if (m_accordianLerp > 1f)
				{
					m_isAnimating = false;
				}
			}
		}
	}
}
