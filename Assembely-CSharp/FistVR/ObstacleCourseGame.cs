using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ObstacleCourseGame : MonoBehaviour
	{
		public AudioSource AudioMusic;

		public AudioSource Audio2d;

		public AudioClip AudClip_BeginGame;

		public AudioClip AudClip_EndGame;

		public AudioClip AudClip_Penalty;

		private int m_numBuzzersHit;

		private int m_numTargetsHit;

		public GameObject TargetRoot;

		private float m_timer;

		private bool m_isPlaying;

		private bool m_hasPlayed;

		public Text Timer;

		private float m_headPenaltyCooldown = 1f;

		public void BeginGame()
		{
			if (!m_isPlaying && !m_hasPlayed)
			{
				m_isPlaying = true;
				m_hasPlayed = true;
				m_numBuzzersHit = 0;
				m_numTargetsHit = 0;
				TargetRoot.SetActive(value: true);
				if (AudioMusic != null)
				{
					AudioMusic.Play();
				}
			}
		}

		public void Update()
		{
			if (m_isPlaying)
			{
				m_timer += Time.deltaTime;
				Timer.text = "TIME - " + FloatToTime(m_timer, "#0:00.00");
				Timer.text += "\n";
				Text timer = Timer;
				timer.text = timer.text + "BUZZERS - " + m_numBuzzersHit + "/20";
				Timer.text += "\n";
				Text timer2 = Timer;
				timer2.text = timer2.text + "TARGETS - " + m_numTargetsHit + "/60";
				if (m_headPenaltyCooldown > 0f)
				{
					m_headPenaltyCooldown -= Time.deltaTime;
				}
			}
		}

		public void RegisterHeadPenalty()
		{
			if (m_isPlaying && m_headPenaltyCooldown <= 0f)
			{
				m_headPenaltyCooldown = 1f;
				m_timer += 10f;
				Audio2d.PlayOneShot(AudClip_Penalty, 0.5f);
			}
		}

		public void EndGame()
		{
			if (m_isPlaying && m_hasPlayed)
			{
				m_isPlaying = false;
				TargetRoot.SetActive(value: false);
				if (AudioMusic != null)
				{
					AudioMusic.Stop();
				}
			}
		}

		public void RegisterBuzzerTouch()
		{
			if (m_isPlaying && m_hasPlayed)
			{
				m_numBuzzersHit++;
			}
		}

		public void RegisterTargetHit()
		{
			if (m_isPlaying && m_hasPlayed)
			{
				m_numTargetsHit++;
			}
		}

		public string FloatToTime(float toConvert, string format)
		{
			return format switch
			{
				"00.0" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0.0" => $"{Mathf.Floor(toConvert) % 60f:#0}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"00.00" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"00.000" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#00.000" => $"{Mathf.Floor(toConvert) % 60f:#00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}", 
				"#00:00" => $"{Mathf.Floor(toConvert / 60f):#00}:{Mathf.Floor(toConvert) % 60f:00}", 
				"0:00.0" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0:00.0" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"0:00.00" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"#0:00.00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"0:00.000" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00.000" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				_ => "error", 
			};
		}
	}
}
