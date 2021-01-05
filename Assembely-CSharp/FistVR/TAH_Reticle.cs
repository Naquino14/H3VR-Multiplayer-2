using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_Reticle : MonoBehaviour
	{
		[Header("Prefabs")]
		public GameObject P_Contact;

		public AIEntity IFFScanner;

		public AITargetPrioritySystem Priority;

		public List<TAH_ReticleContact> Contacts = new List<TAH_ReticleContact>();

		private HashSet<Transform> m_trackedTransforms = new HashSet<Transform>();

		public float Range = 50f;

		public bool UsesHealthRing;

		public Renderer HealthRingRend;

		public Color Health_Full;

		public Color Health_Empty;

		public bool UsesVerticality = true;

		public bool ShowArrows = true;

		public bool RegistersEnemies = true;

		public bool UsesReticlePos;

		private void Start()
		{
			Priority.Init(IFFScanner, 20, 1f, 2f);
			IFFScanner.AIEventReceiveEvent += EventReceive;
		}

		private void OnDestroy()
		{
			IFFScanner.AIEventReceiveEvent -= EventReceive;
		}

		public void EventReceive(AIEvent e)
		{
			if (RegistersEnemies && (!e.IsEntity || e.Entity.IFFCode != IFFScanner.IFFCode) && e.Type == AIEvent.AIEType.Visual)
			{
				Priority.ProcessEvent(e);
			}
		}

		public TAH_ReticleContact RegisterTrackedObject(Transform obj, TAH_ReticleContact.ContactType type)
		{
			if (m_trackedTransforms.Contains(obj))
			{
				return null;
			}
			m_trackedTransforms.Add(obj);
			GameObject gameObject = Object.Instantiate(P_Contact, base.transform.position, base.transform.rotation, base.transform);
			TAH_ReticleContact component = gameObject.GetComponent<TAH_ReticleContact>();
			component.UsesVerticality = UsesVerticality;
			component.ShowArrows = ShowArrows;
			Contacts.Add(component);
			component.InitContact(type, obj, Range);
			return component;
		}

		public void DeRegisterTrackedType(TAH_ReticleContact.ContactType type)
		{
			for (int num = Contacts.Count - 1; num >= 0; num--)
			{
				if (Contacts[num].Type == type)
				{
					m_trackedTransforms.Remove(Contacts[num].TrackedTransform);
					Object.Destroy(Contacts[num].gameObject);
					Contacts.RemoveAt(num);
				}
			}
		}

		private void Update()
		{
			if (UsesHealthRing)
			{
				float playerHealth = GM.GetPlayerHealth();
				Color value = Color.Lerp(Health_Empty, Health_Full, playerHealth);
				playerHealth = 0f - playerHealth;
				playerHealth += 0.5f;
				HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0f, playerHealth));
				HealthRingRend.material.SetColor("_EmissionColor", value);
				Vector3 forward = Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.forward, base.transform.up);
				HealthRingRend.transform.rotation = Quaternion.LookRotation(forward, base.transform.up);
			}
			Priority.Compute();
			for (int i = 0; i < Priority.RecentEvents.Count; i++)
			{
				if (Priority.RecentEvents[i].IsEntity)
				{
					RegisterTrackedObject(Priority.RecentEvents[i].Entity.transform, TAH_ReticleContact.ContactType.Enemy);
				}
			}
			UpdateContacts();
		}

		private void UpdateContacts()
		{
			Vector3 position = GM.CurrentPlayerRoot.position;
			if (UsesReticlePos)
			{
				position = base.transform.position;
			}
			for (int num = Contacts.Count - 1; num >= 0; num--)
			{
				if (!Contacts[num].Tick(position))
				{
					m_trackedTransforms.Remove(Contacts[num].TrackedTransform);
					Object.Destroy(Contacts[num].gameObject);
					Contacts.RemoveAt(num);
				}
			}
		}
	}
}
