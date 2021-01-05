// Decompiled with JetBrains decompiler
// Type: GameObjectSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    this.pages = (int) Mathf.Ceil((float) ((this.particles.Length - 1) / this.maxButtons));
    if (this.spawnOnAwake)
    {
      this.counter = 0;
      this.ReplaceGO(this.particles[this.counter]);
      this.Info(this.particles[this.counter], this.counter);
    }
    if ((double) this.autoChangeDelay <= 0.0)
      return;
    this.InvokeRepeating("NextModel", this.autoChangeDelay, this.autoChangeDelay);
  }

  public void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      if (this._active)
      {
        this._active = false;
        if ((Object) this.image != (Object) null)
          this.image.enabled = false;
      }
      else
      {
        this._active = true;
        if ((Object) this.image != (Object) null)
          this.image.enabled = true;
      }
    }
    if (Input.GetKeyDown(KeyCode.RightArrow))
      this.NextModel();
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      --this.counter;
      if (this.counter < 0)
        this.counter = this.particles.Length - 1;
      this.ReplaceGO(this.particles[this.counter]);
      this.Info(this.particles[this.counter], this.counter + 1);
    }
    if (Input.GetKeyDown(KeyCode.UpArrow) && this.materials.Length > 0)
    {
      ++this.matCounter;
      if (this.matCounter > this.materials.Length - 1)
        this.matCounter = 0;
      this.material = this.materials[this.matCounter];
      if ((Object) this.currentGO != (Object) null)
        this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
    }
    if (Input.GetKeyDown(KeyCode.DownArrow) && this.materials.Length > 0)
    {
      --this.matCounter;
      if (this.matCounter < 0)
        this.matCounter = this.materials.Length - 1;
      this.material = this.materials[this.matCounter];
      if ((Object) this.currentGO != (Object) null)
        this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
    }
    if (Input.GetKeyDown(KeyCode.B))
    {
      ++this.colorCounter;
      if (this.colorCounter > this.cameraColors.Length - 1)
        this.colorCounter = 0;
    }
    Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, this.cameraColors[this.colorCounter], Time.deltaTime * 3f);
  }

  public void NextModel()
  {
    ++this.counter;
    if (this.counter > this.particles.Length - 1)
      this.counter = 0;
    this.ReplaceGO(this.particles[this.counter]);
    this.Info(this.particles[this.counter], this.counter + 1);
  }

  public void OnGUI()
  {
    if (this.showInfo)
      GUI.Label(new Rect((float) ((double) Screen.width * 0.5 - 250.0), 20f, 500f, 500f), this.currentGOInfo, this.bigStyle);
    if (this._active)
    {
      if (this.particles.Length > this.maxButtons)
      {
        if (GUI.Button(new Rect(20f, (float) ((this.maxButtons + 1) * 18), 75f, 18f), "Prev"))
        {
          if (this.page > 0)
            --this.page;
          else
            this.page = this.pages;
        }
        if (GUI.Button(new Rect(95f, (float) ((this.maxButtons + 1) * 18), 75f, 18f), "Next"))
        {
          if (this.page < this.pages)
            ++this.page;
          else
            this.page = 0;
        }
        GUI.Label(new Rect(60f, (float) ((this.maxButtons + 2) * 18), 150f, 22f), "Page" + (object) (this.page + 1) + " / " + (object) (this.pages + 1));
      }
      this.showInfo = GUI.Toggle(new Rect(185f, 20f, 75f, 25f), this.showInfo, "Info");
      int num = this.particles.Length - this.page * this.maxButtons;
      if (num > this.maxButtons)
        num = this.maxButtons;
      for (int index = 0; index < num; ++index)
      {
        string text = this.particles[index + this.page * this.maxButtons].transform.name;
        if (this.removeTextFromButton != string.Empty)
          text = text.Replace(this.removeTextFromButton, string.Empty);
        if (GUI.Button(new Rect(20f, (float) (index * 18 + 18), 150f, 18f), text))
        {
          if ((Object) this.currentGO != (Object) null)
            Object.Destroy((Object) this.currentGO);
          GameObject go = Object.Instantiate<GameObject>(this.particles[index + this.page * this.maxButtons]);
          this.currentGO = go;
          this.counter = index + this.page * this.maxButtons;
          if ((Object) this.material != (Object) null)
            go.GetComponent<Renderer>().sharedMaterial = this.material;
          this.Info(go, index + this.page * this.maxButtons + 1);
        }
      }
      for (int index = 0; index < this.materials.Length; ++index)
      {
        string text = this.materials[index].name;
        if (this.removeTextFromMaterialButton != string.Empty)
          text = text.Replace(this.removeTextFromMaterialButton, string.Empty);
        if (GUI.Button(new Rect(20f, (float) ((this.maxButtons + index + 4) * 18), 150f, 18f), text))
        {
          this.material = this.materials[index];
          if ((Object) this.currentGO != (Object) null)
            this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
        }
      }
    }
    if (!((Object) this.image != (Object) null))
      return;
    Rect pixelInset = this.image.pixelInset;
    pixelInset.x = (float) (Screen.width - this.image.texture.width);
    this.image.pixelInset = pixelInset;
  }

  public void Info(GameObject go, int i)
  {
    if ((Object) go.GetComponent<ParticleSystem>() != (Object) null)
    {
      this.PlayPS(go.GetComponent<ParticleSystem>(), i);
      this.InfoPS(go.GetComponent<ParticleSystem>(), i);
    }
    else
      this.InfoGO(go, i);
  }

  public void ReplaceGO(GameObject _go)
  {
    if ((Object) this.currentGO != (Object) null)
      Object.Destroy((Object) this.currentGO);
    GameObject gameObject = Object.Instantiate<GameObject>(_go);
    this.currentGO = gameObject;
    if (!((Object) this.material != (Object) null))
      return;
    gameObject.GetComponent<Renderer>().sharedMaterial = this.material;
  }

  public void PlayPS(ParticleSystem _ps, int _nr)
  {
    Time.timeScale = 1f;
    _ps.Play();
  }

  public void InfoGO(GameObject _ps, int _nr)
  {
    this.currentGOInfo = string.Empty + (object) _nr + "/" + (object) this.particles.Length + "\n" + _ps.gameObject.name + "\n" + (object) (_ps.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3) + " Tris";
    this.currentGOInfo = this.currentGOInfo.Replace("_", " ");
  }

  public void Instructions()
  {
    this.currentGOInfo += "\n\nUse mouse wheel to zoom \nClick and hold to rotate\nPress Space to show or hide menu\nPress Up and Down arrows to cycle materials\nPress B to cycle background colors";
    this.currentGOInfo = this.currentGOInfo.Replace("(Clone)", string.Empty);
  }

  public void InfoPS(ParticleSystem _ps, int _nr)
  {
    this.currentGOInfo = "System: " + (object) _nr + "/" + (object) this.particles.Length + "\n" + _ps.gameObject.name + "\n\n";
    this.currentGOInfo = this.currentGOInfo.Replace("_", " ");
    this.currentGOInfo = this.currentGOInfo.Replace("(Clone)", string.Empty);
  }
}
