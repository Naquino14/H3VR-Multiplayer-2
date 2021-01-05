using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class LectureMaster : MonoBehaviour
	{
		public enum LectureCam
		{
			CloseStandard,
			CloseStandard2,
			CloseStandard3,
			CloseStandard4,
			MidStandard,
			MidStandard2,
			MidStandard3,
			MidLeft,
			MidRight,
			MidLow,
			WideCorner,
			WideCentered,
			WideHigh,
			WideHighAngleLeft,
			WideHighAngleRight,
			WideLow
		}

		public List<GameObject> Cameras;

		public List<Transform> Slides;

		public SlideSequenceDef SlidesDef;

		public List<FVRViveHand> Hands;

		private int curSlide = -1;

		public Text Notes;

		private FVRObject CurFO_Head;

		private FVRObject CurFO_Torso;

		private FVRObject CurFO_Legs;

		private GameObject m_cur_head;

		private GameObject m_cur_torso;

		private GameObject m_cur_legs;

		public Transform Targ_Head;

		public Transform Targ_Torso;

		public Transform Targ_Legs;

		private float refire = 0.2f;

		private void Start()
		{
		}

		private void UpdateClothing()
		{
			MaybeReplace(CurFO_Head, SlidesDef.Slides[curSlide].Head, ref m_cur_head, Targ_Head);
			MaybeReplace(CurFO_Torso, SlidesDef.Slides[curSlide].Torso, ref m_cur_torso, Targ_Torso);
			MaybeReplace(CurFO_Legs, SlidesDef.Slides[curSlide].Abdomen, ref m_cur_legs, Targ_Legs);
		}

		private void MaybeReplace(FVRObject curFO, FVRObject slideFO, ref GameObject curPiece, Transform targ)
		{
			if (curFO == null || slideFO == null || slideFO.ItemID != curFO.ItemID)
			{
				if (curPiece != null)
				{
					curPiece.transform.SetParent(null);
					Object.Destroy(curPiece);
				}
				if (slideFO != null)
				{
					curPiece = Object.Instantiate(slideFO.GetGameObject(), targ.position, targ.rotation);
					curPiece.transform.SetParent(targ);
					curPiece.layer = LayerMask.NameToLayer("ExternalCamOnly");
				}
			}
		}

		private void Update()
		{
			if (refire > 0f)
			{
				refire -= Time.deltaTime;
			}
			for (int i = 0; i < Hands.Count; i++)
			{
				if (Hands[i].Input.AXButtonDown)
				{
					if (Hands[i].IsThisTheRightHand)
					{
						Advance();
					}
					else
					{
						GoBack();
					}
				}
			}
		}

		private void Advance()
		{
			if (!(refire > 0f))
			{
				refire = 0.2f;
				curSlide++;
				if (curSlide >= SlidesDef.Slides.Count)
				{
					curSlide = SlidesDef.Slides.Count - 1;
				}
				UpdateCam();
				UpdateSlide();
				UpdateText();
				UpdateClothing();
			}
		}

		private void GoBack()
		{
			if (!(refire > 0f))
			{
				refire = 0.2f;
				curSlide--;
				if (curSlide < 0)
				{
					curSlide = 0;
				}
				UpdateCam();
				UpdateSlide();
				UpdateText();
				UpdateClothing();
			}
		}

		private void UpdateCam()
		{
			for (int i = 0; i < Cameras.Count; i++)
			{
				if (i == (int)SlidesDef.Slides[curSlide].CameraIndex)
				{
					Cameras[i].SetActive(value: true);
				}
				else
				{
					Cameras[i].SetActive(value: false);
				}
			}
		}

		private void UpdateSlide()
		{
			for (int i = 0; i < Slides.Count; i++)
			{
				if (i == SlidesDef.Slides[curSlide].SlideIndex)
				{
					Slides[i].gameObject.SetActive(value: true);
				}
				else
				{
					Slides[i].gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdateText()
		{
			Notes.text = SlidesDef.Slides[curSlide].text;
		}
	}
}
