using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class FVRViveHandMenu : MonoBehaviour
	{
		public enum MenuState
		{
			Main,
			TouchNone,
			TouchTeleport,
			TouchSceneReset,
			TouchGrab,
			TouchMoo
		}

		private MenuState curState;

		public Transform Target;

		public Text Text_Main;

		public Text Text_Teleport;

		public Text Text_SceneReset;

		public Text Text_Grab;

		public Text Text_Moo;

		public FVRViveHand Hand;

		public void LateUpdate()
		{
			base.transform.position = Target.position;
			base.transform.rotation = Target.rotation;
		}

		public void SetMenuState(MenuState state)
		{
			if (curState != state)
			{
				Hand.Buzz(Hand.Buzzer.Buzz_OnMenuOption);
			}
			curState = state;
			switch (state)
			{
			case MenuState.Main:
				Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				break;
			case MenuState.TouchNone:
				Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				break;
			case MenuState.TouchSceneReset:
				Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_SceneReset.color = new Color(0.1f, 1f, 0.1f, 1f);
				Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				break;
			case MenuState.TouchTeleport:
				Text_Teleport.color = new Color(0.1f, 1f, 0.1f, 1f);
				Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				break;
			case MenuState.TouchGrab:
				Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Grab.color = new Color(0.1f, 1f, 0.1f, 1f);
				Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				break;
			case MenuState.TouchMoo:
				Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
				Text_Moo.color = new Color(0.1f, 1f, 0.1f, 1f);
				break;
			}
		}
	}
}
