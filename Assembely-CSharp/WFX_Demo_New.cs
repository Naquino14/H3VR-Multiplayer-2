using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFX_Demo_New : MonoBehaviour
{
	public GUIText EffectLabel;

	public GUIText EffectIndexLabel;

	public Renderer groundRenderer;

	public Collider groundCollider;

	public GameObject[] AdditionalEffects;

	public GameObject ground;

	public GameObject walls;

	public GameObject bulletholes;

	public GameObject m4;

	public GameObject m4fps;

	public Material wood;

	public Material concrete;

	public Material metal;

	public Material checker;

	public Material woodWall;

	public Material concreteWall;

	public Material metalWall;

	public Material checkerWall;

	private string groundTextureStr = "Checker";

	private List<string> groundTextures = new List<string>(new string[4]
	{
		"Concrete",
		"Wood",
		"Metal",
		"Checker"
	});

	private GameObject[] ParticleExamples;

	private int exampleIndex;

	private bool slowMo;

	private Vector3 defaultCamPosition;

	private Quaternion defaultCamRotation;

	private List<GameObject> onScreenParticles = new List<GameObject>();

	private void Awake()
	{
		List<GameObject> list = new List<GameObject>();
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			list.Add(gameObject);
		}
		list.AddRange(AdditionalEffects);
		ParticleExamples = list.ToArray();
		defaultCamPosition = Camera.main.transform.position;
		defaultCamRotation = Camera.main.transform.rotation;
		StartCoroutine("CheckForDeletedParticles");
		UpdateUI();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			prevParticle();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextParticle();
		}
		else if (Input.GetKeyDown(KeyCode.Delete))
		{
			destroyParticles();
		}
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitInfo = default(RaycastHit);
			if (groundCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
			{
				GameObject gameObject = spawnParticle();
				if (!gameObject.name.StartsWith("WFX_MF"))
				{
					gameObject.transform.position = hitInfo.point + gameObject.transform.position;
				}
			}
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			Camera.main.transform.Translate(Vector3.forward * ((!(axis < 0f)) ? 1f : (-1f)), Space.Self);
		}
		if (Input.GetMouseButtonDown(2))
		{
			Camera.main.transform.position = defaultCamPosition;
			Camera.main.transform.rotation = defaultCamRotation;
		}
	}

	private void OnToggleGround()
	{
		groundRenderer.enabled = !groundRenderer.enabled;
	}

	private void OnToggleCamera()
	{
		CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;
	}

	private void OnToggleSlowMo()
	{
		slowMo = !slowMo;
		if (slowMo)
		{
			Time.timeScale = 0.33f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void OnPreviousEffect()
	{
		prevParticle();
	}

	private void OnNextEffect()
	{
		nextParticle();
	}

	private void UpdateUI()
	{
		EffectLabel.text = ParticleExamples[exampleIndex].name;
		EffectIndexLabel.text = string.Format("{0}/{1}", (exampleIndex + 1).ToString("00"), ParticleExamples.Length.ToString("00"));
	}

	public GameObject spawnParticle()
	{
		GameObject gameObject = Object.Instantiate(ParticleExamples[exampleIndex]);
		gameObject.transform.position = new Vector3(0f, gameObject.transform.position.y, 0f);
		gameObject.SetActive(value: true);
		if (gameObject.name.StartsWith("WFX_MF"))
		{
			gameObject.transform.parent = ParticleExamples[exampleIndex].transform.parent;
			gameObject.transform.localPosition = ParticleExamples[exampleIndex].transform.localPosition;
			gameObject.transform.localRotation = ParticleExamples[exampleIndex].transform.localRotation;
		}
		else if (gameObject.name.Contains("Hole"))
		{
			gameObject.transform.parent = bulletholes.transform;
		}
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		if (component != null && component.loop)
		{
			component.gameObject.AddComponent<CFX_AutoStopLoopedEffect>();
			component.gameObject.AddComponent<CFX_AutoDestructShuriken>();
		}
		onScreenParticles.Add(gameObject);
		return gameObject;
	}

	private IEnumerator CheckForDeletedParticles()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			for (int num = onScreenParticles.Count - 1; num >= 0; num--)
			{
				if (onScreenParticles[num] == null)
				{
					onScreenParticles.RemoveAt(num);
				}
			}
		}
	}

	private void prevParticle()
	{
		exampleIndex--;
		if (exampleIndex < 0)
		{
			exampleIndex = ParticleExamples.Length - 1;
		}
		UpdateUI();
		showHideStuff();
	}

	private void nextParticle()
	{
		exampleIndex++;
		if (exampleIndex >= ParticleExamples.Length)
		{
			exampleIndex = 0;
		}
		UpdateUI();
		showHideStuff();
	}

	private void destroyParticles()
	{
		for (int num = onScreenParticles.Count - 1; num >= 0; num--)
		{
			if (onScreenParticles[num] != null)
			{
				Object.Destroy(onScreenParticles[num]);
			}
			onScreenParticles.RemoveAt(num);
		}
	}

	private void prevTexture()
	{
		int num = groundTextures.IndexOf(groundTextureStr);
		num--;
		if (num < 0)
		{
			num = groundTextures.Count - 1;
		}
		groundTextureStr = groundTextures[num];
		selectMaterial();
	}

	private void nextTexture()
	{
		int num = groundTextures.IndexOf(groundTextureStr);
		num++;
		if (num >= groundTextures.Count)
		{
			num = 0;
		}
		groundTextureStr = groundTextures[num];
		selectMaterial();
	}

	private void selectMaterial()
	{
		switch (groundTextureStr)
		{
		case "Concrete":
			ground.GetComponent<Renderer>().material = concrete;
			walls.transform.GetChild(0).GetComponent<Renderer>().material = concreteWall;
			walls.transform.GetChild(1).GetComponent<Renderer>().material = concreteWall;
			break;
		case "Wood":
			ground.GetComponent<Renderer>().material = wood;
			walls.transform.GetChild(0).GetComponent<Renderer>().material = woodWall;
			walls.transform.GetChild(1).GetComponent<Renderer>().material = woodWall;
			break;
		case "Metal":
			ground.GetComponent<Renderer>().material = metal;
			walls.transform.GetChild(0).GetComponent<Renderer>().material = metalWall;
			walls.transform.GetChild(1).GetComponent<Renderer>().material = metalWall;
			break;
		case "Checker":
			ground.GetComponent<Renderer>().material = checker;
			walls.transform.GetChild(0).GetComponent<Renderer>().material = checkerWall;
			walls.transform.GetChild(1).GetComponent<Renderer>().material = checkerWall;
			break;
		}
	}

	private void showHideStuff()
	{
		if (ParticleExamples[exampleIndex].name.StartsWith("WFX_MF Spr"))
		{
			m4.GetComponent<Renderer>().enabled = true;
			Camera.main.transform.position = new Vector3(-2.482457f, 3.263842f, -0.004924395f);
			Camera.main.transform.eulerAngles = new Vector3(20f, 90f, 0f);
		}
		else
		{
			m4.GetComponent<Renderer>().enabled = false;
		}
		if (ParticleExamples[exampleIndex].name.StartsWith("WFX_MF FPS"))
		{
			m4fps.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			m4fps.GetComponent<Renderer>().enabled = false;
		}
		if (ParticleExamples[exampleIndex].name.StartsWith("WFX_BImpact"))
		{
			walls.SetActive(value: true);
			Renderer[] componentsInChildren = bulletholes.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
		else
		{
			walls.SetActive(value: false);
			Renderer[] componentsInChildren2 = bulletholes.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer renderer2 in array2)
			{
				renderer2.enabled = false;
			}
		}
		if (ParticleExamples[exampleIndex].name.Contains("Wood"))
		{
			groundTextureStr = "Wood";
			selectMaterial();
		}
		else if (ParticleExamples[exampleIndex].name.Contains("Concrete"))
		{
			groundTextureStr = "Concrete";
			selectMaterial();
		}
		else if (ParticleExamples[exampleIndex].name.Contains("Metal"))
		{
			groundTextureStr = "Metal";
			selectMaterial();
		}
		else if (ParticleExamples[exampleIndex].name.Contains("Dirt") || ParticleExamples[exampleIndex].name.Contains("Sand") || ParticleExamples[exampleIndex].name.Contains("SoftBody"))
		{
			groundTextureStr = "Checker";
			selectMaterial();
		}
		else if (ParticleExamples[exampleIndex].name == "WFX_Explosion")
		{
			groundTextureStr = "Checker";
			selectMaterial();
		}
	}
}
