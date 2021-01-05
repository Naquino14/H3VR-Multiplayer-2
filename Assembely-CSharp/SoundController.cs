using UnityEngine;

public class SoundController : MonoBehaviour
{
	public AudioClip[] _audioClips;

	public int _audioChannels = 10;

	public float _masterVol = 0.5f;

	public float _soundVol = 1f;

	public float _musicVol = 1f;

	public bool _linearRollOff;

	public AudioSource[] channels;

	public int channel;

	public AudioSource[] _musicChannels;

	public int _musicChannel;

	private float _currentMusicVol;

	private float _fadeTo;

	public static SoundController instance;

	public void OnApplicationQuit()
	{
		instance = null;
	}

	public void Start()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		AddChannels();
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	public void StopMusic(bool fade)
	{
		PlayMusic(null, 0f, 1f, fade);
	}

	public void FadeUpMusic()
	{
		if (_musicChannels[_musicChannel].volume < _fadeTo)
		{
			_musicChannels[_musicChannel].volume += 0.0025f;
		}
		else
		{
			CancelInvoke("FadeUpMusic");
		}
	}

	public void FadeDownMusic()
	{
		int num = 0;
		if (_musicChannel == 0)
		{
			num = 1;
		}
		if (_musicChannels[num].volume > 0f)
		{
			_musicChannels[num].volume -= 0.0025f;
			return;
		}
		_musicChannels[num].Stop();
		CancelInvoke("FadeDownMusic");
	}

	public void UpdateMusicVolume()
	{
		for (int i = 0; i < 2; i++)
		{
			_musicChannels[i].volume = _currentMusicVol * _masterVol * _musicVol;
		}
	}

	public void AddChannels()
	{
		channels = new AudioSource[_audioChannels];
		_musicChannels = new AudioSource[2];
		if (channels.Length <= _audioChannels)
		{
			for (int i = 0; i < _audioChannels; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.AddComponent<AudioSource>();
				gameObject.name = "AudioChannel " + i;
				gameObject.transform.parent = base.transform;
				channels[i] = gameObject.GetComponent<AudioSource>();
				if (_linearRollOff)
				{
					channels[i].rolloffMode = AudioRolloffMode.Linear;
				}
			}
		}
		for (int j = 0; j < 2; j++)
		{
			GameObject gameObject2 = new GameObject();
			gameObject2.AddComponent<AudioSource>();
			gameObject2.name = "MusicChannel " + j;
			gameObject2.transform.parent = base.transform;
			_musicChannels[j] = gameObject2.GetComponent<AudioSource>();
			_musicChannels[j].loop = true;
			_musicChannels[j].volume = 0f;
			if (_linearRollOff)
			{
				_musicChannels[j].rolloffMode = AudioRolloffMode.Linear;
			}
		}
	}

	public void PlayMusic(AudioClip clip, float volume, float pitch, bool fade)
	{
		if (!fade)
		{
			_musicChannels[_musicChannel].volume = 0f;
		}
		if (_musicChannel == 0)
		{
			_musicChannel = 1;
		}
		else
		{
			_musicChannel = 0;
		}
		_currentMusicVol = volume;
		_musicChannels[_musicChannel].clip = clip;
		if (fade)
		{
			_fadeTo = volume * _masterVol * _musicVol;
			InvokeRepeating("FadeUpMusic", 0.01f, 0.01f);
			InvokeRepeating("FadeDownMusic", 0.01f, 0.01f);
		}
		else
		{
			_musicChannels[_musicChannel].volume = volume * _masterVol * _musicVol;
		}
		_musicChannels[_musicChannel].GetComponent<AudioSource>().pitch = pitch;
		_musicChannels[_musicChannel].GetComponent<AudioSource>().Play();
	}

	public void Play(int audioClipIndex, float volume, float pitch)
	{
		if (channel < channels.Length - 1)
		{
			channel++;
		}
		else
		{
			channel = 0;
		}
		if (audioClipIndex < _audioClips.Length)
		{
			channels[channel].clip = _audioClips[audioClipIndex];
			channels[channel].GetComponent<AudioSource>().volume = volume * _masterVol * _soundVol;
			channels[channel].GetComponent<AudioSource>().pitch = pitch;
			channels[channel].GetComponent<AudioSource>().Play();
		}
	}

	public void Play(AudioClip clip, float volume, float pitch, Vector3 position)
	{
		if (channel < channels.Length - 1)
		{
			channel++;
		}
		else
		{
			channel = 0;
		}
		channels[channel].clip = clip;
		channels[channel].GetComponent<AudioSource>().volume = volume * _masterVol * _soundVol;
		channels[channel].GetComponent<AudioSource>().pitch = pitch;
		channels[channel].transform.position = position;
		channels[channel].GetComponent<AudioSource>().Play();
	}

	public void Play(AudioClip clip, float volume, float pitch)
	{
		if (channel < channels.Length - 1)
		{
			channel++;
		}
		else
		{
			channel = 0;
		}
		channels[channel].clip = clip;
		channels[channel].GetComponent<AudioSource>().volume = volume * _masterVol * _soundVol;
		channels[channel].GetComponent<AudioSource>().pitch = pitch;
		channels[channel].GetComponent<AudioSource>().Play();
	}

	public void StopAll()
	{
		for (int i = 0; i < channels.Length; i++)
		{
			channels[i].Stop();
		}
	}
}
