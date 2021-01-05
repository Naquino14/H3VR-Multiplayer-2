// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019Battery
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019Battery : FVRPhysicalObject
  {
    private float m_energy = 1f;
    public Renderer LED_FauxBattery_Side;
    public Renderer LED_FauxBattery_Under;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Emissive_Red;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Emissive_Green;

    public void SetEnergy(float f) => this.m_energy = f;

    public float GetEnergy() => this.m_energy;

    private void Update() => this.LED_FauxBattery_Side.material.SetColor("_Color", Color.Lerp(this.Color_Emissive_Red, this.Color_Emissive_Green, this.m_energy));

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      gameObject.GetComponent<LAPD2019Battery>().SetEnergy(this.m_energy);
      return gameObject;
    }
  }
}
