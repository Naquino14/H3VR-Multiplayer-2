// Decompiled with JetBrains decompiler
// Type: FistVR.MR_PumpBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MR_PumpBase : MonoBehaviour, IMG_HandlePumpable
  {
    public MR_SpikePuzzle Puzzle;
    public Transform Spike;
    public float Extension = 0.731f;
    public bool IsGoof;
    public Transform SpawnPos;
    public GameObject GoofPrefab;
    private bool hasGoofed;

    private void Update()
    {
      if (!((Object) this.Spike != (Object) null))
        return;
      this.Extension = this.Spike.transform.localPosition.z;
    }

    public void Pump(float delta)
    {
      if (this.IsGoof)
      {
        if (this.hasGoofed)
          return;
        this.hasGoofed = true;
        Object.Instantiate<GameObject>(this.GoofPrefab, this.SpawnPos.position, this.SpawnPos.rotation);
        GM.MGMaster.Narrator.PlayJumpScare();
      }
      else
      {
        this.Extension += delta * 0.01f;
        this.Extension = Mathf.Clamp(this.Extension, -0.42f, 0.9f);
        this.Spike.transform.localPosition = new Vector3(0.0f, 0.0f, this.Extension);
        this.Puzzle.UpdatePuzzle();
      }
    }
  }
}
