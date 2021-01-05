using UnityEngine;

public class GrenadeSkeeballTarget : MonoBehaviour
{
	public float m_score_y;

	private bool m_isDead;

	private GrenadeSkeeballGame game;

	public int Points;

	public void SetGame(GrenadeSkeeballGame g)
	{
		game = g;
	}

	private void Update()
	{
		if (!m_isDead && base.transform.position.y < m_score_y)
		{
			m_isDead = true;
			game.AddPoints(Points);
			if (Points == 500)
			{
				game.SpawnPointsFX(base.transform.position, GrenadeSkeeballGame.PointsParticleType.Blue);
			}
			else if (Points == 1000)
			{
				game.SpawnPointsFX(base.transform.position, GrenadeSkeeballGame.PointsParticleType.Yellow);
			}
			else
			{
				game.SpawnPointsFX(base.transform.position, GrenadeSkeeballGame.PointsParticleType.Red);
			}
			Invoke("Die", 2f);
		}
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
