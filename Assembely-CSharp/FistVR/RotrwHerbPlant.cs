using UnityEngine;

namespace FistVR
{
	public class RotrwHerbPlant : FVRInteractiveObject
	{
		public RotrwHerb.HerbType Type;

		public GameObject VisibleFruit;

		private bool m_hasFruit = true;

		public FVRObject FruitObject;

		public AudioEvent AudEvent_PickFruit;

		private float m_timeTilRegrow;

		public override bool IsInteractable()
		{
			return m_hasFruit;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_hasFruit = false;
			PickFruit();
			GameObject gameObject = Object.Instantiate(FruitObject.GetGameObject(), VisibleFruit.transform.position, VisibleFruit.transform.rotation);
			FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
			hand.ForceSetInteractable(component);
			component.BeginInteraction(hand);
		}

		private void PickFruit()
		{
			m_hasFruit = false;
			VisibleFruit.SetActive(value: false);
			SM.PlayGenericSound(AudEvent_PickFruit, VisibleFruit.transform.position);
			m_timeTilRegrow = Random.Range(180f, 360f);
		}

		public void Update()
		{
			if (!m_hasFruit)
			{
				if (m_timeTilRegrow > 0f)
				{
					m_timeTilRegrow -= Time.deltaTime;
				}
				else
				{
					RegrowFruit();
				}
			}
		}

		public void RegrowFruit()
		{
			m_hasFruit = true;
			VisibleFruit.SetActive(value: true);
		}
	}
}
