using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_HelpScreen : MonoBehaviour
	{
		[Serializable]
		public class ReactiveTutorialImageSequence
		{
			public List<string> ObjectIDs;

			public List<FVRObject> Objects;

			public List<Texture2D> ImageSequence = new List<Texture2D>();

			public List<Texture2D> ImageSequence_Touch = new List<Texture2D>();

			public List<Texture2D> ImageSequence_TouchStreamlined = new List<Texture2D>();

			public List<Texture2D> ImageSequence_Index = new List<Texture2D>();

			public List<Texture2D> ImageSequence_IndexStreamlined = new List<Texture2D>();
		}

		public Texture2D PickUpObjectImage;

		public Texture2D PickUpObjectImage_Touch;

		public Texture2D PickUpObjectImage_Index;

		private Texture2D m_currentImage;

		public Renderer Rend;

		public List<ReactiveTutorialImageSequence> Sequences = new List<ReactiveTutorialImageSequence>();

		private Dictionary<string, ReactiveTutorialImageSequence> ObjTutDic = new Dictionary<string, ReactiveTutorialImageSequence>();

		private FVRPhysicalObject m_currentlyHeldObject;

		private ControlMode CMode;

		private bool m_isStreamlined;

		private void Start()
		{
			GM.CurrentSceneSettings.ObjectPickedUpEvent += ObjectPickedUp;
			for (int i = 0; i < Sequences.Count; i++)
			{
				for (int j = 0; j < Sequences[i].Objects.Count; j++)
				{
					ObjTutDic.Add(Sequences[i].Objects[j].ItemID, Sequences[i]);
				}
			}
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.ObjectPickedUpEvent -= ObjectPickedUp;
		}

		private void ObjectPickedUp(FVRPhysicalObject obj)
		{
			if (m_currentlyHeldObject != obj && obj.ObjectWrapper != null && ObjTutDic.ContainsKey(obj.ObjectWrapper.ItemID))
			{
				m_currentlyHeldObject = obj;
				UpdateImage();
			}
		}

		private void UpdateImage()
		{
			if (m_currentlyHeldObject != null && !m_currentlyHeldObject.IsHeld)
			{
				m_currentlyHeldObject = null;
			}
			if (m_currentlyHeldObject == null)
			{
				switch (CMode)
				{
				case ControlMode.Vive:
					SetMaterialTexture(PickUpObjectImage);
					break;
				case ControlMode.Oculus:
					SetMaterialTexture(PickUpObjectImage_Touch);
					break;
				case ControlMode.WMR:
					SetMaterialTexture(PickUpObjectImage);
					break;
				case ControlMode.Index:
					SetMaterialTexture(PickUpObjectImage_Index);
					break;
				}
				return;
			}
			int tutorialState = m_currentlyHeldObject.GetTutorialState();
			ReactiveTutorialImageSequence reactiveTutorialImageSequence = ObjTutDic[m_currentlyHeldObject.ObjectWrapper.ItemID];
			switch (CMode)
			{
			case ControlMode.Vive:
				SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence[tutorialState]);
				break;
			case ControlMode.Oculus:
				if (m_isStreamlined && reactiveTutorialImageSequence.ImageSequence_TouchStreamlined.Count > tutorialState && reactiveTutorialImageSequence.ImageSequence_TouchStreamlined[tutorialState] != null)
				{
					SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence_TouchStreamlined[tutorialState]);
				}
				else
				{
					SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence_Touch[tutorialState]);
				}
				break;
			case ControlMode.WMR:
				SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence[tutorialState]);
				break;
			case ControlMode.Index:
				if (m_isStreamlined && reactiveTutorialImageSequence.ImageSequence_IndexStreamlined.Count > tutorialState && reactiveTutorialImageSequence.ImageSequence_IndexStreamlined[tutorialState] != null)
				{
					SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence_IndexStreamlined[tutorialState]);
				}
				else
				{
					SetMaterialTexture(reactiveTutorialImageSequence.ImageSequence_Index[tutorialState]);
				}
				break;
			}
		}

		private void Update()
		{
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].HasInit)
				{
					CMode = GM.CurrentMovementManager.Hands[i].CMode;
					m_isStreamlined = GM.CurrentMovementManager.Hands[i].IsInStreamlinedMode;
				}
			}
			UpdateImage();
		}

		private void SetMaterialTexture(Texture2D t)
		{
			if (m_currentImage != t)
			{
				m_currentImage = t;
				Rend.material.SetTexture("_MainTex", t);
				Rend.material.SetTexture("_IncandescenceMap", t);
			}
		}
	}
}
