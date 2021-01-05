// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_Grid
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_Grid : OmniSpawner
  {
    private OmniSpawnDef_Grid m_def;
    public GameObject[] GridPrefabs;
    private bool m_canPresent;
    private bool m_hasGrid;
    private OmniGrid m_curGrid;

    public override void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
    {
      base.Configure(Def, Range);
      this.m_def = Def as OmniSpawnDef_Grid;
    }

    public override void BeginSpawning()
    {
      base.BeginSpawning();
      this.m_canPresent = true;
    }

    public override void EndSpawning()
    {
      base.EndSpawning();
      this.m_canPresent = false;
    }

    public override void Activate() => base.Activate();

    public override int Deactivate()
    {
      this.m_curGrid.DespawnGrid();
      return base.Deactivate();
    }

    private void Update() => this.UpdateMe();

    private void UpdateMe()
    {
      if (!this.m_isConfigured)
        return;
      switch (this.m_state)
      {
        case OmniSpawner.SpawnerState.Deactivating:
          this.Deactivating();
          break;
        case OmniSpawner.SpawnerState.Activated:
          this.SpawningLoop();
          break;
        case OmniSpawner.SpawnerState.Activating:
          this.Activating();
          break;
      }
    }

    private void SpawningLoop()
    {
      if (!this.m_canPresent || this.m_hasGrid)
        return;
      this.m_hasGrid = true;
      Vector3 position = this.transform.position;
      Vector3 endPos = new Vector3(0.0f, 1.25f, this.GetRange());
      this.m_curGrid = Object.Instantiate<GameObject>(this.GridPrefabs[(int) this.m_def.Size], position, Quaternion.identity).GetComponent<OmniGrid>();
      this.m_curGrid.Init(this, position, endPos, Quaternion.identity, Quaternion.identity, this.m_def.Configuration, this.m_def.Instructions);
      this.m_isDoneSpawning = true;
    }

    public void GridIsFinished()
    {
      this.m_isReadyForWaveEnd = true;
      this.Deactivate();
    }
  }
}
