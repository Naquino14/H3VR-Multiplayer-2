// Decompiled with JetBrains decompiler
// Type: FistVR.PlayerSosigBody
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PlayerSosigBody : MonoBehaviour
  {
    private Transform head;
    private Transform torso;
    public Rigidbody RB;
    public Transform Sosig_Head;
    public Transform Sosig_Torso;
    public Transform Sosig_Abdomen;
    public Transform Sosig_Legs;
    private List<GameObject> m_curClothes = new List<GameObject>();
    private float AttachedRotationMultiplier = 30f;
    private float AttachedPositionMultiplier = 4500f;
    private float AttachedRotationFudge = 500f;
    private float AttachedPositionFudge = 500f;

    private void Start()
    {
      this.head = GM.CurrentPlayerBody.Head;
      this.torso = GM.CurrentPlayerBody.Torso;
    }

    private void FixedUpdate()
    {
      if ((double) this.DistanceFromCoreTarget() > 1.0)
        this.RB.position = this.torso.position - this.torso.up * 0.25f;
      this.SosigPhys();
    }

    private void SosigPhys()
    {
      Vector3 position = this.RB.position;
      Quaternion rotation1 = this.RB.rotation;
      Vector3 vector3_1 = this.torso.position - this.torso.up * 0.25f;
      Quaternion rotation2 = this.torso.rotation;
      Vector3 vector3_2 = vector3_1 - position;
      Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation1);
      float deltaTime = Time.deltaTime;
      float angle;
      Vector3 axis;
      quaternion.ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
        this.RB.angularVelocity = Vector3.MoveTowards(this.RB.angularVelocity, deltaTime * angle * axis * this.AttachedRotationMultiplier, this.AttachedRotationFudge * Time.fixedDeltaTime);
      this.RB.velocity = Vector3.MoveTowards(this.RB.velocity, vector3_2 * this.AttachedPositionMultiplier * deltaTime, this.AttachedPositionFudge * deltaTime);
    }

    public float DistanceFromCoreTarget() => Vector3.Distance(this.RB.position, this.torso.position);

    public void ApplyOutfit(SosigOutfitConfig o)
    {
      if (this.m_curClothes.Count > 0)
      {
        for (int index = this.m_curClothes.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_curClothes[index] != (Object) null)
            Object.Destroy((Object) this.m_curClothes[index]);
        }
      }
      this.m_curClothes.Clear();
      this.SpawnAccesoryToLink(o.Headwear, this.Sosig_Head, o.Chance_Headwear);
      this.SpawnAccesoryToLink(o.Facewear, this.Sosig_Head, o.Chance_Facewear);
      this.SpawnAccesoryToLink(o.Eyewear, this.Sosig_Head, o.Chance_Eyewear);
      this.SpawnAccesoryToLink(o.Torsowear, this.Sosig_Torso, o.Chance_Torsowear);
      this.SpawnAccesoryToLink(o.Pantswear, this.Sosig_Abdomen, o.Chance_Pantswear);
      this.SpawnAccesoryToLink(o.Pantswear_Lower, this.Sosig_Legs, o.Chance_Pantswear_Lower);
      this.SpawnAccesoryToLink(o.Backpacks, this.Sosig_Torso, o.Chance_Backpacks);
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, Transform l, float chance)
    {
      if ((double) Random.Range(0.0f, 1f) > (double) chance || gs.Count < 1)
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.position, l.rotation);
      this.m_curClothes.Add(gameObject);
      Component[] componentsInChildren = gameObject.GetComponentsInChildren<Component>(true);
      for (int index = componentsInChildren.Length - 1; index >= 0; --index)
      {
        componentsInChildren[index].gameObject.layer = LayerMask.NameToLayer("ExternalCamOnly");
        if (!(componentsInChildren[index] is Transform) && !(componentsInChildren[index] is MeshFilter) && !(componentsInChildren[index] is MeshRenderer))
          Object.Destroy((Object) componentsInChildren[index]);
      }
      gameObject.transform.SetParent(l);
    }
  }
}
