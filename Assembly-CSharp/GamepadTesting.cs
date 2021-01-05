// Decompiled with JetBrains decompiler
// Type: GamepadTesting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class GamepadTesting : MonoBehaviour
{
  public Text label;

  private void Update() => this.label.text = string.Empty + "LSX:" + (object) Input.GetAxis("LeftStickXAxis") + "\n" + "LSY:" + (object) Input.GetAxis("LeftStickYAxis") + "\n" + "RSX:" + (object) Input.GetAxis("RightStickXAxis") + "\n" + "RSY:" + (object) Input.GetAxis("RightStickYAxis") + "\n" + "DPX:" + (object) Input.GetAxis("D-Pad X Axis") + "\n" + "DPY:" + (object) Input.GetAxis("D-Pad Y Axis") + "\n" + "LT:" + (object) Input.GetAxis("Left Trigger") + "\n" + "RT:" + (object) Input.GetAxis("Right Trigger") + "\n" + "AB:" + (object) Input.GetButton("A Button") + "\n" + "BB:" + (object) Input.GetButton("B Button") + "\n" + "XB:" + (object) Input.GetButton("X Button") + "\n" + "YB:" + (object) Input.GetButton("Y Button") + "\n" + "LB:" + (object) Input.GetButton("Left Bumper") + "\n" + "RB:" + (object) Input.GetButton("Right Bumper") + "\n" + "Back:" + (object) Input.GetButton("Back Button") + "\n" + "Start:" + (object) Input.GetButton("Start Button") + "\n" + "LSC:" + (object) Input.GetButton("Left Stick Click") + "\n" + "RSC:" + (object) Input.GetButton("Right Stick Click") + "\n";
}
