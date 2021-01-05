using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Throwable))]
	public class Equippable : MonoBehaviour
	{
		[Tooltip("Array of children you do not want to be mirrored. Text, logos, etc.")]
		public Transform[] antiFlip;

		public WhichHand defaultHand = WhichHand.Right;

		private Vector3 initialScale;

		private Interactable interactable;

		[HideInInspector]
		public SteamVR_Input_Sources attachedHandType
		{
			get
			{
				if ((bool)interactable.attachedToHand)
				{
					return interactable.attachedToHand.handType;
				}
				return SteamVR_Input_Sources.Any;
			}
		}

		private void Start()
		{
			initialScale = base.transform.localScale;
			interactable = GetComponent<Interactable>();
		}

		private void Update()
		{
			if (!interactable.attachedToHand)
			{
				return;
			}
			Vector3 localScale = initialScale;
			if ((attachedHandType == SteamVR_Input_Sources.RightHand && defaultHand == WhichHand.Right) || (attachedHandType == SteamVR_Input_Sources.LeftHand && defaultHand == WhichHand.Left))
			{
				localScale.x *= 1f;
				for (int i = 0; i < antiFlip.Length; i++)
				{
					antiFlip[i].localScale = new Vector3(1f, 1f, 1f);
				}
			}
			else
			{
				localScale.x *= -1f;
				for (int j = 0; j < antiFlip.Length; j++)
				{
					antiFlip[j].localScale = new Vector3(-1f, 1f, 1f);
				}
			}
			base.transform.localScale = localScale;
		}
	}
}
