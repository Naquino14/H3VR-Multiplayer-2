// Decompiled with JetBrains decompiler
// Type: GrenadeSkeeballTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class GrenadeSkeeballTarget : MonoBehaviour
{
  public float m_score_y;
  private bool m_isDead;
  private GrenadeSkeeballGame game;
  public int Points;

  public void SetGame(GrenadeSkeeballGame g) => this.game = g;

  private void Update()
  {
    if (this.m_isDead || (double) this.transform.position.y >= (double) this.m_score_y)
      return;
    this.m_isDead = true;
    this.game.AddPoints(this.Points);
    if (this.Points == 500)
      this.game.SpawnPointsFX(this.transform.position, GrenadeSkeeballGame.PointsParticleType.Blue);
    else if (this.Points == 1000)
      this.game.SpawnPointsFX(this.transform.position, GrenadeSkeeballGame.PointsParticleType.Yellow);
    else
      this.game.SpawnPointsFX(this.transform.position, GrenadeSkeeballGame.PointsParticleType.Red);
    this.Invoke("Die", 2f);
  }

  private void Die() => Object.Destroy((Object) this.gameObject);
}
