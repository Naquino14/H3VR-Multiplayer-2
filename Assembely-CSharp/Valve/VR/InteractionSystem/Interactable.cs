using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Interactable : MonoBehaviour
	{
		public delegate void OnAttachedToHandDelegate(Hand hand);

		public delegate void OnDetachedFromHandDelegate(Hand hand);

		[Tooltip("Activates an action set on attach and deactivates on detach")]
		public SteamVR_ActionSet activateActionSetOnAttach;

		[Tooltip("Hide the whole hand on attachment and show on detach")]
		public bool hideHandOnAttach = true;

		[Tooltip("Hide the skeleton part of the hand on attachment and show on detach")]
		public bool hideSkeletonOnAttach;

		[Tooltip("Hide the controller part of the hand on attachment and show on detach")]
		public bool hideControllerOnAttach;

		[Tooltip("The integer in the animator to trigger on pickup. 0 for none")]
		public int handAnimationOnPickup;

		[Tooltip("The range of motion to set on the skeleton. None for no change.")]
		public SkeletalMotionRangeChange setRangeOfMotionOnPickup = SkeletalMotionRangeChange.None;

		[Tooltip("Specify whether you want to snap to the hand's object attachment point, or just the raw hand")]
		public bool useHandObjectAttachmentPoint = true;

		public bool attachEaseIn;

		[HideInInspector]
		public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public float snapAttachEaseInTime = 0.15f;

		public bool snapAttachEaseInCompleted;

		[HideInInspector]
		public SteamVR_Skeleton_Poser skeletonPoser;

		[Tooltip("Should the rendered hand lock on to and follow the object")]
		public bool handFollowTransform = true;

		[Tooltip("Set whether or not you want this interactible to highlight when hovering over it")]
		public bool highlightOnHover = true;

		protected MeshRenderer[] highlightRenderers;

		protected MeshRenderer[] existingRenderers;

		protected GameObject highlightHolder;

		protected SkinnedMeshRenderer[] highlightSkinnedRenderers;

		protected SkinnedMeshRenderer[] existingSkinnedRenderers;

		protected static Material highlightMat;

		[Tooltip("An array of child gameObjects to not render a highlight for. Things like transparent parts, vfx, etc.")]
		public GameObject[] hideHighlight;

		[NonSerialized]
		public Hand attachedToHand;

		[NonSerialized]
		public Hand hoveringHand;

		protected float blendToPoseTime = 0.1f;

		protected float releasePoseBlendTime = 0.2f;

		public bool isDestroying
		{
			get;
			protected set;
		}

		public bool isHovering
		{
			get;
			protected set;
		}

		public bool wasHovering
		{
			get;
			protected set;
		}

		public event OnAttachedToHandDelegate onAttachedToHand;

		public event OnDetachedFromHandDelegate onDetachedFromHand;

		private void Awake()
		{
			skeletonPoser = GetComponent<SteamVR_Skeleton_Poser>();
		}

		protected virtual void Start()
		{
			highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));
			if (highlightMat == null)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");
			}
			if (skeletonPoser != null && useHandObjectAttachmentPoint)
			{
				useHandObjectAttachmentPoint = false;
			}
		}

		protected virtual bool ShouldIgnoreHighlight(Component component)
		{
			return ShouldIgnore(component.gameObject);
		}

		protected virtual bool ShouldIgnore(GameObject check)
		{
			for (int i = 0; i < hideHighlight.Length; i++)
			{
				if (check == hideHighlight[i])
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void CreateHighlightRenderers()
		{
			existingSkinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
			highlightHolder = new GameObject("Highlighter");
			highlightSkinnedRenderers = new SkinnedMeshRenderer[existingSkinnedRenderers.Length];
			for (int i = 0; i < existingSkinnedRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = existingSkinnedRenderers[i];
				if (!ShouldIgnoreHighlight(skinnedMeshRenderer))
				{
					GameObject gameObject = new GameObject("SkinnedHolder");
					gameObject.transform.parent = highlightHolder.transform;
					SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject.AddComponent<SkinnedMeshRenderer>();
					Material[] array = new Material[skinnedMeshRenderer.sharedMaterials.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = highlightMat;
					}
					skinnedMeshRenderer2.sharedMaterials = array;
					skinnedMeshRenderer2.sharedMesh = skinnedMeshRenderer.sharedMesh;
					skinnedMeshRenderer2.rootBone = skinnedMeshRenderer.rootBone;
					skinnedMeshRenderer2.updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
					skinnedMeshRenderer2.bones = skinnedMeshRenderer.bones;
					highlightSkinnedRenderers[i] = skinnedMeshRenderer2;
				}
			}
			MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(includeInactive: true);
			existingRenderers = new MeshRenderer[componentsInChildren.Length];
			highlightRenderers = new MeshRenderer[componentsInChildren.Length];
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				MeshFilter meshFilter = componentsInChildren[k];
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				if (!(meshFilter == null) && !(component == null) && !ShouldIgnoreHighlight(meshFilter))
				{
					GameObject gameObject2 = new GameObject("FilterHolder");
					gameObject2.transform.parent = highlightHolder.transform;
					MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
					meshFilter2.sharedMesh = meshFilter.sharedMesh;
					MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
					Material[] array2 = new Material[component.sharedMaterials.Length];
					for (int l = 0; l < array2.Length; l++)
					{
						array2[l] = highlightMat;
					}
					meshRenderer.sharedMaterials = array2;
					highlightRenderers[k] = meshRenderer;
					existingRenderers[k] = component;
				}
			}
		}

		protected virtual void UpdateHighlightRenderers()
		{
			if (highlightHolder == null)
			{
				return;
			}
			for (int i = 0; i < existingSkinnedRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = existingSkinnedRenderers[i];
				SkinnedMeshRenderer skinnedMeshRenderer2 = highlightSkinnedRenderers[i];
				if (skinnedMeshRenderer != null && skinnedMeshRenderer2 != null && !attachedToHand)
				{
					skinnedMeshRenderer2.transform.position = skinnedMeshRenderer.transform.position;
					skinnedMeshRenderer2.transform.rotation = skinnedMeshRenderer.transform.rotation;
					skinnedMeshRenderer2.transform.localScale = skinnedMeshRenderer.transform.lossyScale;
					skinnedMeshRenderer2.localBounds = skinnedMeshRenderer.localBounds;
					skinnedMeshRenderer2.enabled = isHovering && skinnedMeshRenderer.enabled && skinnedMeshRenderer.gameObject.activeInHierarchy;
					int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
					for (int j = 0; j < blendShapeCount; j++)
					{
						skinnedMeshRenderer2.SetBlendShapeWeight(j, skinnedMeshRenderer.GetBlendShapeWeight(j));
					}
				}
				else if (skinnedMeshRenderer2 != null)
				{
					skinnedMeshRenderer2.enabled = false;
				}
			}
			for (int k = 0; k < highlightRenderers.Length; k++)
			{
				MeshRenderer meshRenderer = existingRenderers[k];
				MeshRenderer meshRenderer2 = highlightRenderers[k];
				if (meshRenderer != null && meshRenderer2 != null && !attachedToHand)
				{
					meshRenderer2.transform.position = meshRenderer.transform.position;
					meshRenderer2.transform.rotation = meshRenderer.transform.rotation;
					meshRenderer2.transform.localScale = meshRenderer.transform.lossyScale;
					meshRenderer2.enabled = isHovering && meshRenderer.enabled && meshRenderer.gameObject.activeInHierarchy;
				}
				else if (meshRenderer2 != null)
				{
					meshRenderer2.enabled = false;
				}
			}
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			wasHovering = isHovering;
			isHovering = true;
			hoveringHand = hand;
			if (highlightOnHover)
			{
				CreateHighlightRenderers();
				UpdateHighlightRenderers();
			}
		}

		private void OnHandHoverEnd(Hand hand)
		{
			wasHovering = isHovering;
			isHovering = false;
			if (highlightOnHover && highlightHolder != null)
			{
				UnityEngine.Object.Destroy(highlightHolder);
			}
		}

		protected virtual void Update()
		{
			if (highlightOnHover)
			{
				UpdateHighlightRenderers();
				if (!isHovering && highlightHolder != null)
				{
					UnityEngine.Object.Destroy(highlightHolder);
				}
			}
		}

		protected virtual void OnAttachedToHand(Hand hand)
		{
			if (activateActionSetOnAttach != null)
			{
				activateActionSetOnAttach.Activate(hand.handType);
			}
			if (this.onAttachedToHand != null)
			{
				this.onAttachedToHand(hand);
			}
			if (skeletonPoser != null && hand.skeleton != null)
			{
				hand.skeleton.BlendToPoser(skeletonPoser, blendToPoseTime);
			}
			attachedToHand = hand;
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			if (activateActionSetOnAttach != null && (hand.otherHand == null || !hand.otherHand.currentAttachedObjectInfo.HasValue || (hand.otherHand.currentAttachedObjectInfo.Value.interactable != null && hand.otherHand.currentAttachedObjectInfo.Value.interactable.activateActionSetOnAttach != activateActionSetOnAttach)))
			{
				activateActionSetOnAttach.Deactivate(hand.handType);
			}
			if (this.onDetachedFromHand != null)
			{
				this.onDetachedFromHand(hand);
			}
			if (skeletonPoser != null && hand.skeleton != null)
			{
				hand.skeleton.BlendToSkeleton(releasePoseBlendTime);
			}
			attachedToHand = null;
		}

		protected virtual void OnDestroy()
		{
			isDestroying = true;
			if (attachedToHand != null)
			{
				attachedToHand.DetachObject(base.gameObject, restoreOriginalParent: false);
				attachedToHand.skeleton.BlendToSkeleton();
			}
			if (highlightHolder != null)
			{
				UnityEngine.Object.Destroy(highlightHolder);
			}
		}

		protected virtual void OnDisable()
		{
			isDestroying = true;
			if (attachedToHand != null)
			{
				attachedToHand.ForceHoverUnlock();
			}
			if (highlightHolder != null)
			{
				UnityEngine.Object.Destroy(highlightHolder);
			}
		}
	}
}
