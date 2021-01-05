using UnityEngine;

namespace FistVR
{
	public class SamplerPlatter_Starting : MonoBehaviour
	{
		public Renderer rend;

		public Texture2D[] Textures;

		public Texture2D[] Textures_Oculus;

		public Texture2D[] Textures_OculusStreamlined;

		public Texture2D[] Textures_Index;

		public Texture2D[] Textures_IndexStreamlined;

		private int m_index;

		public AudioEvent ScreenEvent;

		private ControlMode CMode;

		private bool m_isStreamlined;

		private bool m_hasInit;

		private void Start()
		{
			UpdateImage();
		}

		private void Update()
		{
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].HasInit)
				{
					CMode = GM.CurrentMovementManager.Hands[i].CMode;
					m_isStreamlined = GM.CurrentMovementManager.Hands[i].IsInStreamlinedMode;
					UpdateImage();
					m_hasInit = true;
				}
			}
		}

		public void Next()
		{
			SM.PlayGenericSound(ScreenEvent, base.transform.position);
			m_index++;
			if (m_index >= Textures.Length)
			{
				m_index = 0;
			}
			UpdateImage();
		}

		public void Previous()
		{
			SM.PlayGenericSound(ScreenEvent, base.transform.position);
			m_index--;
			if (m_index < 0)
			{
				m_index = Textures.Length - 1;
			}
			UpdateImage();
		}

		private void UpdateImage()
		{
			switch (CMode)
			{
			case ControlMode.Vive:
				rend.material.SetTexture("_MainTex", Textures[m_index]);
				rend.material.SetTexture("_IncandescenceMap", Textures[m_index]);
				break;
			case ControlMode.Oculus:
				if (m_isStreamlined && Textures_OculusStreamlined.Length > m_index && Textures_OculusStreamlined[m_index] != null)
				{
					rend.material.SetTexture("_MainTex", Textures_OculusStreamlined[m_index]);
					rend.material.SetTexture("_IncandescenceMap", Textures_OculusStreamlined[m_index]);
				}
				else
				{
					rend.material.SetTexture("_MainTex", Textures_Oculus[m_index]);
					rend.material.SetTexture("_IncandescenceMap", Textures_Oculus[m_index]);
				}
				break;
			case ControlMode.WMR:
				rend.material.SetTexture("_MainTex", Textures[m_index]);
				rend.material.SetTexture("_IncandescenceMap", Textures[m_index]);
				break;
			case ControlMode.Index:
				if (m_isStreamlined && Textures_IndexStreamlined.Length > m_index && Textures_IndexStreamlined[m_index] != null)
				{
					rend.material.SetTexture("_MainTex", Textures_IndexStreamlined[m_index]);
					rend.material.SetTexture("_IncandescenceMap", Textures_IndexStreamlined[m_index]);
				}
				else
				{
					rend.material.SetTexture("_MainTex", Textures_Index[m_index]);
					rend.material.SetTexture("_IncandescenceMap", Textures_Index[m_index]);
				}
				break;
			}
		}
	}
}
