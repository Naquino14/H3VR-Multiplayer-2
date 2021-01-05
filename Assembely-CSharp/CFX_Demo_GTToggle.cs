using UnityEngine;

public class CFX_Demo_GTToggle : MonoBehaviour
{
	public Texture Normal;

	public Texture Hover;

	public Color NormalColor = new Color32(128, 128, 128, 128);

	public Color DisabledColor = new Color32(128, 128, 128, 48);

	public bool State = true;

	public string Callback;

	public GameObject Receiver;

	private Rect CollisionRect;

	private bool Over;

	private GUIText Label;

	private void Awake()
	{
		CollisionRect = GetComponent<GUITexture>().GetScreenRect(Camera.main);
		Label = GetComponentInChildren<GUIText>();
		UpdateTexture();
	}

	private void Update()
	{
		if (CollisionRect.Contains(Input.mousePosition))
		{
			Over = true;
			if (Input.GetMouseButtonDown(0))
			{
				OnClick();
			}
		}
		else
		{
			Over = false;
			GetComponent<GUITexture>().color = NormalColor;
		}
		UpdateTexture();
	}

	private void OnClick()
	{
		State = !State;
		Receiver.SendMessage(Callback);
	}

	private void UpdateTexture()
	{
		Color color = ((!State) ? DisabledColor : NormalColor);
		if (Over)
		{
			GetComponent<GUITexture>().texture = Hover;
		}
		else
		{
			GetComponent<GUITexture>().texture = Normal;
		}
		GetComponent<GUITexture>().color = color;
		if (Label != null)
		{
			Label.color = color * 1.75f;
		}
	}
}
