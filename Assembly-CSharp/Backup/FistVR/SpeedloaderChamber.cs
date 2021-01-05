// Decompiled with JetBrains decompiler
// Type: FistVR.SpeedloaderChamber
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SpeedloaderChamber : MonoBehaviour
  {
    public Speedloader SpeedLoader;
    public FireArmRoundType Type;
    public FireArmRoundClass LoadedClass;
    public MeshFilter Filter;
    public Renderer LoadedRenderer;
    public bool IsLoaded = true;
    public bool IsSpent;

    private void Awake()
    {
    }

    public void Load(FireArmRoundClass rclass, bool playSound = false)
    {
      this.IsLoaded = true;
      this.IsSpent = false;
      this.LoadedClass = rclass;
      this.Filter.mesh = AM.GetRoundMesh(this.Type, this.LoadedClass);
      this.LoadedRenderer.material = AM.GetRoundMaterial(this.Type, this.LoadedClass);
      this.LoadedRenderer.enabled = true;
      if (!playSound || !((Object) this.SpeedLoader.ProfileOverride != (Object) null))
        return;
      SM.PlayGenericSound(this.SpeedLoader.ProfileOverride.MagazineInsertRound, this.transform.position);
    }

    public void LoadEmpty(FireArmRoundClass rclass, bool playSound = false)
    {
      this.IsLoaded = true;
      this.IsSpent = true;
      this.LoadedClass = rclass;
      this.Filter.mesh = AM.GetRoundSelfPrefab(this.Type, this.LoadedClass).GetGameObject().GetComponent<FVRFireArmRound>().FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
      this.LoadedRenderer.material = AM.GetRoundMaterial(this.Type, this.LoadedClass);
      this.LoadedRenderer.enabled = true;
      if (!playSound || !((Object) this.SpeedLoader.ProfileOverride != (Object) null))
        return;
      SM.PlayGenericSound(this.SpeedLoader.ProfileOverride.MagazineInsertRound, this.transform.position);
    }

    public FireArmRoundClass Unload()
    {
      this.IsLoaded = false;
      this.LoadedRenderer.enabled = false;
      return this.LoadedClass;
    }
  }
}
