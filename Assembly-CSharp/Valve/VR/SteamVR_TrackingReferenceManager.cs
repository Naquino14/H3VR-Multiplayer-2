// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_TrackingReferenceManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  public class SteamVR_TrackingReferenceManager : MonoBehaviour
  {
    private Dictionary<uint, SteamVR_TrackingReferenceManager.TrackingReferenceObject> trackingReferences = new Dictionary<uint, SteamVR_TrackingReferenceManager.TrackingReferenceObject>();

    private void OnEnable() => SteamVR_Events.NewPoses.AddListener(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));

    private void OnDisable() => SteamVR_Events.NewPoses.RemoveListener(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));

    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
      if (poses == null)
        return;
      for (uint index = 0; (long) index < (long) poses.Length; ++index)
      {
        if (!this.trackingReferences.ContainsKey(index))
        {
          ETrackedDeviceClass trackedDeviceClass = OpenVR.System.GetTrackedDeviceClass(index);
          if (trackedDeviceClass == ETrackedDeviceClass.TrackingReference)
          {
            SteamVR_TrackingReferenceManager.TrackingReferenceObject trackingReferenceObject = new SteamVR_TrackingReferenceManager.TrackingReferenceObject()
            {
              trackedDeviceClass = trackedDeviceClass,
              gameObject = new GameObject("Tracking Reference " + index.ToString())
            };
            trackingReferenceObject.gameObject.transform.parent = this.transform;
            trackingReferenceObject.trackedObject = trackingReferenceObject.gameObject.AddComponent<SteamVR_TrackedObject>();
            trackingReferenceObject.renderModel = trackingReferenceObject.gameObject.AddComponent<SteamVR_RenderModel>();
            trackingReferenceObject.renderModel.createComponents = false;
            trackingReferenceObject.renderModel.updateDynamically = false;
            this.trackingReferences.Add(index, trackingReferenceObject);
            trackingReferenceObject.gameObject.SendMessage("SetDeviceIndex", (object) (int) index, SendMessageOptions.DontRequireReceiver);
          }
          else
            this.trackingReferences.Add(index, new SteamVR_TrackingReferenceManager.TrackingReferenceObject()
            {
              trackedDeviceClass = trackedDeviceClass
            });
        }
      }
    }

    private class TrackingReferenceObject
    {
      public ETrackedDeviceClass trackedDeviceClass;
      public GameObject gameObject;
      public SteamVR_RenderModel renderModel;
      public SteamVR_TrackedObject trackedObject;
    }
  }
}
