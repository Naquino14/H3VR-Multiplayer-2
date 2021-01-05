// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstDamageablePiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwBotWurstDamageablePiece : MonoBehaviour, IFVRDamageable
  {
    public wwBotWurst Bot;
    public float m_life = 10f;
    private bool m_destroyed;
    public Rigidbody[] Shards;
    public GameObject[] Spawns;
    public Transform SpawnPoint;
    public GameObject[] SpawnsOnlySplode;
    private float m_countDownToExplode = 10f;
    private bool m_isCountingDown;
    private bool m_hasSploded;
    public Transform Child;
    public Joint ParentAttachingJoint;
    public FVRDestroyableObject[] DetachingPieces;
    public bool UsesParams = true;
    public FVRDestroyableObject.DetachRBParams DetachRigidbodyParams;
    public Vector2 DestroyEventTimeRange;
    private bool m_hasDetached;
    private bool m_isHead;

    public void SetIsHead(bool b) => this.m_isHead = true;

    public void SetLife(float i) => this.m_life = i;

    public void StartCountingDown()
    {
      this.m_isCountingDown = true;
      this.m_countDownToExplode = Random.Range(2f, 5f);
    }

    public void Update()
    {
      if (!this.m_isCountingDown)
        return;
      this.m_countDownToExplode -= Time.deltaTime;
      if ((double) this.m_countDownToExplode > 0.0 || this.m_hasSploded)
        return;
      this.m_hasSploded = true;
      this.Splode();
      Object.Destroy((Object) this.gameObject);
    }

    public void Splode()
    {
      if ((Object) this.Child != (Object) null)
        this.Child.SetParent((Transform) null);
      if ((Object) this.ParentAttachingJoint != (Object) null)
        Object.Destroy((Object) this.ParentAttachingJoint);
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
      for (int index = 0; index < this.SpawnsOnlySplode.Length; ++index)
        Object.Instantiate<GameObject>(this.SpawnsOnlySplode[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddForceAtPosition(-Vector3.up * Random.Range(0.1f, 1f), this.transform.position, ForceMode.Impulse);
      }
      if (this.DetachingPieces.Length <= 0 || this.m_hasDetached)
        return;
      this.DetachPieces();
    }

    private void Explode(FistVR.Damage dam)
    {
      if ((Object) this.Child != (Object) null)
        this.Child.SetParent((Transform) null);
      if ((Object) this.ParentAttachingJoint != (Object) null)
        Object.Destroy((Object) this.ParentAttachingJoint);
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddForceAtPosition(dam.strikeDir * Random.Range(0.1f, 1f), dam.point, ForceMode.Impulse);
      }
      if (this.DetachingPieces.Length <= 0 || this.m_hasDetached)
        return;
      this.DetachPieces();
    }

    private void DetachPieces()
    {
      this.m_hasDetached = true;
      for (int index = 0; index < this.DetachingPieces.Length; ++index)
      {
        if (!((Object) this.DetachingPieces[index] == (Object) null))
        {
          this.DetachingPieces[index].transform.SetParent((Transform) null);
          Rigidbody rigidbody = this.DetachingPieces[index].gameObject.AddComponent<Rigidbody>();
          if (this.UsesParams)
          {
            rigidbody.mass = this.DetachRigidbodyParams.Mass;
            rigidbody.drag = this.DetachRigidbodyParams.Drag;
            rigidbody.angularDrag = this.DetachRigidbodyParams.AngularDrag;
          }
          this.DetachingPieces[index].Invoke("DestroyEvent", Random.Range(this.DestroyEventTimeRange.x, this.DestroyEventTimeRange.y));
        }
      }
    }

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_isHead && (Object) this.Bot != (Object) null)
        this.Bot.StunBot(Mathf.Max(0.5f, dam.Dam_Stunning));
      if (this.m_destroyed)
        return;
      this.m_life -= 0.0f + dam.Dam_Blunt + dam.Dam_Piercing * 1f + dam.Dam_Cutting * 1.5f;
      if ((double) this.m_life <= 0.0)
      {
        if ((Object) this.Bot != (Object) null)
          this.Bot.RegisterHit(dam, true);
        this.m_destroyed = true;
        this.Explode(dam);
        if ((Object) this.Bot != (Object) null)
          this.Bot.BotExplosionPiece(this, dam.point, dam.strikeDir);
        else
          Object.Destroy((Object) this.gameObject);
      }
      else
      {
        if (!((Object) this.Bot != (Object) null))
          return;
        this.Bot.RegisterHit(dam, false);
      }
    }
  }
}
