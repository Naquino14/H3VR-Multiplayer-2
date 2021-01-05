// Decompiled with JetBrains decompiler
// Type: Nl_DragObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class Nl_DragObject : MonoBehaviour
{
  public static Camera Cam;
  private Rigidbody rigidboy;
  private float distanceZ;
  private bool isTaken;
  private Vector3 offset;
  private Vector3 dir;

  private void Start() => this.rigidboy = this.gameObject.GetComponent<Rigidbody>();

  private void Update()
  {
    if (!this.isTaken)
      return;
    if (Input.GetMouseButton(1))
    {
      Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distanceZ);
      this.rigidboy.MovePosition(Nl_DragObject.Cam.ScreenToWorldPoint(position) + this.offset);
    }
    else
    {
      this.rigidboy.useGravity = true;
      this.rigidboy.constraints = RigidbodyConstraints.None;
      this.isTaken = false;
    }
    if ((double) Input.GetAxis("Horizontal") != 0.0 && Input.GetKey(KeyCode.LeftAlt))
      this.transform.Rotate(Vector3.up * 100f * Time.deltaTime * Input.GetAxis("Horizontal"));
    if ((double) Input.GetAxis("Vertical") == 0.0 || !Input.GetKey(KeyCode.LeftAlt))
      return;
    this.transform.Rotate(Vector3.right * 100f * Time.deltaTime * Input.GetAxis("Vertical"));
  }

  private void OnMouseOver()
  {
    if (!Input.GetKey(KeyCode.LeftAlt) || !Input.GetMouseButtonDown(1))
      return;
    this.isTaken = true;
    this.distanceZ = Vector3.Distance(Nl_DragObject.Cam.transform.position, this.gameObject.transform.position);
    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distanceZ);
    this.offset = this.rigidboy.position - Nl_DragObject.Cam.ScreenToWorldPoint(position);
    this.rigidboy.velocity = Vector3.zero;
    this.rigidboy.useGravity = false;
    this.rigidboy.constraints = RigidbodyConstraints.FreezeRotation;
  }
}
