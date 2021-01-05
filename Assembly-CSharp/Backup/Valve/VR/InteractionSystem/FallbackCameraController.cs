// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.FallbackCameraController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Camera))]
  public class FallbackCameraController : MonoBehaviour
  {
    public float speed = 4f;
    public float shiftSpeed = 16f;
    public bool showInstructions = true;
    private Vector3 startEulerAngles;
    private Vector3 startMousePosition;
    private float realTime;

    private void OnEnable() => this.realTime = Time.realtimeSinceStartup;

    private void Update()
    {
      float z = 0.0f;
      if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        ++z;
      if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        --z;
      float x = 0.0f;
      if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        ++x;
      if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        --x;
      float num1 = this.speed;
      if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        num1 = this.shiftSpeed;
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      float num2 = realtimeSinceStartup - this.realTime;
      this.realTime = realtimeSinceStartup;
      this.transform.position += this.transform.TransformDirection(new Vector3(x, 0.0f, z) * num1 * num2);
      Vector3 mousePosition = Input.mousePosition;
      if (Input.GetMouseButtonDown(1))
      {
        this.startMousePosition = mousePosition;
        this.startEulerAngles = this.transform.localEulerAngles;
      }
      if (!Input.GetMouseButton(1))
        return;
      Vector3 vector3 = mousePosition - this.startMousePosition;
      this.transform.localEulerAngles = this.startEulerAngles + new Vector3((float) (-(double) vector3.y * 360.0) / (float) Screen.height, vector3.x * 360f / (float) Screen.width, 0.0f);
    }

    private void OnGUI()
    {
      if (!this.showInstructions)
        return;
      GUI.Label(new Rect(10f, 10f, 600f, 400f), "WASD/Arrow Keys to translate the camera\nRight mouse click to rotate the camera\nLeft mouse click for standard interactions.\n");
    }
  }
}
