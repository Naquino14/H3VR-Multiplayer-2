// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SpawnAndAttachToHand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SpawnAndAttachToHand : MonoBehaviour
  {
    public Hand hand;
    public GameObject prefab;

    public void SpawnAndAttach(Hand passedInhand)
    {
      Hand hand = passedInhand;
      if ((Object) passedInhand == (Object) null)
        hand = this.hand;
      if ((Object) hand == (Object) null)
        return;
      GameObject objectToAttach = Object.Instantiate<GameObject>(this.prefab);
      hand.AttachObject(objectToAttach, GrabTypes.Scripted);
    }
  }
}
