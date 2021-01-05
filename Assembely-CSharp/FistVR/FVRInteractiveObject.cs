using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRInteractiveObject : MonoBehaviour
	{
		public enum HandFilterMode
		{
			Unfiltered,
			FilterMode_A,
			FilterMode_B
		}

		[NonSerialized]
		public GameObject GameObject;

		[NonSerialized]
		public Transform Transform;

		[NonSerialized]
		private int m_index = -1;

		public static List<FVRInteractiveObject> All = new List<FVRInteractiveObject>();

		[Header("Interactive Object Config")]
		public FVRInteractionControlType ControlType;

		public bool IsSimpleInteract;

		public HandlingGrabType HandlingGrabSound;

		public HandlingReleaseType HandlingReleaseSound;

		public Transform PoseOverride;

		public Transform QBPoseOverride;

		public Transform PoseOverride_Touch;

		public bool UseGrabPointChild;

		public bool UseGripRotInterp;

		public float PositionInterpSpeed = 1f;

		public float RotationInterpSpeed = 1f;

		protected Transform m_grabPointTransform;

		protected float m_pos_interp_tick;

		protected float m_rot_interp_tick;

		public bool EndInteractionIfDistant = true;

		public float EndInteractionDistance = 0.25f;

		[HideInInspector]
		public bool m_hasTriggeredUpSinceBegin;

		protected float triggerCooldown = 0.5f;

		public FVRViveHand m_hand;

		public GameObject UXGeo_Hover;

		public GameObject UXGeo_Held;

		public bool UseFilteredHandTransform;

		public bool UseFilteredHandPosition;

		public bool UseFilteredHandRotation;

		public bool UseSecondStepRotationFiltering;

		protected Quaternion SecondStepFilteredRotation = Quaternion.identity;

		private bool m_isHovered;

		private bool m_isHeld;

		protected Collider[] m_colliders;

		public Transform GrabPointTransform
		{
			get
			{
				return m_grabPointTransform;
			}
			set
			{
				m_grabPointTransform = value;
			}
		}

		[HideInInspector]
		public Vector3 m_handPos
		{
			get
			{
				if (UseFilteredHandTransform || UseFilteredHandPosition)
				{
					return m_hand.Input.FilteredPos;
				}
				return m_hand.Input.Pos;
			}
		}

		[HideInInspector]
		public Quaternion m_handRot
		{
			get
			{
				if (UseFilteredHandTransform || UseFilteredHandRotation)
				{
					return m_hand.Input.FilteredRot;
				}
				return m_hand.Input.Rot;
			}
		}

		[HideInInspector]
		public Vector3 m_palmPos
		{
			get
			{
				if (UseFilteredHandTransform || UseFilteredHandPosition)
				{
					return m_hand.Input.FilteredPalmPos;
				}
				return m_hand.Input.PalmPos;
			}
		}

		[HideInInspector]
		public Quaternion m_palmRot
		{
			get
			{
				if (UseFilteredHandTransform || UseFilteredHandRotation)
				{
					return m_hand.Input.FilteredPalmRot;
				}
				return m_hand.Input.PalmRot;
			}
		}

		public bool IsHovered
		{
			get
			{
				return m_isHovered;
			}
			set
			{
				bool isHovered = m_isHovered;
				if (value)
				{
					SetUXHoverGeoViz(b: true);
				}
				else
				{
					SetUXHoverGeoViz(b: false);
				}
				m_isHovered = value;
				if (m_isHovered && !isHovered)
				{
					OnHoverStart();
				}
				else if (m_isHovered && isHovered)
				{
					OnHoverEnd();
				}
			}
		}

		public bool IsHeld
		{
			get
			{
				return m_isHeld;
			}
			set
			{
				bool isHeld = m_isHeld;
				if (value)
				{
					SetUXHeldGeoViz(b: true);
				}
				else
				{
					SetUXHeldGeoViz(b: false);
				}
				m_isHeld = value;
				if (!isHeld && m_isHeld && this is FVRPhysicalObject && (this as FVRPhysicalObject).ObjectWrapper != null)
				{
					GM.CurrentSceneSettings.OnFVRObjectPickedUp(this as FVRPhysicalObject);
				}
			}
		}

		public virtual bool IsSelectionRestricted()
		{
			return false;
		}

		public virtual bool IsInteractable()
		{
			return true;
		}

		public virtual bool IsDistantGrabbable()
		{
			return false;
		}

		protected virtual void OnHoverStart()
		{
		}

		protected virtual void OnHoverEnd()
		{
		}

		protected virtual void OnHoverStay()
		{
		}

		protected virtual void SetUXHoverGeoViz(bool b)
		{
			if (UXGeo_Hover != null)
			{
				UXGeo_Hover.SetActive(b);
			}
		}

		protected virtual void SetUXHeldGeoViz(bool b)
		{
			if (UXGeo_Held != null)
			{
				UXGeo_Held.SetActive(b);
			}
		}

		public void UpdateGrabPointTransform()
		{
			if (m_hand != null)
			{
				m_grabPointTransform.position = m_handPos;
				m_grabPointTransform.rotation = m_handRot;
			}
		}

		public virtual void PlayGrabSound(bool isHard, FVRViveHand hand)
		{
			if (HandlingGrabSound != 0 && hand.CanMakeGrabReleaseSound)
			{
				SM.PlayHandlingGrabSound(HandlingGrabSound, hand.Input.Pos, isHard);
				hand.HandMadeGrabReleaseSound();
			}
		}

		public virtual void PlayReleaseSound(FVRViveHand hand)
		{
			if (HandlingReleaseSound != 0 && hand.CanMakeGrabReleaseSound)
			{
				SM.PlayHandlingReleaseSound(HandlingReleaseSound, hand.Input.Pos);
				hand.HandMadeGrabReleaseSound();
			}
		}

		public virtual void BeginInteraction(FVRViveHand hand)
		{
			PlayGrabSound(!IsHeld, hand);
			if (IsHeld && m_hand != hand && m_hand != null)
			{
				m_hand.EndInteractionIfHeld(this);
			}
			m_hasTriggeredUpSinceBegin = false;
			IsHeld = true;
			m_hand = hand;
			triggerCooldown = 0.5f;
			if (m_grabPointTransform == null)
			{
				m_grabPointTransform = new GameObject("interpRot").transform;
			}
			m_grabPointTransform.SetParent(Transform);
			m_grabPointTransform.position = m_handPos;
			m_grabPointTransform.rotation = m_handRot;
			m_pos_interp_tick = 0f;
			m_rot_interp_tick = 0f;
		}

		public virtual void UpdateInteraction(FVRViveHand hand)
		{
			IsHeld = true;
			m_hand = hand;
			if (!m_hasTriggeredUpSinceBegin && m_hand.Input.TriggerFloat < 0.15f)
			{
				m_hasTriggeredUpSinceBegin = true;
			}
			if (triggerCooldown > 0f)
			{
				triggerCooldown -= Time.deltaTime;
			}
		}

		public virtual void EndInteraction(FVRViveHand hand)
		{
			m_hasTriggeredUpSinceBegin = false;
			m_hand = null;
			IsHeld = false;
			PlayReleaseSound(hand);
		}

		public virtual void SimpleInteraction(FVRViveHand hand)
		{
		}

		public virtual void Poke(FVRViveHand hand)
		{
		}

		public virtual void ForceBreakInteraction()
		{
			if (m_hand != null)
			{
				m_hand.EndInteractionIfHeld(this);
				EndInteraction(m_hand);
			}
		}

		protected virtual void Awake()
		{
			m_index = All.Count;
			All.Add(this);
			GameObject = base.gameObject;
			Transform = base.transform;
			m_colliders = GetComponentsInChildren<Collider>(includeInactive: true);
		}

		protected virtual void Start()
		{
		}

		public static void GlobalUpdate()
		{
			for (int i = 0; i < All.Count; i++)
			{
				if (All[i] != null && All[i].enabled && All[i].GameObject.activeInHierarchy)
				{
					All[i].FVRUpdate();
				}
			}
		}

		protected virtual void FVRUpdate()
		{
			if (IsHovered)
			{
				OnHoverStay();
			}
		}

		public static void GlobalFixedUpdate()
		{
			for (int i = 0; i < All.Count; i++)
			{
				if (All[i] != null && All[i].enabled && All[i].GameObject.activeInHierarchy)
				{
					All[i].FVRFixedUpdate();
				}
			}
		}

		protected virtual void FVRFixedUpdate()
		{
			if (IsHeld)
			{
				TestHandDistance();
			}
		}

		public virtual void OnDestroy()
		{
			if (IsHeld && m_hand != null)
			{
				m_hand.ForceSetInteractable(null);
			}
			if (m_index >= 0)
			{
				All[m_index] = All[All.Count - 1];
				All[m_index].m_index = m_index;
				All.RemoveAt(All.Count - 1);
			}
		}

		public virtual void TestHandDistance()
		{
			if (!EndInteractionIfDistant)
			{
				return;
			}
			if (PoseOverride == null)
			{
				if (Vector3.Distance(m_handPos, Transform.position) >= EndInteractionDistance)
				{
					ForceBreakInteraction();
				}
				return;
			}
			float num = Vector3.Distance(m_handPos, PoseOverride.position);
			if (num >= EndInteractionDistance)
			{
				ForceBreakInteraction();
			}
		}

		public void SetCollidersToLayer(List<Collider> cols, bool triggersToo, string layerName)
		{
			if (triggersToo)
			{
				foreach (Collider col in cols)
				{
					if (col != null)
					{
						col.gameObject.layer = LayerMask.NameToLayer(layerName);
					}
				}
				return;
			}
			foreach (Collider col2 in cols)
			{
				if (col2 != null && !col2.isTrigger)
				{
					col2.gameObject.layer = LayerMask.NameToLayer(layerName);
				}
			}
		}

		public void SetAllCollidersToLayer(bool triggersToo, string layerName)
		{
			if (triggersToo)
			{
				Collider[] colliders = m_colliders;
				foreach (Collider collider in colliders)
				{
					if (collider != null)
					{
						collider.gameObject.layer = LayerMask.NameToLayer(layerName);
					}
				}
				return;
			}
			Collider[] colliders2 = m_colliders;
			foreach (Collider collider2 in colliders2)
			{
				if (collider2 != null && !collider2.isTrigger)
				{
					collider2.gameObject.layer = LayerMask.NameToLayer(layerName);
				}
			}
		}

		public Vector3 GetClosestValidPoint(Vector3 vA, Vector3 vB, Vector3 vPoint)
		{
			Vector3 rhs = vPoint - vA;
			Vector3 normalized = (vB - vA).normalized;
			float num = Vector3.Distance(vA, vB);
			float num2 = Vector3.Dot(normalized, rhs);
			if (num2 <= 0f)
			{
				return vA;
			}
			if (num2 >= num)
			{
				return vB;
			}
			Vector3 vector = normalized * num2;
			return vA + vector;
		}
	}
}
