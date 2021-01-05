using UnityEngine;

public class FilterTestQuaternion : MonoBehaviour
{
	public Transform noisyTransform;

	public Transform filteredTransform;

	private Quaternion quat;

	private OneEuroFilter<Quaternion> rotationFilter;

	public bool filterOn = true;

	public float filterFrequency = 120f;

	public float filterMinCutoff = 1f;

	public float filterBeta;

	public float filterDcutoff = 1f;

	public float noiseAmount = 1f;

	private float timer;

	private void Start()
	{
		quat = default(Quaternion);
		quat.eulerAngles = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
		rotationFilter = new OneEuroFilter<Quaternion>(filterFrequency);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 2f)
		{
			quat.eulerAngles = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
			timer = 0f;
		}
		noisyTransform.rotation = Quaternion.Slerp(noisyTransform.rotation, PerturbedRotation(quat), 0.5f * Time.deltaTime);
		if (filterOn)
		{
			rotationFilter.UpdateParams(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff);
			filteredTransform.rotation = rotationFilter.Filter(noisyTransform.rotation);
		}
		else
		{
			filteredTransform.rotation = noisyTransform.rotation;
		}
	}

	private Quaternion PerturbedRotation(Quaternion _rotation)
	{
		Quaternion quaternion = new Quaternion(Random.value * noiseAmount - noiseAmount / 2f, Random.value * noiseAmount - noiseAmount / 2f, Random.value * noiseAmount - noiseAmount / 2f, Random.value * noiseAmount - noiseAmount / 2f);
		quaternion.x *= Time.deltaTime;
		quaternion.y *= Time.deltaTime;
		quaternion.z *= Time.deltaTime;
		quaternion.w *= Time.deltaTime;
		return quaternion * _rotation;
	}
}
