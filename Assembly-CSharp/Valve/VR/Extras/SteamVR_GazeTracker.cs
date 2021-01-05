// Decompiled with JetBrains decompiler
// Type: Valve.VR.Extras.SteamVR_GazeTracker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.Extras
{
  public class SteamVR_GazeTracker : MonoBehaviour
  {
    public bool isInGaze;
    public float gazeInCutoff = 0.15f;
    public float gazeOutCutoff = 0.4f;
    protected Transform hmdTrackedObject;

    public event GazeEventHandler GazeOn;

    public event GazeEventHandler GazeOff;

    public virtual void OnGazeOn(GazeEventArgs gazeEventArgs)
    {
      if (this.GazeOn == null)
        return;
      this.GazeOn((object) this, gazeEventArgs);
    }

    public virtual void OnGazeOff(GazeEventArgs gazeEventArgs)
    {
      if (this.GazeOff == null)
        return;
      this.GazeOff((object) this, gazeEventArgs);
    }

    protected virtual void Update()
    {
      if ((Object) this.hmdTrackedObject == (Object) null)
      {
        foreach (SteamVR_TrackedObject steamVrTrackedObject in Object.FindObjectsOfType<SteamVR_TrackedObject>())
        {
          if (steamVrTrackedObject.index == SteamVR_TrackedObject.EIndex.Hmd)
          {
            this.hmdTrackedObject = steamVrTrackedObject.transform;
            break;
          }
        }
      }
      if (!(bool) (Object) this.hmdTrackedObject)
        return;
      Ray ray = new Ray(this.hmdTrackedObject.position, this.hmdTrackedObject.forward);
      Plane plane = new Plane(this.hmdTrackedObject.forward, this.transform.position);
      float enter = 0.0f;
      if (!plane.Raycast(ray, out enter))
        return;
      float num = Vector3.Distance(this.hmdTrackedObject.position + this.hmdTrackedObject.forward * enter, this.transform.position);
      if ((double) num < (double) this.gazeInCutoff && !this.isInGaze)
      {
        this.isInGaze = true;
        GazeEventArgs gazeEventArgs;
        gazeEventArgs.distance = num;
        this.OnGazeOn(gazeEventArgs);
      }
      else
      {
        if ((double) num < (double) this.gazeOutCutoff || !this.isInGaze)
          return;
        this.isInGaze = false;
        GazeEventArgs gazeEventArgs;
        gazeEventArgs.distance = num;
        this.OnGazeOff(gazeEventArgs);
      }
    }
  }
}
