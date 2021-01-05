// Decompiled with JetBrains decompiler
// Type: FistVR.SosigLinkWeakSpot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SosigLinkWeakSpot : MonoBehaviour, IFVRDamageable
  {
    public SosigLink L;
    public bool RandomPlace = true;
    public float Radius;
    public float HeightRange;

    public void Awake()
    {
      if (!this.RandomPlace)
        return;
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      onUnitSphere.Normalize();
      onUnitSphere *= this.Radius;
      this.transform.localPosition = new Vector3(onUnitSphere.x, Random.Range(-this.HeightRange, this.HeightRange), onUnitSphere.z);
    }

    public void Damage(FistVR.Damage D)
    {
      if (D.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.L.LinkExplodes(D.Class);
    }
  }
}
