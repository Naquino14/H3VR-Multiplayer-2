// Decompiled with JetBrains decompiler
// Type: FilterTestQuaternion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class FilterTestQuaternion : MonoBehaviour
{
  public Transform noisyTransform;
  public Transform filteredTransform;
  private Quaternion quat;
  private OneEuroFilter<Quaternion> rotationFilter;
  public bool filterOn = true;
  public float filterFrequency = 120f;
  public float filterMinCutoff = 1f;
  public float filterBeta;
  public float filterDcutoff = 1f;
  public float noiseAmount = 1f;
  private float timer;

  private void Start()
  {
    this.quat = new Quaternion();
    this.quat.eulerAngles = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
    this.rotationFilter = new OneEuroFilter<Quaternion>(this.filterFrequency);
  }

  private void Update()
  {
    this.timer += Time.deltaTime;
    if ((double) this.timer > 2.0)
    {
      this.quat.eulerAngles = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
      this.timer = 0.0f;
    }
    this.noisyTransform.rotation = Quaternion.Slerp(this.noisyTransform.rotation, this.PerturbedRotation(this.quat), 0.5f * Time.deltaTime);
    if (this.filterOn)
    {
      this.rotationFilter.UpdateParams(this.filterFrequency, this.filterMinCutoff, this.filterBeta, this.filterDcutoff);
      this.filteredTransform.rotation = this.rotationFilter.Filter<Quaternion>(this.noisyTransform.rotation);
    }
    else
      this.filteredTransform.rotation = this.noisyTransform.rotation;
  }

  private Quaternion PerturbedRotation(Quaternion _rotation)
  {
    Quaternion quaternion = new Quaternion((float) ((double) Random.value * (double) this.noiseAmount - (double) this.noiseAmount / 2.0), (float) ((double) Random.value * (double) this.noiseAmount - (double) this.noiseAmount / 2.0), (float) ((double) Random.value * (double) this.noiseAmount - (double) this.noiseAmount / 2.0), (float) ((double) Random.value * (double) this.noiseAmount - (double) this.noiseAmount / 2.0));
    quaternion.x *= Time.deltaTime;
    quaternion.y *= Time.deltaTime;
    quaternion.z *= Time.deltaTime;
    quaternion.w *= Time.deltaTime;
    return quaternion * _rotation;
  }
}
