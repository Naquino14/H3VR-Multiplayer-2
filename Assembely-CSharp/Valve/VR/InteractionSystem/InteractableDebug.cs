using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class InteractableDebug : MonoBehaviour
	{
		[NonSerialized]
		public Hand attachedToHand;

		public float simulateReleasesForXSecondsAroundRelease;

		public float simulateReleasesEveryXSeconds = 0.005f;

		public bool setPositionsForSimulations;

		private Renderer[] selfRenderers;

		private Collider[] colliders;

		private Color lastColor;

		private Throwable throwable;

		private const bool onlyColorOnChange = true;

		public Rigidbody rigidbody;

		private bool isSimulation;

		private bool isThrowable => throwable != null;

		private void Awake()
		{
			selfRenderers = GetComponentsInChildren<Renderer>();
			throwable = GetComponent<Throwable>();
			rigidbody = GetComponent<Rigidbody>();
			colliders = GetComponentsInChildren<Collider>();
		}

		private void OnAttachedToHand(Hand hand)
		{
			attachedToHand = hand;
			CreateMarker(Color.green);
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			Color color = hand.currentAttachedObjectInfo.Value.grabbedWithType switch
			{
				GrabTypes.Grip => Color.blue, 
				GrabTypes.Pinch => Color.green, 
				GrabTypes.Trigger => Color.yellow, 
				GrabTypes.Scripted => Color.red, 
				_ => Color.white, 
			};
			if ((true && color != lastColor) ? true : false)
			{
				ColorSelf(color);
			}
			lastColor = color;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			if (isThrowable)
			{
				throwable.GetReleaseVelocities(hand, out var velocity, out var _);
				CreateMarker(Color.cyan, velocity.normalized);
			}
			CreateMarker(Color.red);
			attachedToHand = null;
			if (isSimulation || simulateReleasesForXSecondsAroundRelease == 0f)
			{
				return;
			}
			float num = 0f - simulateReleasesForXSecondsAroundRelease;
			float num2 = simulateReleasesForXSecondsAroundRelease;
			List<InteractableDebug> list = new List<InteractableDebug>();
			list.Add(this);
			for (float num3 = num; num3 <= num2; num3 += simulateReleasesEveryXSeconds)
			{
				float t = Mathf.InverseLerp(num, num2, num3);
				InteractableDebug item = CreateSimulation(hand, num3, Color.Lerp(Color.red, Color.green, t));
				list.Add(item);
			}
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					list[i].IgnoreObject(list[j]);
				}
			}
		}

		public Collider[] GetColliders()
		{
			return colliders;
		}

		public void IgnoreObject(InteractableDebug otherInteractable)
		{
			Collider[] array = otherInteractable.GetColliders();
			for (int i = 0; i < colliders.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					Physics.IgnoreCollision(colliders[i], array[j]);
				}
			}
		}

		public void SetIsSimulation()
		{
			isSimulation = true;
		}

		private InteractableDebug CreateSimulation(Hand fromHand, float timeOffset, Color copyColor)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
			InteractableDebug component = gameObject.GetComponent<InteractableDebug>();
			component.SetIsSimulation();
			component.ColorSelf(copyColor);
			gameObject.name = $"{gameObject.name} [offset: {timeOffset:0.000}]";
			Vector3 trackedObjectVelocity = fromHand.GetTrackedObjectVelocity(timeOffset);
			trackedObjectVelocity *= throwable.scaleReleaseVelocity;
			component.rigidbody.velocity = trackedObjectVelocity;
			return component;
		}

		private void CreateMarker(Color markerColor, float destroyAfter = 10f)
		{
			CreateMarker(markerColor, attachedToHand.GetTrackedObjectVelocity().normalized, destroyAfter);
		}

		private void CreateMarker(Color markerColor, Vector3 forward, float destroyAfter = 10f)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Collider>());
			gameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.transform.localScale = new Vector3(0.01f, 0.01f, 0.25f);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, gameObject2.transform.localScale.z / 2f);
			gameObject.transform.position = attachedToHand.transform.position;
			gameObject.transform.forward = forward;
			ColorThing(markerColor, gameObject.GetComponentsInChildren<Renderer>());
			if (destroyAfter > 0f)
			{
				UnityEngine.Object.Destroy(gameObject, destroyAfter);
			}
		}

		private void ColorSelf(Color newColor)
		{
			ColorThing(newColor, selfRenderers);
		}

		private void ColorThing(Color newColor, Renderer[] renderers)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = newColor;
			}
		}
	}
}
