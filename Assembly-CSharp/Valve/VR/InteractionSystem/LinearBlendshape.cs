// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearBlendshape
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class LinearBlendshape : MonoBehaviour
  {
    public LinearMapping linearMapping;
    public SkinnedMeshRenderer skinnedMesh;
    private float lastValue;

    private void Awake()
    {
      if ((Object) this.skinnedMesh == (Object) null)
        this.skinnedMesh = this.GetComponent<SkinnedMeshRenderer>();
      if (!((Object) this.linearMapping == (Object) null))
        return;
      this.linearMapping = this.GetComponent<LinearMapping>();
    }

    private void Update()
    {
      float num = this.linearMapping.value;
      if ((double) num != (double) this.lastValue)
        this.skinnedMesh.SetBlendShapeWeight(0, Util.RemapNumberClamped(num, 0.0f, 1f, 1f, 100f));
      this.lastValue = num;
    }
  }
}
