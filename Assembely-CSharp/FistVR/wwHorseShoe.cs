using UnityEngine;

namespace FistVR
{
	public class wwHorseShoe : FVRPhysicalObject
	{
		private bool m_isShattered;

		public wwHorseShoePlinth Plinth;

		public GameObject ShatterFX;

		public GameObject SuccessParticles;

		public GameObject SuccessParticles2;

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (col.collider.attachedRigidbody != null && col.collider.attachedRigidbody.gameObject.CompareTag("HorseshoePost"))
			{
				wwHorseShoePost component = col.collider.attachedRigidbody.gameObject.GetComponent<wwHorseShoePost>();
				if (component != null && component.PostIndex == Plinth.PlinthIndex)
				{
					Plinth.HitSuccess();
					Object.Instantiate(SuccessParticles, Plinth.transform.position, Plinth.transform.rotation);
					Object.Instantiate(SuccessParticles2, col.collider.attachedRigidbody.gameObject.transform.position, col.collider.attachedRigidbody.gameObject.transform.rotation);
				}
			}
			Shatter();
		}

		private void Shatter()
		{
			if (!m_isShattered)
			{
				m_isShattered = true;
				if (m_hand != null)
				{
					FVRViveHand hand = m_hand;
					m_hand.ForceSetInteractable(null);
					EndInteraction(hand);
				}
				Object.Instantiate(ShatterFX, base.transform.position, base.transform.rotation);
				Plinth.NeedNewHorseshoe();
				Object.Destroy(base.gameObject);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (base.transform.position.y < -100f)
			{
				Shatter();
			}
			if (base.IsHeld || base.QuickbeltSlot != null)
			{
				DistanceCheck();
			}
		}

		private void DistanceCheck()
		{
			Vector2 a = new Vector2(base.transform.position.x, base.transform.position.z);
			Vector2 b = new Vector2(Plinth.transform.position.x, Plinth.transform.position.z);
			if (Vector2.Distance(a, b) > 2f)
			{
				Shatter();
			}
		}
	}
}
