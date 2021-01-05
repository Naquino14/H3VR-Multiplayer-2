using UnityEngine;
using UnityEngine.EventSystems;

namespace Valve.VR.InteractionSystem
{
	public class InputModule : BaseInputModule
	{
		private GameObject submitObject;

		private static InputModule _instance;

		public static InputModule instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType<InputModule>();
				}
				return _instance;
			}
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			return submitObject != null;
		}

		public void HoverBegin(GameObject gameObject)
		{
			PointerEventData eventData = new PointerEventData(base.eventSystem);
			ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerEnterHandler);
		}

		public void HoverEnd(GameObject gameObject)
		{
			PointerEventData pointerEventData = new PointerEventData(base.eventSystem);
			pointerEventData.selectedObject = null;
			ExecuteEvents.Execute(gameObject, pointerEventData, ExecuteEvents.pointerExitHandler);
		}

		public void Submit(GameObject gameObject)
		{
			submitObject = gameObject;
		}

		public override void Process()
		{
			if ((bool)submitObject)
			{
				BaseEventData baseEventData = GetBaseEventData();
				baseEventData.selectedObject = submitObject;
				ExecuteEvents.Execute(submitObject, baseEventData, ExecuteEvents.submitHandler);
				submitObject = null;
			}
		}
	}
}
