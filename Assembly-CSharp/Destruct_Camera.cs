// Decompiled with JetBrains decompiler
// Type: Destruct_Camera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof (Camera))]
[AddComponentMenu("Raycast destroyer (GabroMedia)")]
public class Destruct_Camera : MonoBehaviour
{
  private Camera cam;
  private bool inCycle;
  private GameObject smoke;
  public AudioClip[] shotSound;
  [Range(10f, 60f)]
  public float removeDebrisDelay;
  [Range(5f, 40f)]
  public float impactForce;
  [Range(0.0f, 1f)]
  public float gunShotFrequency;

  private void Start() => this.cam = this.GetComponent<Camera>();

  private void Update()
  {
    Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitInfo;
    if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) || !Input.GetMouseButton(0) || this.inCycle)
      return;
    this.StartCoroutine(this.delay(this.gunShotFrequency));
    AudioSource.PlayClipAtPoint(this.shotSound[Random.Range(0, this.shotSound.Length)], this.transform.position);
    if (!(bool) (Object) hitInfo.transform.GetComponent<Destruct>())
      return;
    this.BreakObject(hitInfo.transform.GetComponent<Destruct>(), ray.direction, hitInfo.point);
  }

  private void BreakObject(Destruct instance, Vector3 rayDirection, Vector3 hitPoint)
  {
    GameObject fracturedPrefab = instance.fracturedPrefab;
    GameObject smokeParticle = instance.smokeParticle;
    AudioClip shatter = instance.shatter;
    Vector3 position1 = instance.transform.position;
    Quaternion rotation = instance.transform.rotation;
    Object.Destroy((Object) instance.gameObject);
    if ((bool) (Object) smokeParticle)
    {
      Vector3 position2 = new Vector3(position1.x, position1.y + 1f, position1.z);
      this.smoke = Object.Instantiate<GameObject>(smokeParticle, position2, rotation);
    }
    AudioSource.PlayClipAtPoint(shatter, position1);
    GameObject go = Object.Instantiate<GameObject>(fracturedPrefab, position1, rotation);
    go.name = "FracturedClone";
    foreach (Rigidbody componentsInChild in go.GetComponentsInChildren<Rigidbody>())
      componentsInChild.AddForceAtPosition(rayDirection * this.impactForce, hitPoint, ForceMode.Impulse);
    this.StartCoroutine(this.removeDebris(go, this.removeDebrisDelay));
    if (!(bool) (Object) this.smoke)
      return;
    Object.Destroy((Object) this.smoke, 2f);
  }

  [DebuggerHidden]
  private IEnumerator delay(float secs) => (IEnumerator) new Destruct_Camera.\u003Cdelay\u003Ec__Iterator0()
  {
    secs = secs,
    \u0024this = this
  };

  [DebuggerHidden]
  private IEnumerator removeDebris(GameObject go, float seconds) => (IEnumerator) new Destruct_Camera.\u003CremoveDebris\u003Ec__Iterator1()
  {
    seconds = seconds,
    go = go
  };
}
