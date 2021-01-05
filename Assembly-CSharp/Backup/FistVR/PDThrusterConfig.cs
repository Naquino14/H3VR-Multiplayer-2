// Decompiled with JetBrains decompiler
// Type: FistVR.PDThrusterConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New PD Thruster Config", menuName = "PancakeDrone/Thruster Definition", order = 0)]
  public class PDThrusterConfig : ScriptableObject
  {
    public float MaxThrustForce = 10f;
    public float ThrustEngagementResponseSpeedUp = 2f;
    public float ThrustEngagementResponseSpeedDown = 10f;
    public AnimationCurve ThrustResponseCurve;
    public float HeatGenerationBase = 100f;
    public AnimationCurve LoadToHeatCurve;
    public AnimationCurve HeatToMaxLoadCurve;
    public AnimationCurve IntegrityToMaxLoadCurve;
    public AnimationCurve IntegrityToControlFailureCurve;
  }
}
