// Decompiled with JetBrains decompiler
// Type: DinoFracture.NotifyOnFracture
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public class NotifyOnFracture : MonoBehaviour
  {
    public GameObject[] GameObjects = new GameObject[1];

    private void OnFracture(OnFractureEventArgs args)
    {
      if (!((Object) args.OriginalObject.gameObject == (Object) this.gameObject))
        return;
      for (int index = 0; index < this.GameObjects.Length; ++index)
      {
        if ((Object) this.GameObjects[index] != (Object) null)
          this.GameObjects[index].SendMessage(nameof (OnFracture), (object) args, SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}
