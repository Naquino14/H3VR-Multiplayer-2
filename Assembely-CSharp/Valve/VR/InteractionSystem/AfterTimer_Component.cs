using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[Serializable]
	public class AfterTimer_Component : MonoBehaviour
	{
		private Action callback;

		private float triggerTime;

		private bool timerActive;

		private bool triggerOnEarlyDestroy;

		public void Init(float _time, Action _callback, bool earlydestroy)
		{
			triggerTime = _time;
			callback = _callback;
			triggerOnEarlyDestroy = earlydestroy;
			timerActive = true;
			StartCoroutine(Wait());
		}

		private IEnumerator Wait()
		{
			yield return new WaitForSeconds(triggerTime);
			timerActive = false;
			callback();
			UnityEngine.Object.Destroy(this);
		}

		private void OnDestroy()
		{
			if (timerActive)
			{
				StopCoroutine(Wait());
				timerActive = false;
				if (triggerOnEarlyDestroy)
				{
					callback();
				}
			}
		}
	}
}
