using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ReticleSwapper : FVRInteractiveObject
	{
		public Text ReadoutText;

		public Renderer ReticleRenderer;

		public Texture2D[] Reticles;

		private int m_curReticle;

		public Transform Switch;

		public Vector3[] Eulers;

		public AudioSource Aud;

		public AudioClip Clip;

		private void ReticleForward()
		{
			m_curReticle++;
			if (m_curReticle >= Reticles.Length)
			{
				m_curReticle = 0;
			}
			ReticleRenderer.material.SetTexture("_MainTex", Reticles[m_curReticle]);
			ReadoutText.text = "Reticle " + (m_curReticle + 1);
			if (Switch != null)
			{
				Switch.localEulerAngles = Eulers[m_curReticle];
			}
			if (Aud != null)
			{
				Aud.PlayOneShot(Clip, 0.5f);
			}
		}

		private void ReticleBack()
		{
			m_curReticle--;
			if (m_curReticle < 0)
			{
				m_curReticle = Reticles.Length - 1;
			}
			ReticleRenderer.material.SetTexture("_MainTex", Reticles[m_curReticle]);
			ReadoutText.text = "Reticle " + (m_curReticle + 1);
			if (Switch != null)
			{
				Switch.localEulerAngles = Eulers[m_curReticle];
			}
			if (Aud != null)
			{
				Aud.PlayOneShot(Clip, 0.5f);
			}
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ReticleForward();
		}
	}
}
