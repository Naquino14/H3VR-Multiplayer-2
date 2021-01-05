using UnityEngine;

namespace Valve.VR.Extras
{
	[RequireComponent(typeof(SteamVR_TrackedObject))]
	public class SteamVR_TestThrow : MonoBehaviour
	{
		public GameObject prefab;

		public Rigidbody attachPoint;

		public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

		private SteamVR_Behaviour_Pose trackedObj;

		private FixedJoint joint;

		private void Awake()
		{
			trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
		}

		private void FixedUpdate()
		{
			if (joint == null && spawn.GetStateDown(trackedObj.inputSource))
			{
				GameObject gameObject = Object.Instantiate(prefab);
				gameObject.transform.position = attachPoint.transform.position;
				joint = gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = attachPoint;
			}
			else if (joint != null && spawn.GetStateUp(trackedObj.inputSource))
			{
				GameObject gameObject2 = joint.gameObject;
				Rigidbody component = gameObject2.GetComponent<Rigidbody>();
				Object.DestroyImmediate(joint);
				joint = null;
				Object.Destroy(gameObject2, 15f);
				Transform transform = ((!trackedObj.origin) ? trackedObj.transform.parent : trackedObj.origin);
				if (transform != null)
				{
					component.velocity = transform.TransformVector(trackedObj.GetVelocity());
					component.angularVelocity = transform.TransformVector(trackedObj.GetAngularVelocity());
				}
				else
				{
					component.velocity = trackedObj.GetVelocity();
					component.angularVelocity = trackedObj.GetAngularVelocity();
				}
				component.maxAngularVelocity = component.angularVelocity.magnitude;
			}
		}
	}
}
