using UnityEngine;

public class MediaPlayerFullScreenCtrl : MonoBehaviour
{
	public GameObject m_objVideo;

	private int m_iOrgWidth;

	private int m_iOrgHeight;

	private void Start()
	{
		Resize();
	}

	private void Update()
	{
		if (m_iOrgWidth != Screen.width)
		{
			Resize();
		}
		if (m_iOrgHeight != Screen.height)
		{
			Resize();
		}
	}

	private void Resize()
	{
		m_iOrgWidth = Screen.width;
		m_iOrgHeight = Screen.height;
		float num = (float)m_iOrgHeight / (float)m_iOrgWidth;
		m_objVideo.transform.localScale = new Vector3(20f / num, 20f / num, 1f);
		m_objVideo.transform.GetComponent<MediaPlayerCtrl>().Resize();
	}
}
