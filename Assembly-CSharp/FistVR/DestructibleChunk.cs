// Decompiled with JetBrains decompiler
// Type: FistVR.DestructibleChunk
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DestructibleChunk : MonoBehaviour, IFVRDamageable
  {
    public DestructibleChunkProfile Profile;
    public MeshFilter MeshFilter;
    public MeshCollider MeshCollider;
    private int m_chunkIndex;
    private bool m_scalesSpawns;
    private int m_currentDestructionRenderer;
    private int m_maxDestructionRenderer;
    private float m_currentLife;
    private float m_startingLife;
    private float m_damageCutoff;
    private bool m_isDestroyed;

    private void Awake() => this.Configure();

    private void Configure()
    {
      this.m_currentLife = this.Profile.TotalLife;
      this.m_startingLife = this.Profile.TotalLife;
      this.m_damageCutoff = this.Profile.DamageCutoff;
      this.m_chunkIndex = Random.Range(0, this.Profile.MaxRandomIndex + 1);
      this.m_currentDestructionRenderer = 0;
      this.m_maxDestructionRenderer = this.Profile.DGeoStages.Count - 1;
      this.m_scalesSpawns = this.Profile.ScalesSpawns;
    }

    public void Damage(FistVR.Damage d)
    {
      this.m_currentLife -= Mathf.Clamp(d.Dam_TotalKinetic - Mathf.Abs(this.m_damageCutoff), 0.0f, d.Dam_TotalKinetic);
      this.UpdateDestruction();
    }

    private void UpdateDestruction()
    {
      if (this.m_isDestroyed)
        return;
      bool flag = false;
      if ((double) this.m_currentLife <= 0.0)
      {
        this.m_isDestroyed = true;
        if (this.Profile.IsDestroyedOnZeroLife)
          flag = true;
      }
      if (this.m_isDestroyed && this.Profile.SpawnsOnDestruction)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.Profile.SpawnOnDestruction, this.transform.position, this.transform.rotation);
        if (this.m_scalesSpawns)
        {
          float num = gameObject.transform.localScale.x * this.transform.localScale.x;
          gameObject.transform.localScale = new Vector3(num, num, num);
        }
      }
      if (flag)
      {
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        int num1 = Mathf.Clamp(Mathf.RoundToInt((float) (1.0 - (double) this.m_currentLife / (double) this.m_startingLife) * (float) this.m_maxDestructionRenderer), 0, this.m_maxDestructionRenderer);
        if (this.m_currentDestructionRenderer != num1)
        {
          this.m_currentDestructionRenderer = num1;
          this.MeshFilter.mesh = this.Profile.DGeoStages[this.m_currentDestructionRenderer].GetMesh(this.m_chunkIndex);
          this.MeshCollider.sharedMesh = this.Profile.DGeoStages[this.m_currentDestructionRenderer].GetMesh(this.m_chunkIndex);
          if (this.Profile.DGeoStages[this.m_currentDestructionRenderer].SpawnsOnEnterIndex)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.Profile.DGeoStages[this.m_currentDestructionRenderer].SpawnOnEnterIndex, this.transform.position, this.transform.rotation);
            if (this.m_scalesSpawns)
            {
              float num2 = gameObject.transform.localScale.x * this.transform.localScale.x;
              gameObject.transform.localScale = new Vector3(num2, num2, num2);
            }
          }
        }
        if (!this.m_isDestroyed || !this.Profile.UsesFinalMesh)
          return;
        this.MeshFilter.mesh = this.Profile.FinalMesh;
        this.MeshCollider.sharedMesh = this.Profile.FinalMesh;
      }
    }
  }
}
