// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.JoeJeffGestures
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class JoeJeffGestures : MonoBehaviour
  {
    private const float openFingerAmount = 0.1f;
    private const float closedFingerAmount = 0.9f;
    private const float closedThumbAmount = 0.4f;
    private JoeJeff joeJeff;
    private bool lastPeaceSignState;

    private void Awake() => this.joeJeff = this.GetComponent<JoeJeff>();

    private void Update()
    {
      if ((Object) Player.instance == (Object) null)
        return;
      Transform transform = Camera.main.transform;
      if ((double) Vector3.Angle(transform.forward, this.transform.position - transform.position) >= 90.0)
        return;
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        if ((Object) Player.instance.hands[index] != (Object) null)
        {
          SteamVR_Behaviour_Skeleton skeleton = Player.instance.hands[index].skeleton;
          if ((Object) skeleton != (Object) null)
          {
            if ((double) skeleton.indexCurl <= 0.100000001490116 && (double) skeleton.middleCurl <= 0.100000001490116 && ((double) skeleton.thumbCurl >= 0.400000005960464 && (double) skeleton.ringCurl >= 0.899999976158142) && (double) skeleton.pinkyCurl >= 0.899999976158142)
              this.PeaceSignRecognized(true);
            else
              this.PeaceSignRecognized(false);
          }
        }
      }
    }

    private void PeaceSignRecognized(bool currentPeaceSignState)
    {
      if (!this.lastPeaceSignState && currentPeaceSignState)
        this.joeJeff.Jump();
      this.lastPeaceSignState = currentPeaceSignState;
    }
  }
}
