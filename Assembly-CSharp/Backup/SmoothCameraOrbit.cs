// Decompiled with JetBrains decompiler
// Type: SmoothCameraOrbit
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
public class SmoothCameraOrbit : MonoBehaviour
{
  public Transform target;
  public Vector3 targetOffset;
  public float distance = 5f;
  public float maxDistance = 20f;
  public float minDistance = 0.6f;
  public float xSpeed = 200f;
  public float ySpeed = 200f;
  public int yMinLimit = -80;
  public int yMaxLimit = 80;
  public int zoomRate = 40;
  public float panSpeed = 0.3f;
  public float zoomDampening = 5f;
  public float autoRotate = 1f;
  public float autoRotateSpeed = 0.1f;
  private float xDeg;
  private float yDeg;
  private float currentDistance;
  private float desiredDistance;
  private Quaternion currentRotation;
  private Quaternion desiredRotation;
  private Quaternion rotation;
  private Vector3 position;
  private float idleTimer;
  private float idleSmooth;

  private void Start() => this.Init();

  private void OnEnable() => this.Init();

  public void Init()
  {
    if (!(bool) (Object) this.target)
      this.target = new GameObject("Cam Target")
      {
        transform = {
          position = (this.transform.position + this.transform.forward * this.distance)
        }
      }.transform;
    this.currentDistance = this.distance;
    this.desiredDistance = this.distance;
    this.position = this.transform.position;
    this.rotation = this.transform.rotation;
    this.currentRotation = this.transform.rotation;
    this.desiredRotation = this.transform.rotation;
    this.xDeg = Vector3.Angle(Vector3.right, this.transform.right);
    this.yDeg = Vector3.Angle(Vector3.up, this.transform.up);
    this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
  }

  private void LateUpdate()
  {
    if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
      this.desiredDistance -= (float) ((double) Input.GetAxis("Mouse Y") * 0.0199999995529652 * (double) this.zoomRate * 0.125) * Mathf.Abs(this.desiredDistance);
    else if (Input.GetMouseButton(0))
    {
      this.xDeg += (float) ((double) Input.GetAxis("Mouse X") * (double) this.xSpeed * 0.0199999995529652);
      this.yDeg -= (float) ((double) Input.GetAxis("Mouse Y") * (double) this.ySpeed * 0.0199999995529652);
      this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float) this.yMinLimit, (float) this.yMaxLimit);
      this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0.0f);
      this.currentRotation = this.transform.rotation;
      this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, 0.02f * this.zoomDampening);
      this.transform.rotation = this.rotation;
      this.idleTimer = 0.0f;
      this.idleSmooth = 0.0f;
    }
    else
    {
      this.idleTimer += 0.02f;
      if ((double) this.idleTimer > (double) this.autoRotate && (double) this.autoRotate > 0.0)
      {
        this.idleSmooth += (float) ((0.0199999995529652 + (double) this.idleSmooth) * 0.00499999988824129);
        this.idleSmooth = Mathf.Clamp(this.idleSmooth, 0.0f, 1f);
        this.xDeg += this.xSpeed * Time.deltaTime * this.idleSmooth * this.autoRotateSpeed;
      }
      this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float) this.yMinLimit, (float) this.yMaxLimit);
      this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0.0f);
      this.currentRotation = this.transform.rotation;
      this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, (float) (0.0199999995529652 * (double) this.zoomDampening * 2.0));
      this.transform.rotation = this.rotation;
    }
    this.desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * (float) this.zoomRate * Mathf.Abs(this.desiredDistance);
    this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
    this.currentDistance = Mathf.Lerp(this.currentDistance, this.desiredDistance, 0.02f * this.zoomDampening);
    this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
    this.transform.position = this.position;
  }

  private static float ClampAngle(float angle, float min, float max)
  {
    if ((double) angle < -360.0)
      angle += 360f;
    if ((double) angle > 360.0)
      angle -= 360f;
    return Mathf.Clamp(angle, min, max);
  }
}
