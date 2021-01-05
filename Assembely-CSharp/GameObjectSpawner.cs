using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
	public GameObject[] particles;

	public Material[] materials;

	public Color[] cameraColors;

	public int maxButtons = 10;

	public bool spawnOnAwake = true;

	public bool showInfo;

	public string removeTextFromButton;

	public string removeTextFromMaterialButton;

	public float autoChangeDelay;

	public GUITexture image;

	private int page;

	private int pages;

	private string currentGOInfo;

	private GameObject currentGO;

	private Color currentColor;

	private bool isPS;

	private Material material;

	private bool _active = true;

	private int counter = -1;

	private int matCounter = -1;

	private int colorCounter;

	public GUIStyle bigStyle;

	public void Start()
	{
		pages = (int)Mathf.Ceil((particles.Length - 1) / maxButtons);
		if (spawnOnAwake)
		{
			counter = 0;
			ReplaceGO(particles[counter]);
			Info(particles[counter], counter);
		}
		if (autoChangeDelay > 0f)
		{
			InvokeRepeating("NextModel", autoChangeDelay, autoChangeDelay);
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (_active)
			{
				_active = false;
				if (image != null)
				{
					image.enabled = false;
				}
			}
			else
			{
				_active = true;
				if (image != null)
				{
					image.enabled = true;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			NextModel();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			counter--;
			if (counter < 0)
			{
				counter = particles.Length - 1;
			}
			ReplaceGO(particles[counter]);
			Info(particles[counter], counter + 1);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && materials.Length > 0)
		{
			matCounter++;
			if (matCounter > materials.Length - 1)
			{
				matCounter = 0;
			}
			material = materials[matCounter];
			if (currentGO != null)
			{
				currentGO.GetComponent<Renderer>().sharedMaterial = material;
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && materials.Length > 0)
		{
			matCounter--;
			if (matCounter < 0)
			{
				matCounter = materials.Length - 1;
			}
			material = materials[matCounter];
			if (currentGO != null)
			{
				currentGO.GetComponent<Renderer>().sharedMaterial = material;
			}
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			colorCounter++;
			if (colorCounter > cameraColors.Length - 1)
			{
				colorCounter = 0;
			}
		}
		Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, cameraColors[colorCounter], Time.deltaTime * 3f);
	}

	public void NextModel()
	{
		counter++;
		if (counter > particles.Length - 1)
		{
			counter = 0;
		}
		ReplaceGO(particles[counter]);
		Info(particles[counter], counter + 1);
	}

	public void OnGUI()
	{
		if (showInfo)
		{
			GUI.Label(new Rect((float)Screen.width * 0.5f - 250f, 20f, 500f, 500f), currentGOInfo, bigStyle);
		}
		if (_active)
		{
			if (particles.Length > maxButtons)
			{
				if (GUI.Button(new Rect(20f, (maxButtons + 1) * 18, 75f, 18f), "Prev"))
				{
					if (page > 0)
					{
						page--;
					}
					else
					{
						page = pages;
					}
				}
				if (GUI.Button(new Rect(95f, (maxButtons + 1) * 18, 75f, 18f), "Next"))
				{
					if (page < pages)
					{
						page++;
					}
					else
					{
						page = 0;
					}
				}
				GUI.Label(new Rect(60f, (maxButtons + 2) * 18, 150f, 22f), "Page" + (page + 1) + " / " + (pages + 1));
			}
			showInfo = GUI.Toggle(new Rect(185f, 20f, 75f, 25f), showInfo, "Info");
			int num = particles.Length - page * maxButtons;
			if (num > maxButtons)
			{
				num = maxButtons;
			}
			for (int i = 0; i < num; i++)
			{
				string text = particles[i + page * maxButtons].transform.name;
				if (removeTextFromButton != string.Empty)
				{
					text = text.Replace(removeTextFromButton, string.Empty);
				}
				if (GUI.Button(new Rect(20f, i * 18 + 18, 150f, 18f), text))
				{
					if (currentGO != null)
					{
						Object.Destroy(currentGO);
					}
					GameObject gameObject = (currentGO = Object.Instantiate(particles[i + page * maxButtons]));
					counter = i + page * maxButtons;
					if (material != null)
					{
						gameObject.GetComponent<Renderer>().sharedMaterial = material;
					}
					Info(gameObject, i + page * maxButtons + 1);
				}
			}
			for (int j = 0; j < materials.Length; j++)
			{
				string text2 = materials[j].name;
				if (removeTextFromMaterialButton != string.Empty)
				{
					text2 = text2.Replace(removeTextFromMaterialButton, string.Empty);
				}
				if (GUI.Button(new Rect(20f, (maxButtons + j + 4) * 18, 150f, 18f), text2))
				{
					material = materials[j];
					if (currentGO != null)
					{
						currentGO.GetComponent<Renderer>().sharedMaterial = material;
					}
				}
			}
		}
		if (image != null)
		{
			Rect pixelInset = image.pixelInset;
			pixelInset.x = Screen.width - image.texture.width;
			image.pixelInset = pixelInset;
		}
	}

	public void Info(GameObject go, int i)
	{
		if (go.GetComponent<ParticleSystem>() != null)
		{
			PlayPS(go.GetComponent<ParticleSystem>(), i);
			InfoPS(go.GetComponent<ParticleSystem>(), i);
		}
		else
		{
			InfoGO(go, i);
		}
	}

	public void ReplaceGO(GameObject _go)
	{
		if (currentGO != null)
		{
			Object.Destroy(currentGO);
		}
		GameObject gameObject = (currentGO = Object.Instantiate(_go));
		if (material != null)
		{
			gameObject.GetComponent<Renderer>().sharedMaterial = material;
		}
	}

	public void PlayPS(ParticleSystem _ps, int _nr)
	{
		Time.timeScale = 1f;
		_ps.Play();
	}

	public void InfoGO(GameObject _ps, int _nr)
	{
		currentGOInfo = string.Empty + _nr + "/" + particles.Length + "\n" + _ps.gameObject.name + "\n" + _ps.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3 + " Tris";
		currentGOInfo = currentGOInfo.Replace("_", " ");
	}

	public void Instructions()
	{
		currentGOInfo += "\n\nUse mouse wheel to zoom \nClick and hold to rotate\nPress Space to show or hide menu\nPress Up and Down arrows to cycle materials\nPress B to cycle background colors";
		currentGOInfo = currentGOInfo.Replace("(Clone)", string.Empty);
	}

	public void InfoPS(ParticleSystem _ps, int _nr)
	{
		currentGOInfo = "System: " + _nr + "/" + particles.Length + "\n" + _ps.gameObject.name + "\n\n";
		currentGOInfo = currentGOInfo.Replace("_", " ");
		currentGOInfo = currentGOInfo.Replace("(Clone)", string.Empty);
	}
}
