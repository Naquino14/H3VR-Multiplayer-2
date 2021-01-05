using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeekBarCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEventSystemHandler
{
	public MediaPlayerCtrl m_srcVideo;

	public Slider m_srcSlider;

	public float m_fDragTime = 0.2f;

	private bool m_bActiveDrag = true;

	private bool m_bUpdate = true;

	private float m_fDeltaTime;

	private float m_fLastValue;

	private float m_fLastSetValue;

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_bActiveDrag)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > m_fDragTime)
			{
				m_bActiveDrag = true;
				m_fDeltaTime = 0f;
			}
		}
		if (m_bUpdate && m_srcVideo != null && m_srcSlider != null)
		{
			m_srcSlider.value = m_srcVideo.GetSeekBarValue();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("OnPointerEnter:");
		m_bUpdate = false;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("OnPointerExit:");
		m_bUpdate = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		m_srcVideo.SetSeekBarValue(m_srcSlider.value);
	}

	public void OnDrag(PointerEventData eventData)
	{
		Debug.Log("OnDrag:" + eventData);
		if (!m_bActiveDrag)
		{
			m_fLastValue = m_srcSlider.value;
			return;
		}
		m_srcVideo.SetSeekBarValue(m_srcSlider.value);
		m_fLastSetValue = m_srcSlider.value;
		m_bActiveDrag = false;
	}
}
