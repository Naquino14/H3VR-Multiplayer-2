// Decompiled with JetBrains decompiler
// Type: FistVR.eSlabDiscSensor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class eSlabDiscSensor : MonoBehaviour
  {
    public eSlabDisc Disc;
    private eSlabSensor m_detectedSensor;

    private void OnTriggerEnter(Collider col) => this.Detect(col);

    private void Detect(Collider col)
    {
      if ((Object) this.Disc == (Object) null || col.gameObject.tag != "ESlabSlot")
        return;
      this.m_detectedSensor = col.gameObject.GetComponent<eSlabSensor>();
    }

    private void Update()
    {
      if (!((Object) this.m_detectedSensor != (Object) null))
        return;
      if ((double) this.m_detectedSensor.eSlab.insertCooldown <= 0.0)
      {
        if (this.m_detectedSensor.eSlab.LoadDisc(this.Disc))
        {
          if ((Object) this.Disc.m_hand != (Object) null)
            this.Disc.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
          Object.Destroy((Object) this.Disc.gameObject);
        }
        else
          this.m_detectedSensor = (eSlabSensor) null;
      }
      else
        this.m_detectedSensor = (eSlabSensor) null;
    }
  }
}
