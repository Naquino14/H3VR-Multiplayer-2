// Decompiled with JetBrains decompiler
// Type: DinoFracture.DisableObjectsOnFracture
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture
{
  public class DisableObjectsOnFracture : MonoBehaviour
  {
    public GameObject[] ObjectsToDisable;

    private void OnFracture(OnFractureEventArgs e)
    {
      for (int index = 0; index < this.ObjectsToDisable.Length; ++index)
      {
        if ((Object) this.ObjectsToDisable[index] != (Object) null)
          this.ObjectsToDisable[index].SetActive(false);
      }
    }
  }
}
