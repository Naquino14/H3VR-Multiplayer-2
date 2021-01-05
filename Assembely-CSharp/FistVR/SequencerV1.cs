using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SequencerV1 : MonoBehaviour
	{
		public FVRLever DistanceLever;

		public KeyCodeDisplay Display;

		public Text ScoreText;

		public Transform InitialPoint;

		private float m_floatScore;

		private int m_score;

		private bool m_isStarted;

		private float m_distance;

		private int m_numTargets;

		private int m_numWaves;

		private float m_wavelength;

		private float m_wavecooldown = 1f;

		private bool m_isCooldownTicking;

		private bool m_isShootTicking;

		private int m_wavesLeft;

		private float m_timeLeft;

		private float m_cooldownLeft;

		public GameObject SimpleTarget;

		private List<GameObject> Targets = new List<GameObject>();

		public Vector3 MinPos;

		public Vector3 MaxPos;

		public Renderer GreenLight;

		private AudioSource aud;

		private int m_possibleScore;

		public float FloatScore
		{
			get
			{
				return m_floatScore;
			}
			set
			{
				m_floatScore = value;
				Score = Mathf.RoundToInt(m_floatScore);
			}
		}

		public int Score
		{
			get
			{
				return m_score;
			}
			set
			{
				m_score = value;
				ScoreText.text = m_score + "/" + m_possibleScore;
			}
		}

		public void Awake()
		{
			aud = GetComponent<AudioSource>();
		}

		public void AddTargetPoints(int i)
		{
			FloatScore += i;
		}

		public void BeginSequence(int inty)
		{
			Reset(0);
			if (m_isStarted)
			{
				return;
			}
			string myText = Display.MyText;
			if (myText.Length >= 3)
			{
				m_numTargets = int.Parse(myText[0].ToString());
				m_numWaves = int.Parse(myText[1].ToString());
				string text = myText[2].ToString();
				if (myText.Length == 4)
				{
					text += myText[3];
				}
				m_wavelength = int.Parse(text);
				if (!(m_wavelength < 1f) && m_numTargets != 0 && m_numWaves != 0)
				{
					m_distance = DistanceLever.GetLeverValue();
					m_isCooldownTicking = false;
					m_isShootTicking = false;
					m_wavesLeft = m_numWaves;
					m_timeLeft = m_wavelength;
					m_cooldownLeft = m_wavecooldown;
					FloatScore = 0f;
					m_isStarted = true;
					m_isCooldownTicking = true;
					m_possibleScore = 10 * m_numWaves * m_numTargets;
					GreenLight.material.SetFloat("_EmissionWeight", 1f);
				}
			}
		}

		public void Reset(int inty)
		{
			FloatScore = 0f;
			m_isStarted = false;
			for (int num = Targets.Count - 1; num >= 0; num--)
			{
				if (Targets[num] != null)
				{
					Object.Destroy(Targets[num]);
				}
			}
			Targets.Clear();
			m_isCooldownTicking = false;
			m_isShootTicking = false;
			GreenLight.material.SetFloat("_EmissionWeight", 0f);
		}

		private void Update()
		{
			if (m_isStarted && m_isCooldownTicking)
			{
				if (m_cooldownLeft > 0f)
				{
					m_cooldownLeft -= Time.deltaTime;
				}
				else
				{
					m_isCooldownTicking = false;
					m_isShootTicking = true;
					m_timeLeft = m_wavelength + 2f;
					aud.PlayOneShot(aud.clip, 0.5f);
					m_wavesLeft--;
					for (int i = 0; i < m_numTargets; i++)
					{
						GameObject gameObject = Object.Instantiate(SimpleTarget, InitialPoint.position, InitialPoint.rotation);
						MaskedTarget component = gameObject.GetComponent<MaskedTarget>();
						Targets.Add(gameObject);
						Vector3 vector = new Vector3(Random.Range(MinPos.x, MaxPos.x), Random.Range(MinPos.y, MaxPos.y), Mathf.Lerp(MinPos.z, MaxPos.z, m_distance));
						component.Init(InitialPoint.position, vector, 1f, Vector3.forward, (base.transform.position - vector).normalized, this);
					}
				}
			}
			if (!m_isShootTicking)
			{
				return;
			}
			if (m_timeLeft > 0f)
			{
				m_timeLeft -= Time.deltaTime;
				return;
			}
			for (int num = Targets.Count - 1; num >= 0; num--)
			{
				if (Targets[num] != null)
				{
					Object.Destroy(Targets[num]);
				}
			}
			Targets.Clear();
			if (m_wavesLeft > 0)
			{
				m_isShootTicking = false;
				m_isCooldownTicking = true;
				m_cooldownLeft = m_wavecooldown;
			}
			else
			{
				GreenLight.material.SetFloat("_EmissionWeight", 0f);
			}
		}
	}
}
