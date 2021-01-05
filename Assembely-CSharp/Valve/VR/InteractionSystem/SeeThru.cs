using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SeeThru : MonoBehaviour
	{
		public Material seeThruMaterial;

		private GameObject seeThru;

		private Interactable interactable;

		private Renderer sourceRenderer;

		private Renderer destRenderer;

		private void Awake()
		{
			interactable = GetComponentInParent<Interactable>();
			seeThru = new GameObject("_see_thru");
			seeThru.transform.parent = base.transform;
			seeThru.transform.localPosition = Vector3.zero;
			seeThru.transform.localRotation = Quaternion.identity;
			seeThru.transform.localScale = Vector3.one;
			MeshFilter component = GetComponent<MeshFilter>();
			if (component != null)
			{
				MeshFilter meshFilter = seeThru.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = component.sharedMesh;
			}
			MeshRenderer component2 = GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				sourceRenderer = component2;
				destRenderer = seeThru.AddComponent<MeshRenderer>();
			}
			SkinnedMeshRenderer component3 = GetComponent<SkinnedMeshRenderer>();
			if (component3 != null)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = seeThru.AddComponent<SkinnedMeshRenderer>();
				sourceRenderer = component3;
				destRenderer = skinnedMeshRenderer;
				skinnedMeshRenderer.sharedMesh = component3.sharedMesh;
				skinnedMeshRenderer.rootBone = component3.rootBone;
				skinnedMeshRenderer.bones = component3.bones;
				skinnedMeshRenderer.quality = component3.quality;
				skinnedMeshRenderer.updateWhenOffscreen = component3.updateWhenOffscreen;
			}
			if (sourceRenderer != null && destRenderer != null)
			{
				int num = sourceRenderer.sharedMaterials.Length;
				Material[] array = new Material[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = seeThruMaterial;
				}
				destRenderer.sharedMaterials = array;
				for (int j = 0; j < destRenderer.materials.Length; j++)
				{
					destRenderer.materials[j].renderQueue = 2001;
				}
				for (int k = 0; k < sourceRenderer.materials.Length; k++)
				{
					if (sourceRenderer.materials[k].renderQueue == 2000)
					{
						sourceRenderer.materials[k].renderQueue = 2002;
					}
				}
			}
			seeThru.gameObject.SetActive(value: false);
		}

		private void OnEnable()
		{
			interactable.onAttachedToHand += AttachedToHand;
			interactable.onDetachedFromHand += DetachedFromHand;
		}

		private void OnDisable()
		{
			interactable.onAttachedToHand -= AttachedToHand;
			interactable.onDetachedFromHand -= DetachedFromHand;
		}

		private void AttachedToHand(Hand hand)
		{
			seeThru.SetActive(value: true);
		}

		private void DetachedFromHand(Hand hand)
		{
			seeThru.SetActive(value: false);
		}

		private void Update()
		{
			if (seeThru.activeInHierarchy)
			{
				int num = Mathf.Min(sourceRenderer.materials.Length, destRenderer.materials.Length);
				for (int i = 0; i < num; i++)
				{
					destRenderer.materials[i].mainTexture = sourceRenderer.materials[i].mainTexture;
					destRenderer.materials[i].color = destRenderer.materials[i].color * sourceRenderer.materials[i].color;
				}
			}
		}
	}
}
