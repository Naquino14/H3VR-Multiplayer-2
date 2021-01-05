using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_TrackingReferenceManager : MonoBehaviour
	{
		private class TrackingReferenceObject
		{
			public ETrackedDeviceClass trackedDeviceClass;

			public GameObject gameObject;

			public SteamVR_RenderModel renderModel;

			public SteamVR_TrackedObject trackedObject;
		}

		private Dictionary<uint, TrackingReferenceObject> trackingReferences = new Dictionary<uint, TrackingReferenceObject>();

		private void OnEnable()
		{
			SteamVR_Events.NewPoses.AddListener(OnNewPoses);
		}

		private void OnDisable()
		{
			SteamVR_Events.NewPoses.RemoveListener(OnNewPoses);
		}

		private void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			if (poses == null)
			{
				return;
			}
			for (uint num = 0u; num < poses.Length; num++)
			{
				if (!trackingReferences.ContainsKey(num))
				{
					ETrackedDeviceClass trackedDeviceClass = OpenVR.System.GetTrackedDeviceClass(num);
					if (trackedDeviceClass == ETrackedDeviceClass.TrackingReference)
					{
						TrackingReferenceObject trackingReferenceObject = new TrackingReferenceObject();
						trackingReferenceObject.trackedDeviceClass = trackedDeviceClass;
						trackingReferenceObject.gameObject = new GameObject("Tracking Reference " + num);
						trackingReferenceObject.gameObject.transform.parent = base.transform;
						trackingReferenceObject.trackedObject = trackingReferenceObject.gameObject.AddComponent<SteamVR_TrackedObject>();
						trackingReferenceObject.renderModel = trackingReferenceObject.gameObject.AddComponent<SteamVR_RenderModel>();
						trackingReferenceObject.renderModel.createComponents = false;
						trackingReferenceObject.renderModel.updateDynamically = false;
						trackingReferences.Add(num, trackingReferenceObject);
						trackingReferenceObject.gameObject.SendMessage("SetDeviceIndex", (int)num, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						trackingReferences.Add(num, new TrackingReferenceObject
						{
							trackedDeviceClass = trackedDeviceClass
						});
					}
				}
			}
		}
	}
}
