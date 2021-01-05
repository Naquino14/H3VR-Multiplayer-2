using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
	public class BuggyController : MonoBehaviour
	{
		public Transform modelJoystick;

		public float joystickRot = 20f;

		public Transform modelTrigger;

		public float triggerRot = 20f;

		public BuggyBuddy buggy;

		public Transform buttonBrake;

		public Transform buttonReset;

		public Canvas ui_Canvas;

		public Image ui_rpm;

		public Image ui_speed;

		public RectTransform ui_steer;

		public float ui_steerangle;

		public Vector2 ui_fillAngles;

		public Transform resetToPoint;

		public SteamVR_Action_Vector2 actionSteering = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("buggy", "Steering");

		public SteamVR_Action_Single actionThrottle = SteamVR_Input.GetAction<SteamVR_Action_Single>("buggy", "Throttle");

		public SteamVR_Action_Boolean actionBrake = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Brake");

		public SteamVR_Action_Boolean actionReset = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Reset");

		private float usteer;

		private Interactable interactable;

		private Quaternion trigSRot;

		private Quaternion joySRot;

		private Coroutine resettingRoutine;

		private Vector3 initialScale;

		private float buzztimer;

		private void Start()
		{
			joySRot = modelJoystick.localRotation;
			trigSRot = modelTrigger.localRotation;
			interactable = GetComponent<Interactable>();
			StartCoroutine(DoBuzz());
			buggy.controllerReference = base.transform;
			initialScale = buggy.transform.localScale;
		}

		private void Update()
		{
			Vector2 steer = Vector2.zero;
			float num = 0f;
			float handBrake = 0f;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if ((bool)interactable.attachedToHand)
			{
				SteamVR_Input_Sources handType = interactable.attachedToHand.handType;
				steer = actionSteering.GetAxis(handType);
				num = actionThrottle.GetAxis(handType);
				flag2 = actionBrake.GetState(handType);
				flag3 = actionReset.GetState(handType);
				handBrake = (flag2 ? 1 : 0);
				flag = actionReset.GetStateDown(handType);
			}
			if (flag && resettingRoutine == null)
			{
				resettingRoutine = StartCoroutine(DoReset());
			}
			if (ui_Canvas != null)
			{
				ui_Canvas.gameObject.SetActive(interactable.attachedToHand);
				usteer = Mathf.Lerp(usteer, steer.x, Time.deltaTime * 9f);
				ui_steer.localEulerAngles = Vector3.forward * usteer * (0f - ui_steerangle);
				ui_rpm.fillAmount = Mathf.Lerp(ui_rpm.fillAmount, Mathf.Lerp(ui_fillAngles.x, ui_fillAngles.y, num), Time.deltaTime * 4f);
				float num2 = 40f;
				ui_speed.fillAmount = Mathf.Lerp(ui_fillAngles.x, ui_fillAngles.y, 1f - Mathf.Exp((0f - buggy.speed) / num2));
			}
			modelJoystick.localRotation = joySRot;
			modelJoystick.Rotate(steer.y * (0f - joystickRot), steer.x * (0f - joystickRot), 0f, Space.Self);
			modelTrigger.localRotation = trigSRot;
			modelTrigger.Rotate(num * (0f - triggerRot), 0f, 0f, Space.Self);
			buttonBrake.localScale = new Vector3(1f, 1f, (!flag2) ? 1f : 0.4f);
			buttonReset.localScale = new Vector3(1f, 1f, (!flag3) ? 1f : 0.4f);
			buggy.steer = steer;
			buggy.throttle = num;
			buggy.handBrake = handBrake;
			buggy.controllerReference = base.transform;
		}

		private IEnumerator DoReset()
		{
			float startTime = Time.time;
			float overTime = 1f;
			float endTime = startTime + overTime;
			buggy.transform.position = resetToPoint.transform.position;
			buggy.transform.rotation = resetToPoint.transform.rotation;
			buggy.transform.localScale = initialScale * 0.1f;
			while (Time.time < endTime)
			{
				buggy.transform.localScale = Vector3.Lerp(buggy.transform.localScale, initialScale, Time.deltaTime * 5f);
				yield return null;
			}
			buggy.transform.localScale = initialScale;
			resettingRoutine = null;
		}

		private IEnumerator DoBuzz()
		{
			while (true)
			{
				if (buzztimer < 1f)
				{
					buzztimer += Time.deltaTime * buggy.mvol * 70f;
					yield return null;
					continue;
				}
				buzztimer = 0f;
				if ((bool)interactable.attachedToHand)
				{
					interactable.attachedToHand.TriggerHapticPulse((ushort)Mathf.RoundToInt(300f * Mathf.Lerp(1f, 0.6f, buggy.mvol)));
				}
			}
		}
	}
}
