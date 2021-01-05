// Decompiled with JetBrains decompiler
// Type: WFX_Demo_New
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
  private List<string> groundTextures = new List<string>((IEnumerable<string>) new string[4]
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
    List<GameObject> gameObjectList = new List<GameObject>();
    int childCount = this.transform.childCount;
    for (int index = 0; index < childCount; ++index)
    {
      GameObject gameObject = this.transform.GetChild(index).gameObject;
      gameObjectList.Add(gameObject);
    }
    gameObjectList.AddRange((IEnumerable<GameObject>) this.AdditionalEffects);
    this.ParticleExamples = gameObjectList.ToArray();
    this.defaultCamPosition = Camera.main.transform.position;
    this.defaultCamRotation = Camera.main.transform.rotation;
    this.StartCoroutine("CheckForDeletedParticles");
    this.UpdateUI();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftArrow))
      this.prevParticle();
    else if (Input.GetKeyDown(KeyCode.RightArrow))
      this.nextParticle();
    else if (Input.GetKeyDown(KeyCode.Delete))
      this.destroyParticles();
    if (Input.GetMouseButtonDown(0))
    {
      RaycastHit hitInfo = new RaycastHit();
      if (this.groundCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
      {
        GameObject gameObject = this.spawnParticle();
        if (!gameObject.name.StartsWith("WFX_MF"))
          gameObject.transform.position = hitInfo.point + gameObject.transform.position;
      }
    }
    float axis = Input.GetAxis("Mouse ScrollWheel");
    if ((double) axis != 0.0)
      Camera.main.transform.Translate(Vector3.forward * ((double) axis >= 0.0 ? 1f : -1f), Space.Self);
    if (!Input.GetMouseButtonDown(2))
      return;
    Camera.main.transform.position = this.defaultCamPosition;
    Camera.main.transform.rotation = this.defaultCamRotation;
  }

  private void OnToggleGround() => this.groundRenderer.enabled = !this.groundRenderer.enabled;

  private void OnToggleCamera() => CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;

  private void OnToggleSlowMo()
  {
    this.slowMo = !this.slowMo;
    if (this.slowMo)
      Time.timeScale = 0.33f;
    else
      Time.timeScale = 1f;
  }

  private void OnPreviousEffect() => this.prevParticle();

  private void OnNextEffect() => this.nextParticle();

  private void UpdateUI()
  {
    this.EffectLabel.text = this.ParticleExamples[this.exampleIndex].name;
    this.EffectIndexLabel.text = string.Format("{0}/{1}", (object) (this.exampleIndex + 1).ToString("00"), (object) this.ParticleExamples.Length.ToString("00"));
  }

  public GameObject spawnParticle()
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.ParticleExamples[this.exampleIndex]);
    gameObject.transform.position = new Vector3(0.0f, gameObject.transform.position.y, 0.0f);
    gameObject.SetActive(true);
    if (gameObject.name.StartsWith("WFX_MF"))
    {
      gameObject.transform.parent = this.ParticleExamples[this.exampleIndex].transform.parent;
      gameObject.transform.localPosition = this.ParticleExamples[this.exampleIndex].transform.localPosition;
      gameObject.transform.localRotation = this.ParticleExamples[this.exampleIndex].transform.localRotation;
    }
    else if (gameObject.name.Contains("Hole"))
      gameObject.transform.parent = this.bulletholes.transform;
    ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
    if ((Object) component != (Object) null && component.loop)
    {
      component.gameObject.AddComponent<CFX_AutoStopLoopedEffect>();
      component.gameObject.AddComponent<CFX_AutoDestructShuriken>();
    }
    this.onScreenParticles.Add(gameObject);
    return gameObject;
  }

  [DebuggerHidden]
  private IEnumerator CheckForDeletedParticles() => (IEnumerator) new WFX_Demo_New.\u003CCheckForDeletedParticles\u003Ec__Iterator0()
  {
    \u0024this = this
  };

  private void prevParticle()
  {
    --this.exampleIndex;
    if (this.exampleIndex < 0)
      this.exampleIndex = this.ParticleExamples.Length - 1;
    this.UpdateUI();
    this.showHideStuff();
  }

  private void nextParticle()
  {
    ++this.exampleIndex;
    if (this.exampleIndex >= this.ParticleExamples.Length)
      this.exampleIndex = 0;
    this.UpdateUI();
    this.showHideStuff();
  }

  private void destroyParticles()
  {
    for (int index = this.onScreenParticles.Count - 1; index >= 0; --index)
    {
      if ((Object) this.onScreenParticles[index] != (Object) null)
        Object.Destroy((Object) this.onScreenParticles[index]);
      this.onScreenParticles.RemoveAt(index);
    }
  }

  private void prevTexture()
  {
    int index = this.groundTextures.IndexOf(this.groundTextureStr) - 1;
    if (index < 0)
      index = this.groundTextures.Count - 1;
    this.groundTextureStr = this.groundTextures[index];
    this.selectMaterial();
  }

  private void nextTexture()
  {
    int index = this.groundTextures.IndexOf(this.groundTextureStr) + 1;
    if (index >= this.groundTextures.Count)
      index = 0;
    this.groundTextureStr = this.groundTextures[index];
    this.selectMaterial();
  }

  private void selectMaterial()
  {
    switch (this.groundTextureStr)
    {
      case "Concrete":
        this.ground.GetComponent<Renderer>().material = this.concrete;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.concreteWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.concreteWall;
        break;
      case "Wood":
        this.ground.GetComponent<Renderer>().material = this.wood;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.woodWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.woodWall;
        break;
      case "Metal":
        this.ground.GetComponent<Renderer>().material = this.metal;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.metalWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.metalWall;
        break;
      case "Checker":
        this.ground.GetComponent<Renderer>().material = this.checker;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.checkerWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.checkerWall;
        break;
    }
  }

  private void showHideStuff()
  {
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_MF Spr"))
    {
      this.m4.GetComponent<Renderer>().enabled = true;
      Camera.main.transform.position = new Vector3(-2.482457f, 3.263842f, -0.004924395f);
      Camera.main.transform.eulerAngles = new Vector3(20f, 90f, 0.0f);
    }
    else
      this.m4.GetComponent<Renderer>().enabled = false;
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_MF FPS"))
      this.m4fps.GetComponent<Renderer>().enabled = true;
    else
      this.m4fps.GetComponent<Renderer>().enabled = false;
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_BImpact"))
    {
      this.walls.SetActive(true);
      foreach (Renderer componentsInChild in this.bulletholes.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = true;
    }
    else
    {
      this.walls.SetActive(false);
      foreach (Renderer componentsInChild in this.bulletholes.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = false;
    }
    if (this.ParticleExamples[this.exampleIndex].name.Contains("Wood"))
    {
      this.groundTextureStr = "Wood";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Concrete"))
    {
      this.groundTextureStr = "Concrete";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Metal"))
    {
      this.groundTextureStr = "Metal";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Dirt") || this.ParticleExamples[this.exampleIndex].name.Contains("Sand") || this.ParticleExamples[this.exampleIndex].name.Contains("SoftBody"))
    {
      this.groundTextureStr = "Checker";
      this.selectMaterial();
    }
    else
    {
      if (!(this.ParticleExamples[this.exampleIndex].name == "WFX_Explosion"))
        return;
      this.groundTextureStr = "Checker";
      this.selectMaterial();
    }
  }
}
