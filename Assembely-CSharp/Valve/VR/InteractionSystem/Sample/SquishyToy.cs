using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class SquishyToy : MonoBehaviour
	{
		public Interactable interactable;

		public SkinnedMeshRenderer renderer;

		public bool affectMaterial = true;

		public SteamVR_Action_Single gripSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");

		public SteamVR_Action_Single pinchSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");

		private Rigidbody rigidbody;

		private void Start()
		{
			if (rigidbody == null)
			{
				rigidbody = GetComponent<Rigidbody>();
			}
			if (interactable == null)
			{
				interactable = GetComponent<Interactable>();
			}
			if (renderer == null)
			{
				renderer = GetComponent<SkinnedMeshRenderer>();
			}
		}

		private void Update()
		{
			float num = 0f;
			float num2 = 0f;
			if ((bool)interactable.attachedToHand)
			{
				num = gripSqueeze.GetAxis(interactable.attachedToHand.handType);
				num2 = pinchSqueeze.GetAxis(interactable.attachedToHand.handType);
			}
			renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), num * 100f, Time.deltaTime * 10f));
			if (renderer.sharedMesh.blendShapeCount > 1)
			{
				renderer.SetBlendShapeWeight(1, Mathf.Lerp(renderer.GetBlendShapeWeight(1), num2 * 100f, Time.deltaTime * 10f));
			}
			if (affectMaterial)
			{
				renderer.material.SetFloat("_Deform", Mathf.Pow(num * 1f, 0.5f));
				if (renderer.material.HasProperty("_PinchDeform"))
				{
					renderer.material.SetFloat("_PinchDeform", Mathf.Pow(num2 * 1f, 0.5f));
				}
			}
		}
	}
}
