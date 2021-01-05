using UnityEngine;
using UnityEngine.UI;

public class GamepadTesting : MonoBehaviour
{
	public Text label;

	private void Update()
	{
		string empty = string.Empty;
		string text = empty;
		empty = text + "LSX:" + Input.GetAxis("LeftStickXAxis") + "\n";
		text = empty;
		empty = text + "LSY:" + Input.GetAxis("LeftStickYAxis") + "\n";
		text = empty;
		empty = text + "RSX:" + Input.GetAxis("RightStickXAxis") + "\n";
		text = empty;
		empty = text + "RSY:" + Input.GetAxis("RightStickYAxis") + "\n";
		text = empty;
		empty = text + "DPX:" + Input.GetAxis("D-Pad X Axis") + "\n";
		text = empty;
		empty = text + "DPY:" + Input.GetAxis("D-Pad Y Axis") + "\n";
		text = empty;
		empty = text + "LT:" + Input.GetAxis("Left Trigger") + "\n";
		text = empty;
		empty = text + "RT:" + Input.GetAxis("Right Trigger") + "\n";
		text = empty;
		empty = text + "AB:" + Input.GetButton("A Button") + "\n";
		text = empty;
		empty = text + "BB:" + Input.GetButton("B Button") + "\n";
		text = empty;
		empty = text + "XB:" + Input.GetButton("X Button") + "\n";
		text = empty;
		empty = text + "YB:" + Input.GetButton("Y Button") + "\n";
		text = empty;
		empty = text + "LB:" + Input.GetButton("Left Bumper") + "\n";
		text = empty;
		empty = text + "RB:" + Input.GetButton("Right Bumper") + "\n";
		text = empty;
		empty = text + "Back:" + Input.GetButton("Back Button") + "\n";
		text = empty;
		empty = text + "Start:" + Input.GetButton("Start Button") + "\n";
		text = empty;
		empty = text + "LSC:" + Input.GetButton("Left Stick Click") + "\n";
		text = empty;
		empty = text + "RSC:" + Input.GetButton("Right Stick Click") + "\n";
		label.text = empty;
	}
}
