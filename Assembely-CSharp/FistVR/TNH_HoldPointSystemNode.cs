using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TNH_HoldPointSystemNode : MonoBehaviour
	{
		public enum SystemNodeMode
		{
			Passive,
			Hacking,
			Analyzing,
			Indentified
		}

		public TNH_HoldPoint HoldPoint;

		public Transform NodeCenter;

		public float ActivationRange = 1f;

		private bool m_hasActivated;

		private bool m_hasInitiatedHold;

		public Text Display;

		private SystemNodeMode m_mode;

		private float m_timeTilTargRotationChange = 1f;

		private float m_changeSpeed = 0.2f;

		private float m_rotateSpeed = 20f;

		private float m_curNodeHeight = 1.5f;

		private float m_tarNodeHeight = 1.5f;

		private float m_curTextHeight = 2f;

		private float m_tarTextHeight = 2.4f;

		private Quaternion m_targRotation;

		public Renderer NodeRenderer;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Passive;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Hacking;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Analyzing;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Indentified;

		public Transform DisplayTrans;

		private float m_activateAmount;

		public AudioEvent AUDEvent_HoldActivate;

		public GameObject VFX_Activate;

		public GameObject VFX_HoldActivate;

		private void Start()
		{
			m_targRotation = NodeCenter.rotation;
		}

		public void SetDisplayString(string s)
		{
			Display.text = s;
		}

		public void SetNodeMode(SystemNodeMode m)
		{
			m_mode = m;
			switch (m_mode)
			{
			case SystemNodeMode.Passive:
				NodeRenderer.material.SetColor("_RimColor", Color_Passive);
				break;
			case SystemNodeMode.Hacking:
				NodeRenderer.material.SetColor("_RimColor", Color_Hacking);
				break;
			case SystemNodeMode.Analyzing:
				NodeRenderer.material.SetColor("_RimColor", Color_Analyzing);
				break;
			case SystemNodeMode.Indentified:
				NodeRenderer.material.SetColor("_RimColor", Color_Indentified);
				break;
			}
		}

		private void Update()
		{
			Vector3 position = GM.CurrentPlayerBody.Head.position;
			Vector3 position2 = DisplayTrans.position;
			Vector3 forward = position - position2;
			forward.y = 0f;
			DisplayTrans.rotation = Quaternion.LookRotation(forward, Vector3.up);
			if (!m_hasActivated)
			{
				float num = Vector3.Distance(NodeCenter.position, GM.CurrentPlayerBody.LeftHand.transform.position);
				float num2 = Vector3.Distance(NodeCenter.position, GM.CurrentPlayerBody.RightHand.transform.position);
				if (num < ActivationRange || num2 < ActivationRange)
				{
					m_hasActivated = true;
					Object.Instantiate(VFX_Activate, NodeCenter.position, NodeCenter.rotation);
					SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AUDEvent_HoldActivate, base.transform.position);
				}
			}
			if (m_hasActivated && !m_hasInitiatedHold)
			{
				m_activateAmount += Time.deltaTime;
				if (m_activateAmount > 0.5f)
				{
					m_hasInitiatedHold = true;
					Object.Instantiate(VFX_HoldActivate, NodeCenter.position, NodeCenter.rotation);
					HoldPoint.BeginHoldChallenge();
				}
			}
			switch (m_mode)
			{
			case SystemNodeMode.Passive:
				m_changeSpeed = 2f;
				m_rotateSpeed = 20f;
				m_tarNodeHeight = 1.5f;
				m_tarTextHeight = 2f;
				break;
			case SystemNodeMode.Hacking:
				m_changeSpeed = 2f;
				m_rotateSpeed = 50f;
				m_tarNodeHeight = 1.5f;
				m_tarTextHeight = 2f;
				break;
			case SystemNodeMode.Analyzing:
				m_changeSpeed = 5f;
				m_rotateSpeed = 500f;
				m_tarNodeHeight = 2.5f;
				m_tarTextHeight = 2.4f;
				break;
			case SystemNodeMode.Indentified:
				m_changeSpeed = 5f;
				m_rotateSpeed = 500f;
				m_tarNodeHeight = 2.5f;
				m_tarTextHeight = 2.4f;
				break;
			}
			m_timeTilTargRotationChange -= Time.deltaTime * m_changeSpeed;
			if (m_timeTilTargRotationChange <= 0f)
			{
				PickNewTarg();
				m_timeTilTargRotationChange = 1f;
			}
			m_curNodeHeight = Mathf.Lerp(m_curNodeHeight, m_tarNodeHeight, Time.deltaTime * 4f);
			NodeCenter.localPosition = new Vector3(0f, m_curNodeHeight, 0f);
			m_curTextHeight = Mathf.Lerp(m_curTextHeight, m_tarTextHeight, Time.deltaTime * 4f);
			DisplayTrans.localPosition = new Vector3(0f, m_curTextHeight, 0f);
			Quaternion rotation = Quaternion.RotateTowards(NodeCenter.rotation, m_targRotation, m_rotateSpeed * Time.deltaTime);
			NodeCenter.rotation = rotation;
		}

		private void PickNewTarg()
		{
			switch (m_mode)
			{
			case SystemNodeMode.Passive:
				m_targRotation = Quaternion.Slerp(NodeCenter.rotation, Random.rotation, 0.3f);
				break;
			case SystemNodeMode.Hacking:
				m_targRotation = Quaternion.Slerp(NodeCenter.rotation, Random.rotation, 0.4f);
				break;
			case SystemNodeMode.Analyzing:
				m_targRotation = Quaternion.Slerp(NodeCenter.rotation, Random.rotation, 0.7f);
				break;
			case SystemNodeMode.Indentified:
				m_targRotation = Quaternion.Slerp(NodeCenter.rotation, Random.rotation, 0.7f);
				break;
			}
		}
	}
}
