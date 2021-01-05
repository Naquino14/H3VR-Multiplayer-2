using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PlayerSosigBody : MonoBehaviour
	{
		private Transform head;

		private Transform torso;

		public Rigidbody RB;

		public Transform Sosig_Head;

		public Transform Sosig_Torso;

		public Transform Sosig_Abdomen;

		public Transform Sosig_Legs;

		private List<GameObject> m_curClothes = new List<GameObject>();

		private float AttachedRotationMultiplier = 30f;

		private float AttachedPositionMultiplier = 4500f;

		private float AttachedRotationFudge = 500f;

		private float AttachedPositionFudge = 500f;

		private void Start()
		{
			head = GM.CurrentPlayerBody.Head;
			torso = GM.CurrentPlayerBody.Torso;
		}

		private void FixedUpdate()
		{
			if (DistanceFromCoreTarget() > 1f)
			{
				RB.position = torso.position - torso.up * 0.25f;
			}
			SosigPhys();
		}

		private void SosigPhys()
		{
			Vector3 position = RB.position;
			Quaternion rotation = RB.rotation;
			Vector3 vector = torso.position - torso.up * 0.25f;
			Quaternion rotation2 = torso.rotation;
			Vector3 vector2 = vector - position;
			Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation);
			float deltaTime = Time.deltaTime;
			quaternion.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = deltaTime * angle * axis * AttachedRotationMultiplier;
				RB.angularVelocity = Vector3.MoveTowards(RB.angularVelocity, target, AttachedRotationFudge * Time.fixedDeltaTime);
			}
			Vector3 target2 = vector2 * AttachedPositionMultiplier * deltaTime;
			RB.velocity = Vector3.MoveTowards(RB.velocity, target2, AttachedPositionFudge * deltaTime);
		}

		public float DistanceFromCoreTarget()
		{
			return Vector3.Distance(RB.position, torso.position);
		}

		public void ApplyOutfit(SosigOutfitConfig o)
		{
			if (m_curClothes.Count > 0)
			{
				for (int num = m_curClothes.Count - 1; num >= 0; num--)
				{
					if (m_curClothes[num] != null)
					{
						Object.Destroy(m_curClothes[num]);
					}
				}
			}
			m_curClothes.Clear();
			SpawnAccesoryToLink(o.Headwear, Sosig_Head, o.Chance_Headwear);
			SpawnAccesoryToLink(o.Facewear, Sosig_Head, o.Chance_Facewear);
			SpawnAccesoryToLink(o.Eyewear, Sosig_Head, o.Chance_Eyewear);
			SpawnAccesoryToLink(o.Torsowear, Sosig_Torso, o.Chance_Torsowear);
			SpawnAccesoryToLink(o.Pantswear, Sosig_Abdomen, o.Chance_Pantswear);
			SpawnAccesoryToLink(o.Pantswear_Lower, Sosig_Legs, o.Chance_Pantswear_Lower);
			SpawnAccesoryToLink(o.Backpacks, Sosig_Torso, o.Chance_Backpacks);
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, Transform l, float chance)
		{
			if (Random.Range(0f, 1f) > chance || gs.Count < 1)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.position, l.rotation);
			m_curClothes.Add(gameObject);
			Component[] componentsInChildren = gameObject.GetComponentsInChildren<Component>(includeInactive: true);
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				componentsInChildren[num].gameObject.layer = LayerMask.NameToLayer("ExternalCamOnly");
				if (!(componentsInChildren[num] is Transform) && !(componentsInChildren[num] is MeshFilter) && !(componentsInChildren[num] is MeshRenderer))
				{
					Object.Destroy(componentsInChildren[num]);
				}
			}
			gameObject.transform.SetParent(l);
		}
	}
}
