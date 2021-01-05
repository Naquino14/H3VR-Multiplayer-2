// Decompiled with JetBrains decompiler
// Type: FistVR.MaskedTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MaskedTarget : MonoBehaviour, IFVRDamageable
  {
    public Texture2D MaskTexture;
    public string DisplayText;
    public Rigidbody[] Shards;
    private bool isMoving;
    private bool isRotating;
    private bool isDestroyed;
    private float m_speedMult;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Vector3 m_startFacing;
    private Vector3 m_endFacing;
    private Vector3 m_endUpVector;
    private float m_moveTick;
    private float m_rotTick;
    public GameObject ExplosionSound;
    private SequencerV1 Sequencer;

    public void Init(
      Vector3 startPos,
      Vector3 endPos,
      float speedMult,
      Vector3 startFacing,
      Vector3 endFacing,
      SequencerV1 seq)
    {
      this.Sequencer = seq;
      this.m_startPos = startPos;
      this.m_endPos = endPos;
      this.m_speedMult = speedMult;
      this.m_startFacing = startFacing;
      this.m_endFacing = endFacing;
      this.transform.position = startPos;
      this.transform.rotation = Quaternion.LookRotation(startFacing);
      this.isMoving = true;
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      Vector3 vector3 = this.transform.InverseTransformPoint(dam.point) * 0.5f + new Vector3(0.5f, 0.5f, 0.0f);
      Color pixel = this.MaskTexture.GetPixel(Mathf.RoundToInt((float) this.MaskTexture.width * vector3.x), Mathf.RoundToInt((float) this.MaskTexture.width * vector3.y));
      Debug.Log((object) ("local point" + (object) vector3));
      Debug.Log((object) ("alpha is:" + (object) pixel.a));
      if (Mathf.RoundToInt(pixel.a * 10f) <= 0 || this.isDestroyed)
        return;
      this.isDestroyed = true;
      this.Sequencer.AddTargetPoints(Mathf.RoundToInt(pixel.a * 10f));
      this.Destroy(dam.point, dam.strikeDir * 5f);
    }

    public void Update()
    {
      if (this.isMoving)
      {
        if ((double) this.m_moveTick < 1.0)
        {
          this.m_moveTick += Time.deltaTime * this.m_speedMult;
        }
        else
        {
          this.m_moveTick = 1f;
          this.isMoving = false;
          this.isRotating = true;
          this.m_endUpVector = Random.onUnitSphere;
        }
        this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveTick * this.m_moveTick);
      }
      if (!this.isRotating)
        return;
      if ((double) this.m_rotTick < 1.0)
      {
        this.m_rotTick += Time.deltaTime * this.m_speedMult;
      }
      else
      {
        this.m_rotTick = 1f;
        this.isRotating = false;
      }
      this.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(this.m_startFacing, Vector3.up), Quaternion.LookRotation(this.m_endFacing, this.m_endUpVector), this.m_rotTick * this.m_rotTick);
    }

    public void Destroy(Vector3 point, Vector3 force)
    {
      Object.Instantiate<GameObject>(this.ExplosionSound, this.transform.position, this.transform.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddExplosionForce(4f, point, 15f, 0.25f, ForceMode.Impulse);
      }
      Object.Destroy((Object) this.gameObject);
    }
  }
}
