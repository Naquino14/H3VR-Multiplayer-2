// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpikeCeilingPlate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_SpikeCeilingPlate : MonoBehaviour
  {
    public Transform ReciprocatingSpikes;
    public Transform[] CastPointsLong;
    public Transform[] CastPointsShort;
    private int m_longIndex;
    private int m_shortIndex;
    public LayerMask PlayerLM;
    private RaycastHit m_hit;
    private float longLength = 1.9f;
    private float shortLength = 1.2f;

    public void Retract() => this.ReciprocatingSpikes.transform.localPosition = new Vector3(0.0f, 1.2f, 0.0f);

    public void Expand() => this.ReciprocatingSpikes.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

    public void Update()
    {
      ++this.m_longIndex;
      if (this.m_longIndex >= this.CastPointsLong.Length)
        this.m_longIndex = 0;
      ++this.m_shortIndex;
      if (this.m_shortIndex >= this.CastPointsShort.Length)
        this.m_shortIndex = 0;
      if (Physics.Raycast(this.CastPointsLong[this.m_longIndex].position, Vector3.down, out this.m_hit, this.longLength, (int) this.PlayerLM, QueryTriggerInteraction.Collide) && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
      {
        FVRPlayerHitbox component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
        if ((Object) component != (Object) null)
          component.Damage(new DamageDealt()
          {
            force = Vector3.zero,
            hitNormal = Vector3.zero,
            IsInside = false,
            MPa = 1f,
            MPaRootMeter = 1f,
            point = this.transform.position,
            PointsDamage = 6000f,
            ShotOrigin = (Transform) null,
            strikeDir = Vector3.zero,
            uvCoords = Vector2.zero,
            IsInitialContact = true
          });
      }
      if (!Physics.Raycast(this.CastPointsShort[this.m_shortIndex].position, Vector3.down, out this.m_hit, this.shortLength, (int) this.PlayerLM, QueryTriggerInteraction.Collide) || !((Object) this.m_hit.collider.attachedRigidbody != (Object) null))
        return;
      FVRPlayerHitbox component1 = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
      if (!((Object) component1 != (Object) null))
        return;
      component1.Damage(new DamageDealt()
      {
        force = Vector3.zero,
        hitNormal = Vector3.zero,
        IsInside = false,
        MPa = 1f,
        MPaRootMeter = 1f,
        point = this.transform.position,
        PointsDamage = 6000f,
        ShotOrigin = (Transform) null,
        strikeDir = Vector3.zero,
        uvCoords = Vector2.zero,
        IsInitialContact = true
      });
    }
  }
}
