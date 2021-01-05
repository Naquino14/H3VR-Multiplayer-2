using UnityEngine;

internal struct ProbeCullTrack : ITracked
{
	public bool Enabled;

	public BoundingSphere BoundingSphere;

	public ReflectionProbe Probe;

	public int Index
	{
		get;
		set;
	}
}
