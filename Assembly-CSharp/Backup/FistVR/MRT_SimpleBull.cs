// Decompiled with JetBrains decompiler
// Type: FistVR.MRT_SimpleBull
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MRT_SimpleBull : MonoBehaviour, IFVRDamageable
  {
    [Header("Simple Bull Config")]
    public Texture2D MaskTexture;
    public GameObject[] Shards;
    private Rigidbody[] m_shardRigidbodies;
    public GameObject Spawned_ArriveFX;
    public GameObject Spawned_DestructionFX;
    public float MoveSpeed = 1f;
    public float RotSpeed = 1f;
    public bool ArePointsNegative;
    private float m_moveTick;
    private float m_rotTick;
    private float m_downTick;
    public bool IsSimulHit;
    private ModularRangeSequencer m_sequencer;
    private ModularRangeSequenceDefinition.WaveDefinition m_waveDefinition;
    private ModularRangeSequenceDefinition.TargetMovementStyle m_movementStyle;
    private MRT_SimpleBull.SimpleBullState BState = MRT_SimpleBull.SimpleBullState.SpawnedWaiting;
    private Vector3 m_startFacing = Vector3.zero;
    private Vector3 m_endFacing = Vector3.zero;
    private Vector3 m_startPos = Vector3.zero;
    private Vector3 m_endPos = Vector3.zero;
    private float m_timeToShoot = 1f;
    private int m_spawnIndex;
    private Vector3 m_randomPos = Vector3.zero;
    protected bool isActivated;
    protected bool isDestroyed;

    public void Awake()
    {
      this.m_shardRigidbodies = new Rigidbody[this.Shards.Length];
      for (int index = 0; index < this.Shards.Length; ++index)
        this.m_shardRigidbodies[index] = this.Shards[index].GetComponent<Rigidbody>();
    }

    public void Init(
      ModularRangeSequencer sequencer,
      Vector3 startpos,
      Vector3 endpos,
      float timeToShoot,
      Vector3 startFacing,
      Vector3 endFacing,
      ModularRangeSequenceDefinition.WaveDefinition wavedef,
      int spawnIndex)
    {
      this.m_sequencer = sequencer;
      this.m_waveDefinition = wavedef;
      this.m_spawnIndex = spawnIndex;
      this.m_movementStyle = this.m_waveDefinition.MovementStyle;
      this.m_startPos = startpos;
      this.m_endPos = endpos;
      this.m_timeToShoot = timeToShoot;
      this.m_startFacing = startFacing;
      this.m_endFacing = endFacing;
      this.transform.position = this.m_startPos;
      this.transform.rotation = Quaternion.LookRotation(this.m_startFacing);
      this.m_moveTick = 0.0f;
      this.m_rotTick = 0.0f;
      this.m_downTick = 0.0f;
      this.m_randomPos = new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 3f), this.m_endPos.z);
    }

    public void Damage(FistVR.Damage dam)
    {
    }

    public void Activate()
    {
      this.isActivated = true;
      this.BState = MRT_SimpleBull.SimpleBullState.MovingIn;
    }

    private void Deactivate() => this.isActivated = false;

    public void Update()
    {
      switch (this.BState)
      {
        case MRT_SimpleBull.SimpleBullState.MovingIn:
          if ((double) this.m_moveTick < 1.0)
          {
            this.m_moveTick += Time.deltaTime * this.MoveSpeed;
          }
          else
          {
            this.m_moveTick = 1f;
            this.BState = MRT_SimpleBull.SimpleBullState.RotatingToActivate;
          }
          this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveTick);
          break;
        case MRT_SimpleBull.SimpleBullState.RotatingToActivate:
          if ((double) this.m_rotTick < 1.0)
          {
            this.m_rotTick += Time.deltaTime * this.RotSpeed;
          }
          else
          {
            this.m_rotTick = 1f;
            if ((Object) this.Spawned_ArriveFX != (Object) null)
              Object.Instantiate<GameObject>(this.Spawned_ArriveFX, this.transform.position, this.transform.rotation);
            this.BState = MRT_SimpleBull.SimpleBullState.TickingDown;
          }
          this.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(this.m_startFacing, Vector3.up), Quaternion.LookRotation(this.m_endFacing, Vector3.up), this.m_rotTick * this.m_rotTick);
          break;
        case MRT_SimpleBull.SimpleBullState.TickingDown:
          if ((double) this.m_downTick < (double) this.m_timeToShoot)
          {
            this.m_downTick += Time.deltaTime;
            this.TargetMovement(this.m_downTick, this.m_timeToShoot);
            break;
          }
          this.m_downTick = this.m_timeToShoot;
          this.BState = MRT_SimpleBull.SimpleBullState.RotatingToHide;
          break;
        case MRT_SimpleBull.SimpleBullState.RotatingToHide:
          if ((double) this.m_rotTick > 0.0)
          {
            this.m_rotTick -= Time.deltaTime * this.RotSpeed;
          }
          else
          {
            this.m_rotTick = 0.0f;
            this.Deactivate();
            this.BState = MRT_SimpleBull.SimpleBullState.MovingOut;
          }
          this.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(this.m_startFacing, Vector3.up), Quaternion.LookRotation(this.m_endFacing, Vector3.up), this.m_rotTick * this.m_rotTick);
          break;
        case MRT_SimpleBull.SimpleBullState.MovingOut:
          if ((double) this.m_moveTick > 0.0)
          {
            this.m_moveTick -= Time.deltaTime * this.MoveSpeed;
          }
          else
          {
            this.m_moveTick = 0.0f;
            this.BState = MRT_SimpleBull.SimpleBullState.Finished;
            this.Destroy();
          }
          this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveTick);
          break;
      }
    }

    private void TargetMovement(float tick, float maxTime)
    {
      float num1 = 2f;
      float num2 = 1f;
      float x = 0.0f;
      float y = 0.0f;
      float z = 0.0f;
      float num3 = (float) ((double) (this.m_spawnIndex % 2) * 2.0 - 1.0);
      Vector3 zero = Vector3.zero;
      Vector3 vector3 = Vector3.zero;
      switch (this.m_movementStyle)
      {
        case ModularRangeSequenceDefinition.TargetMovementStyle.Static:
          x = this.m_endPos.x;
          y = this.m_endPos.y;
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.SinusoidX:
          x = Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5)) * num3 * num1;
          y = this.m_endPos.y;
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.SinusoidY:
          x = this.m_endPos.x;
          y = Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5)) * num3 * num1 + num2;
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.WhipX:
          float t1 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos1 = this.m_endPos;
          vector3 = new Vector3(-this.m_endPos.x, this.m_endPos.y, this.m_endPos.z);
          x = Mathf.Lerp(endPos1.x, vector3.x, t1);
          y = this.m_endPos.y;
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.WhipY:
          float t2 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos2 = this.m_endPos;
          vector3 = new Vector3(this.m_endPos.x, (float) (-(double) this.m_endPos.y + 2.0 * (double) num2), this.m_endPos.z);
          x = this.m_endPos.x;
          y = Mathf.Lerp(endPos2.y, vector3.y, t2);
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.WhipZ:
          float t3 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos3 = this.m_endPos;
          vector3 = new Vector3(this.m_endPos.x, this.m_endPos.y, this.m_endPos.z + 10f);
          x = this.m_endPos.x;
          y = this.m_endPos.y;
          z = Mathf.Lerp(endPos3.z, vector3.z, t3);
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.TowardCenterWhipZ:
          float t4 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos4 = this.m_endPos;
          vector3 = new Vector3(-this.m_endPos.x, (float) (-(double) this.m_endPos.y + 2.0 * (double) num2), this.m_endPos.z + 10f);
          x = Mathf.Lerp(endPos4.x, vector3.x, t4);
          y = Mathf.Lerp(endPos4.y, vector3.y, t4);
          z = Mathf.Lerp(endPos4.z, vector3.z, t4);
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.RandomDir:
          float t5 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos5 = this.m_endPos;
          vector3 = new Vector3(this.m_randomPos.x, this.m_randomPos.y, this.m_endPos.z);
          x = Mathf.Lerp(endPos5.x, vector3.x, t5);
          y = Mathf.Lerp(endPos5.y, vector3.y, t5);
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.TowardCenter:
          float t6 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos6 = this.m_endPos;
          vector3 = new Vector3(-this.m_endPos.x, (float) (-(double) this.m_endPos.y + 2.0 * (double) num2), this.m_endPos.z);
          x = Mathf.Lerp(endPos6.x, vector3.x, t6);
          y = Mathf.Lerp(endPos6.y, vector3.y, t6);
          z = this.m_endPos.z;
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.WhipXWhipZ:
          float t7 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos7 = this.m_endPos;
          vector3 = new Vector3(-this.m_endPos.x, this.m_endPos.y, this.m_endPos.z + 10f);
          x = Mathf.Lerp(endPos7.x, vector3.x, t7);
          y = this.m_endPos.y;
          z = Mathf.Lerp(endPos7.z, vector3.z, t7);
          break;
        case ModularRangeSequenceDefinition.TargetMovementStyle.WhipYWhipZ:
          float t8 = (float) ((double) Mathf.Sin((float) ((double) tick / (double) maxTime * (double) maxTime * 3.14159274101257 * 0.5 - 1.57079637050629)) / 2.0 + 0.5);
          Vector3 endPos8 = this.m_endPos;
          vector3 = new Vector3(this.m_endPos.x, (float) (-(double) this.m_endPos.y + 2.0 * (double) num2), this.m_endPos.z + 10f);
          x = this.m_endPos.x;
          y = Mathf.Lerp(endPos8.y, vector3.y, t8);
          z = Mathf.Lerp(endPos8.z, vector3.z, t8);
          break;
      }
      this.transform.position = new Vector3(x, y, z);
    }

    public void Destroy()
    {
      if (this.isDestroyed)
        return;
      this.isDestroyed = true;
      Object.Destroy((Object) this.gameObject);
    }

    public void Destroy(Vector3 point, Vector3 force)
    {
      if (this.isDestroyed)
        return;
      this.isDestroyed = true;
      if ((Object) this.Spawned_DestructionFX != (Object) null)
        Object.Instantiate<GameObject>(this.Spawned_DestructionFX, this.transform.position, this.transform.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.m_shardRigidbodies[index].AddForceAtPosition(force * 6f, point);
        this.m_shardRigidbodies[index].AddExplosionForce(force.magnitude * 6f, point, 15f, 0.1f);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public enum SimpleBullState
    {
      SpawnedWaiting = -1, // 0xFFFFFFFF
      MovingIn = 0,
      RotatingToActivate = 1,
      TickingDown = 2,
      RotatingToHide = 3,
      MovingOut = 4,
      Finished = 5,
    }
  }
}
