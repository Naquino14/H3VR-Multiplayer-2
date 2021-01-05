// Decompiled with JetBrains decompiler
// Type: FistVR.BouyantVolume
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BouyantVolume : MonoBehaviour
  {
    public HashSet<Rigidbody> ContainedObjectsHash = new HashSet<Rigidbody>();
    public List<Rigidbody> ContainedObjectsList = new List<Rigidbody>();
    public Dictionary<Rigidbody, float> OgDrag = new Dictionary<Rigidbody, float>();
    public Dictionary<Rigidbody, float> OgAngDrag = new Dictionary<Rigidbody, float>();
    private int testIndex;
    public float SurfaceOfWaterY = 2f;
    private Color FogColor;
    private float FogDensity;
    public Color WaterFogColor;
    public float WaterFogDensity;
    private bool m_isPlayerInWater;
    private Bounds waterBounds;
    public bool UsesImpactSplash;

    private void Awake()
    {
      this.FogColor = RenderSettings.fogColor;
      this.FogDensity = RenderSettings.fogDensity;
      this.waterBounds = this.GetComponent<BoxCollider>().bounds;
    }

    private void FixedUpdate()
    {
      this.TestForPlayerHead();
      this.TestingLoop();
    }

    private void TestForPlayerHead()
    {
      bool flag = this.waterBounds.Contains(GM.CurrentPlayerBody.Head.position);
      if (this.m_isPlayerInWater && !flag)
      {
        this.PlayerExitedWater();
      }
      else
      {
        if (this.m_isPlayerInWater || !flag)
          return;
        this.PlayerEnteredWater();
      }
    }

    private void PlayerEnteredWater()
    {
      this.m_isPlayerInWater = true;
      RenderSettings.fogColor = this.WaterFogColor;
      RenderSettings.fogDensity = this.WaterFogDensity;
    }

    private void PlayerExitedWater()
    {
      this.m_isPlayerInWater = false;
      RenderSettings.fogColor = this.FogColor;
      RenderSettings.fogDensity = this.FogDensity;
    }

    private void TestingLoop()
    {
      if (this.ContainedObjectsList.Count <= 0)
        return;
      for (int index = this.ContainedObjectsList.Count - 1; index >= 0; --index)
      {
        if ((Object) this.ContainedObjectsList[index] == (Object) null)
        {
          this.OgDrag.Remove(this.ContainedObjectsList[index]);
          this.OgAngDrag.Remove(this.ContainedObjectsList[index]);
          this.ContainedObjectsHash.Remove(this.ContainedObjectsList[index]);
          this.ContainedObjectsList.RemoveAt(index);
        }
        else
          this.TestRigidbody(this.ContainedObjectsList[index]);
      }
    }

    private void OnTriggerEnter(Collider col) => this.TestColliderEnter(col);

    private void OnTriggerExit(Collider col) => this.TestColliderExit(col);

    private void TestColliderEnter(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      Rigidbody attachedRigidbody = col.attachedRigidbody;
      if (!this.ContainedObjectsHash.Add(attachedRigidbody))
        return;
      this.ContainedObjectsList.Add(attachedRigidbody);
      if (!this.OgDrag.ContainsKey(attachedRigidbody))
        this.OgDrag.Add(attachedRigidbody, attachedRigidbody.drag);
      if (!this.OgAngDrag.ContainsKey(attachedRigidbody))
        this.OgAngDrag.Add(attachedRigidbody, attachedRigidbody.angularDrag);
      FVRPhysicalObject component = attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if ((Object) component != (Object) null)
      {
        component.IsInWater = true;
        Vector3 pos = new Vector3(component.transform.position.x, this.SurfaceOfWaterY, component.transform.position.z);
        if (component.HasImpactController)
        {
          double num1 = (double) SM.PlayImpactSound(component.AudioImpactController.ImpactType, MatSoundType.Water, AudioImpactIntensity.Hard, pos, FVRPooledAudioType.Impacts, 20f);
        }
        else
        {
          double num2 = (double) SM.PlayImpactSound(ImpactType.Generic, MatSoundType.Water, AudioImpactIntensity.Hard, pos, FVRPooledAudioType.Impacts, 20f);
        }
        if (component is FVRFireArmRound)
          FXM.SpawnImpactEffect(pos, Vector3.up, 3, ImpactEffectMagnitude.Medium, true);
        else
          FXM.SpawnImpactEffect(pos, Vector3.up, 3, ImpactEffectMagnitude.Large, true);
      }
      attachedRigidbody.drag = 5f;
      attachedRigidbody.angularDrag = 5f;
    }

    private void TestColliderExit(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      Rigidbody attachedRigidbody = col.attachedRigidbody;
      if (!this.ContainedObjectsHash.Remove(attachedRigidbody))
        return;
      this.ContainedObjectsList.Remove(attachedRigidbody);
      FVRPhysicalObject component = attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if ((Object) component != (Object) null)
        component.IsInWater = false;
      if (this.OgDrag.ContainsKey(attachedRigidbody))
        col.attachedRigidbody.drag = this.OgDrag[attachedRigidbody];
      if (!this.OgAngDrag.ContainsKey(attachedRigidbody))
        return;
      col.attachedRigidbody.angularDrag = this.OgAngDrag[attachedRigidbody];
    }

    private void TestRigidbody(Rigidbody rb)
    {
      Vector3 worldCenterOfMass = rb.worldCenterOfMass;
      float num1 = worldCenterOfMass.y - this.SurfaceOfWaterY;
      if ((double) num1 >= 0.0)
        return;
      Vector3 force1 = -13f * Mathf.Abs(Physics.gravity.y) * Mathf.Clamp(num1, -2f, 2f) * Vector3.up;
      force1.x = 0.0f;
      force1.z = 0.0f;
      rb.AddForceAtPosition(force1, worldCenterOfMass, ForceMode.Acceleration);
      Vector3 position = rb.transform.position;
      float num2 = Time.time * (float) ((double) Mathf.PerlinNoise(Time.time * 0.2f, position.y) * 0.200000002980232 + 0.100000001490116);
      float num3 = Mathf.PerlinNoise(position.x + num2, position.z + num2);
      float num4 = Mathf.PerlinNoise(position.x + 0.1f - num2, position.z + num2);
      float num5 = Mathf.PerlinNoise(position.x + num2, position.z + 0.1f - num2);
      Vector2 vector2 = new Vector2(num4 - num3, num5 - num3);
      Vector3 force2 = new Vector3(vector2.x, 0.0f, vector2.y) * 3f;
      rb.AddForce(force2, ForceMode.Acceleration);
      Vector3 vector3 = new Vector3(vector2.x, 0.0f, vector2.y) * 1f;
      rb.AddTorque(vector3 * num2 * (1f / 500f));
    }
  }
}
