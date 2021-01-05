using UnityEngine;

namespace FistVR
{
	public class wwHorseShoePlinth : MonoBehaviour
	{
		public int PlinthIndex;

		public wwHorseShoeGame Game;

		public GameObject HorseShoeTrigger;

		private bool m_isShoeActive;

		public GameObject GlyphNotCompleted;

		public GameObject GlyphCompleted;

		public Transform spinnybit;

		private float m_spinnyRot;

		public AudioEvent SuccessEvent;

		private bool m_isCompleted;

		public bool IsCompleted()
		{
			return m_isCompleted;
		}

		public void SetCompleted()
		{
			m_isCompleted = true;
			GlyphNotCompleted.SetActive(value: false);
			GlyphCompleted.SetActive(value: true);
		}

		public void HitSuccess()
		{
			SM.PlayGenericSound(SuccessEvent, base.transform.position);
			if (!m_isCompleted)
			{
				m_isCompleted = true;
				Game.RegisterSuccess(PlinthIndex);
			}
			SetCompleted();
		}

		public void GrabbedHorseshoe()
		{
			m_isShoeActive = true;
		}

		public void NeedNewHorseshoe()
		{
			m_isShoeActive = false;
		}

		public void Update()
		{
			if (!m_isShoeActive)
			{
				m_spinnyRot += 360f * Time.deltaTime;
				spinnybit.localEulerAngles = new Vector3(0f, m_spinnyRot, 0f);
			}
		}
	}
}
