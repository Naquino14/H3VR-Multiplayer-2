// Decompiled with JetBrains decompiler
// Type: FistVR.ShatterablePhysicalObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ShatterablePhysicalObject : FVRPhysicalObject, IFVRDamageable
  {
    public float currentToughness = 200f;
    [Header("Shatterable Params")]
    public PMat Pmaterial;
    public Rigidbody[] SubObjects;
    public GameObject[] SecondarySpawns;
    private bool isShattered;
    public float CollisionShatterThreshold = 5f;
    public bool TransfersVelocityExplosively;
    public float DamageReceivedMultiplier = 1f;
    private Rigidbody m_rb;
    private bool m_hasRigidbody;

    protected override void Awake()
    {
      base.Awake();
      if (!((Object) this.m_rb != (Object) null))
        return;
      this.m_hasRigidbody = true;
    }

    protected override void FVRUpdate() => base.FVRUpdate();

    public void Damage(FistVR.Damage dam)
    {
      if (this.isShattered || (Object) this.QuickbeltSlot != (Object) null)
        return;
      Vector3 force = dam.Dam_TotalKinetic * dam.strikeDir * 0.01f * this.DamageReceivedMultiplier;
      Vector3 point = dam.point;
      this.currentToughness -= dam.Dam_TotalKinetic;
      if ((double) this.currentToughness <= 0.0)
      {
        this.isShattered = true;
        if (this.SubObjects.Length > 0)
        {
          for (int index = 0; index < this.SubObjects.Length; ++index)
          {
            this.SubObjects[index].gameObject.SetActive(true);
            this.SubObjects[index].transform.SetParent((Transform) null);
            this.SubObjects[index].velocity = this.RootRigidbody.velocity + force * (1f / (float) this.SubObjects.Length) * 0.1f;
            this.SubObjects[index].angularVelocity = this.RootRigidbody.velocity;
            if (this.TransfersVelocityExplosively)
              this.SubObjects[index].AddForceAtPosition((this.SubObjects[index].transform.position - this.transform.position).normalized * this.SubObjects[index].velocity.magnitude / (float) this.SubObjects.Length, this.transform.position, ForceMode.Impulse);
          }
        }
        for (int index = 0; index < this.SecondarySpawns.Length; ++index)
          Object.Instantiate<GameObject>(this.SecondarySpawns[index], dam.point, Quaternion.LookRotation(-dam.hitNormal, Random.onUnitSphere));
        if (this.IsHeld)
          this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
        Object.Destroy((Object) this.gameObject);
      }
      else
        this.RootRigidbody.AddForceAtPosition(force, point, ForceMode.Impulse);
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      float magnitude = col.relativeVelocity.magnitude;
      if ((double) magnitude < (double) this.CollisionShatterThreshold)
        return;
      float num1 = 100f;
      if (this.m_hasRigidbody)
        num1 = this.m_rb.mass;
      float num2 = num1;
      if ((Object) col.rigidbody != (Object) null)
        num2 = col.rigidbody.mass;
      float num3 = num2 / num1;
      float num4 = magnitude * num3;
      Vector3 relativeVelocity = col.relativeVelocity;
      FistVR.Damage dam = new FistVR.Damage()
      {
        Dam_Blunt = (float) ((double) num4 * (double) magnitude * 100.0)
      };
      dam.Dam_TotalKinetic = dam.Dam_Blunt;
      dam.hitNormal = col.contacts[0].normal;
      dam.strikeDir = col.relativeVelocity.normalized;
      dam.point = col.contacts[0].point;
      dam.Class = FistVR.Damage.DamageClass.Environment;
      Debug.Log((object) ("collision:" + (object) dam.Dam_Blunt));
      this.Damage(dam);
    }
  }
}
