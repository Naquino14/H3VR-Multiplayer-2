// Decompiled with JetBrains decompiler
// Type: FistVR.FVRReverbSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRReverbSystem : MonoBehaviour
  {
    public List<FVRReverbEnvironment> Environments;
    public int NumToCheckAFrame = 10;
    private bool hasEnvironmentsToCheck;
    public FVRReverbEnvironment CurrentReverbEnvironment;
    public FVRReverbEnvironment DefaultEnvironment;
    private int m_lowestPriority = 100;

    public void Awake()
    {
      this.CurrentReverbEnvironment = this.DefaultEnvironment;
      if (this.Environments.Count <= 0)
        return;
      this.hasEnvironmentsToCheck = true;
    }

    public void Start()
    {
      SM.ReverbSystem = this;
      for (int index = 0; index < this.Environments.Count; ++index)
      {
        this.Environments[index].SetPriorityBasedOnType();
        this.m_lowestPriority = Mathf.Min(this.m_lowestPriority, this.Environments[index].Priority);
      }
    }

    public void OnDestroy() => SM.ReverbSystem = (FVRReverbSystem) null;

    public void Update()
    {
      if (!this.hasEnvironmentsToCheck)
        return;
      this.CheckPlayerEnvironment();
    }

    public void CheckPlayerEnvironment()
    {
      this.CurrentReverbEnvironment = this.GetSoundEnvironment(GM.CurrentPlayerBody.Head.position);
      this.SetCurrentReverbSettings(this.CurrentReverbEnvironment.Environment);
    }

    public bool TestVolumeBool(FVRReverbEnvironment e, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = e.transform.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    public FVRReverbEnvironment GetSoundEnvironment(Vector3 pos)
    {
      float num = 100f;
      bool flag = false;
      int index1 = 0;
      for (int index2 = 0; index2 < this.Environments.Count; ++index2)
      {
        if (this.TestVolumeBool(this.Environments[index2], pos) && (double) this.Environments[index2].Priority < (double) num)
        {
          num = (float) this.Environments[index2].Priority;
          flag = true;
          index1 = index2;
          if ((double) num <= (double) this.m_lowestPriority)
            break;
        }
      }
      return flag ? this.Environments[index1] : this.DefaultEnvironment;
    }

    public void SetCurrentReverbSettings(FVRSoundEnvironment e)
    {
      if (GM.CurrentSceneSettings.DefaultSoundEnvironment == e)
        return;
      GM.CurrentSceneSettings.DefaultSoundEnvironment = e;
      SM.TransitionToReverbEnvironment(e, 0.1f);
    }
  }
}
