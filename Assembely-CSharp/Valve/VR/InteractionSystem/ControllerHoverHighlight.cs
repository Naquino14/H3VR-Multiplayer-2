using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ControllerHoverHighlight : MonoBehaviour
	{
		public Material highLightMaterial;

		public bool fireHapticsOnHightlight = true;

		protected Hand hand;

		protected RenderModel renderModel;

		protected SteamVR_Events.Action renderModelLoadedAction;

		protected void Awake()
		{
			hand = GetComponentInParent<Hand>();
		}

		protected void OnHandInitialized(int deviceIndex)
		{
			GameObject gameObject = Object.Instantiate(hand.renderModelPrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = hand.renderModelPrefab.transform.localScale;
			renderModel = gameObject.GetComponent<RenderModel>();
			renderModel.SetInputSource(hand.handType);
			renderModel.OnHandInitialized(deviceIndex);
			renderModel.SetMaterial(highLightMaterial);
			hand.SetHoverRenderModel(renderModel);
			renderModel.onControllerLoaded += RenderModel_onControllerLoaded;
			renderModel.Hide();
		}

		private void RenderModel_onControllerLoaded()
		{
			renderModel.Hide();
		}

		protected void OnParentHandHoverBegin(Interactable other)
		{
			if (base.isActiveAndEnabled && other.transform.parent != base.transform.parent)
			{
				ShowHighlight();
			}
		}

		private void OnParentHandHoverEnd(Interactable other)
		{
			HideHighlight();
		}

		private void OnParentHandInputFocusAcquired()
		{
			if (base.isActiveAndEnabled && (bool)hand.hoveringInteractable && hand.hoveringInteractable.transform.parent != base.transform.parent)
			{
				ShowHighlight();
			}
		}

		private void OnParentHandInputFocusLost()
		{
			HideHighlight();
		}

		public void ShowHighlight()
		{
			if (!(renderModel == null))
			{
				if (fireHapticsOnHightlight)
				{
					hand.TriggerHapticPulse(500);
				}
				renderModel.Show();
			}
		}

		public void HideHighlight()
		{
			if (!(renderModel == null))
			{
				if (fireHapticsOnHightlight)
				{
					hand.TriggerHapticPulse(300);
				}
				renderModel.Hide();
			}
		}
	}
}
