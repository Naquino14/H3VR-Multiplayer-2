using UnityEngine;

public struct OnHitInfo
{
	public Vector2 uv;

	public int score;

	public OnHitInfo(Vector2 uv, int score)
	{
		this.uv = uv;
		this.score = score;
	}
}
