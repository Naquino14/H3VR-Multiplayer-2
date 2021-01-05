// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearDisplacement
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class LinearDisplacement : MonoBehaviour
  {
    public Vector3 displacement;
    public LinearMapping linearMapping;
    private Vector3 initialPosition;

    private void Start()
    {
      this.initialPosition = this.transform.localPosition;
      if (!((Object) this.linearMapping == (Object) null))
        return;
      this.linearMapping = this.GetComponent<LinearMapping>();
    }

    private void Update()
    {
      if (!(bool) (Object) this.linearMapping)
        return;
      this.transform.localPosition = this.initialPosition + this.linearMapping.value * this.displacement;
    }
  }
}
