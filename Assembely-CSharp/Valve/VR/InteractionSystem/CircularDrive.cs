using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class CircularDrive : MonoBehaviour
	{
		public enum Axis_t
		{
			XAxis,
			YAxis,
			ZAxis
		}

		[Tooltip("The axis around which the circular drive will rotate in local space")]
		public Axis_t axisOfRotation;

		[Tooltip("Child GameObject which has the Collider component to initiate interaction, only needs to be set if there is more than one Collider child")]
		public Collider childCollider;

		[Tooltip("A LinearMapping component to drive, if not specified one will be dynamically added to this GameObject")]
		public LinearMapping linearMapping;

		[Tooltip("If true, the drive will stay manipulating as long as the button is held down, if false, it will stop if the controller moves out of the collider")]
		public bool hoverLock;

		[Header("Limited Rotation")]
		[Tooltip("If true, the rotation will be limited to [minAngle, maxAngle], if false, the rotation is unlimited")]
		public bool limited;

		public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);

		public UnityEvent onFrozenDistanceThreshold;

		[Header("Limited Rotation Min")]
		[Tooltip("If limited is true, the specifies the lower limit, otherwise value is unused")]
		public float minAngle = -45f;

		[Tooltip("If limited, set whether drive will freeze its angle when the min angle is reached")]
		public bool freezeOnMin;

		[Tooltip("If limited, event invoked when minAngle is reached")]
		public UnityEvent onMinAngle;

		[Header("Limited Rotation Max")]
		[Tooltip("If limited is true, the specifies the upper limit, otherwise value is unused")]
		public float maxAngle = 45f;

		[Tooltip("If limited, set whether drive will freeze its angle when the max angle is reached")]
		public bool freezeOnMax;

		[Tooltip("If limited, event invoked when maxAngle is reached")]
		public UnityEvent onMaxAngle;

		[Tooltip("If limited is true, this forces the starting angle to be startAngle, clamped to [minAngle, maxAngle]")]
		public bool forceStart;

		[Tooltip("If limited is true and forceStart is true, the starting angle will be this, clamped to [minAngle, maxAngle]")]
		public float startAngle;

		[Tooltip("If true, the transform of the GameObject this component is on will be rotated accordingly")]
		public bool rotateGameObject = true;

		[Tooltip("If true, the path of the Hand (red) and the projected value (green) will be drawn")]
		public bool debugPath;

		[Tooltip("If debugPath is true, this is the maximum number of GameObjects to create to draw the path")]
		public int dbgPathLimit = 50;

		[Tooltip("If not null, the TextMesh will display the linear value and the angular value of this circular drive")]
		public TextMesh debugText;

		[Tooltip("The output angle value of the drive in degrees, unlimited will increase or decrease without bound, take the 360 modulus to find number of rotations")]
		public float outAngle;

		private Quaternion start;

		private Vector3 worldPlaneNormal = new Vector3(1f, 0f, 0f);

		private Vector3 localPlaneNormal = new Vector3(1f, 0f, 0f);

		private Vector3 lastHandProjected;

		private Color red = new Color(1f, 0f, 0f);

		private Color green = new Color(0f, 1f, 0f);

		private GameObject[] dbgHandObjects;

		private GameObject[] dbgProjObjects;

		private GameObject dbgObjectsParent;

		private int dbgObjectCount;

		private int dbgObjectIndex;

		private bool driving;

		private float minMaxAngularThreshold = 1f;

		private bool frozen;

		private float frozenAngle;

		private Vector3 frozenHandWorldPos = new Vector3(0f, 0f, 0f);

		private Vector2 frozenSqDistanceMinMaxThreshold = new Vector2(0f, 0f);

		private Hand handHoverLocked;

		private Interactable interactable;

		private GrabTypes grabbedWithType;

		private void Freeze(Hand hand)
		{
			frozen = true;
			frozenAngle = outAngle;
			frozenHandWorldPos = hand.hoverSphereTransform.position;
			frozenSqDistanceMinMaxThreshold.x = frozenDistanceMinMaxThreshold.x * frozenDistanceMinMaxThreshold.x;
			frozenSqDistanceMinMaxThreshold.y = frozenDistanceMinMaxThreshold.y * frozenDistanceMinMaxThreshold.y;
		}

		private void UnFreeze()
		{
			frozen = false;
			frozenHandWorldPos.Set(0f, 0f, 0f);
		}

		private void Awake()
		{
			interactable = GetComponent<Interactable>();
		}

		private void Start()
		{
			if (childCollider == null)
			{
				childCollider = GetComponentInChildren<Collider>();
			}
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
			if (linearMapping == null)
			{
				linearMapping = base.gameObject.AddComponent<LinearMapping>();
			}
			worldPlaneNormal = new Vector3(0f, 0f, 0f);
			worldPlaneNormal[(int)axisOfRotation] = 1f;
			localPlaneNormal = worldPlaneNormal;
			if ((bool)base.transform.parent)
			{
				worldPlaneNormal = base.transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormal).normalized;
			}
			if (limited)
			{
				start = Quaternion.identity;
				outAngle = base.transform.localEulerAngles[(int)axisOfRotation];
				if (forceStart)
				{
					outAngle = Mathf.Clamp(startAngle, minAngle, maxAngle);
				}
			}
			else
			{
				start = Quaternion.AngleAxis(base.transform.localEulerAngles[(int)axisOfRotation], localPlaneNormal);
				outAngle = 0f;
			}
			if ((bool)debugText)
			{
				debugText.alignment = TextAlignment.Left;
				debugText.anchor = TextAnchor.UpperLeft;
			}
			UpdateAll();
		}

		private void OnDisable()
		{
			if ((bool)handHoverLocked)
			{
				handHoverLocked.HideGrabHint();
				handHoverLocked.HoverUnlock(interactable);
				handHoverLocked = null;
			}
		}

		private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount)
		{
			if (hand != null)
			{
				int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0f, 1f, 100f, 900f);
				nCount = Mathf.Clamp(nCount, 1, 10);
				for (ushort i = 0; i < nCount; i = (ushort)(i + 1))
				{
					ushort duration = (ushort)Random.Range(100, nRangeMax);
					hand.TriggerHapticPulse(duration);
					yield return new WaitForSeconds(0.01f);
				}
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			hand.ShowGrabHint();
		}

		private void OnHandHoverEnd(Hand hand)
		{
			hand.HideGrabHint();
			if (driving && (bool)hand)
			{
				StartCoroutine(HapticPulses(hand, 1f, 10));
			}
			driving = false;
			handHoverLocked = null;
		}

		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			bool flag = !hand.IsGrabbingWithType(grabbedWithType);
			if (grabbedWithType == GrabTypes.None && grabStarting != 0)
			{
				grabbedWithType = grabStarting;
				lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);
				if (hoverLock)
				{
					hand.HoverLock(interactable);
					handHoverLocked = hand;
				}
				driving = true;
				ComputeAngle(hand);
				UpdateAll();
				hand.HideGrabHint();
			}
			else if (grabbedWithType != 0 && flag)
			{
				if (hoverLock)
				{
					hand.HoverUnlock(interactable);
					handHoverLocked = null;
				}
				driving = false;
				grabbedWithType = GrabTypes.None;
			}
			if (driving && !flag && hand.hoveringInteractable == interactable)
			{
				ComputeAngle(hand);
				UpdateAll();
			}
		}

		private Vector3 ComputeToTransformProjected(Transform xForm)
		{
			Vector3 normalized = (xForm.position - base.transform.position).normalized;
			Vector3 vector = new Vector3(0f, 0f, 0f);
			if (normalized.sqrMagnitude > 0f)
			{
				vector = Vector3.ProjectOnPlane(normalized, worldPlaneNormal).normalized;
			}
			else
			{
				Debug.LogFormat("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", base.gameObject.ToString());
			}
			if (debugPath && dbgPathLimit > 0)
			{
				DrawDebugPath(xForm, vector);
			}
			return vector;
		}

		private void DrawDebugPath(Transform xForm, Vector3 toTransformProjected)
		{
			if (dbgObjectCount == 0)
			{
				dbgObjectsParent = new GameObject("Circular Drive Debug");
				dbgHandObjects = new GameObject[dbgPathLimit];
				dbgProjObjects = new GameObject[dbgPathLimit];
				dbgObjectCount = dbgPathLimit;
				dbgObjectIndex = 0;
			}
			GameObject gameObject = null;
			if ((bool)dbgHandObjects[dbgObjectIndex])
			{
				gameObject = dbgHandObjects[dbgObjectIndex];
			}
			else
			{
				gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject.transform.SetParent(dbgObjectsParent.transform);
				dbgHandObjects[dbgObjectIndex] = gameObject;
			}
			gameObject.name = $"actual_{(int)((1f - red.r) * 10f)}";
			gameObject.transform.position = xForm.position;
			gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
			gameObject.gameObject.GetComponent<Renderer>().material.color = red;
			if (red.r > 0.1f)
			{
				red.r -= 0.1f;
			}
			else
			{
				red.r = 1f;
			}
			gameObject = null;
			if ((bool)dbgProjObjects[dbgObjectIndex])
			{
				gameObject = dbgProjObjects[dbgObjectIndex];
			}
			else
			{
				gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject.transform.SetParent(dbgObjectsParent.transform);
				dbgProjObjects[dbgObjectIndex] = gameObject;
			}
			gameObject.name = $"projed_{(int)((1f - green.g) * 10f)}";
			gameObject.transform.position = base.transform.position + toTransformProjected * 0.25f;
			gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
			gameObject.gameObject.GetComponent<Renderer>().material.color = green;
			if (green.g > 0.1f)
			{
				green.g -= 0.1f;
			}
			else
			{
				green.g = 1f;
			}
			dbgObjectIndex = (dbgObjectIndex + 1) % dbgObjectCount;
		}

		private void UpdateLinearMapping()
		{
			if (limited)
			{
				linearMapping.value = (outAngle - minAngle) / (maxAngle - minAngle);
			}
			else
			{
				float num = outAngle / 360f;
				linearMapping.value = num - Mathf.Floor(num);
			}
			UpdateDebugText();
		}

		private void UpdateGameObject()
		{
			if (rotateGameObject)
			{
				base.transform.localRotation = start * Quaternion.AngleAxis(outAngle, localPlaneNormal);
			}
		}

		private void UpdateDebugText()
		{
			if ((bool)debugText)
			{
				debugText.text = $"Linear: {linearMapping.value}\nAngle:  {outAngle}\n";
			}
		}

		private void UpdateAll()
		{
			UpdateLinearMapping();
			UpdateGameObject();
			UpdateDebugText();
		}

		private void ComputeAngle(Hand hand)
		{
			Vector3 vector = ComputeToTransformProjected(hand.hoverSphereTransform);
			if (vector.Equals(lastHandProjected))
			{
				return;
			}
			float num = Vector3.Angle(lastHandProjected, vector);
			if (!(num > 0f))
			{
				return;
			}
			if (frozen)
			{
				float sqrMagnitude = (hand.hoverSphereTransform.position - frozenHandWorldPos).sqrMagnitude;
				if (sqrMagnitude > frozenSqDistanceMinMaxThreshold.x)
				{
					outAngle = frozenAngle + Random.Range(-1f, 1f);
					float num2 = Util.RemapNumberClamped(sqrMagnitude, frozenSqDistanceMinMaxThreshold.x, frozenSqDistanceMinMaxThreshold.y, 0f, 1f);
					if (num2 > 0f)
					{
						StartCoroutine(HapticPulses(hand, num2, 10));
					}
					else
					{
						StartCoroutine(HapticPulses(hand, 0.5f, 10));
					}
					if (sqrMagnitude >= frozenSqDistanceMinMaxThreshold.y)
					{
						onFrozenDistanceThreshold.Invoke();
					}
				}
				return;
			}
			Vector3 normalized = Vector3.Cross(lastHandProjected, vector).normalized;
			float num3 = Vector3.Dot(worldPlaneNormal, normalized);
			float num4 = num;
			if (num3 < 0f)
			{
				num4 = 0f - num4;
			}
			if (limited)
			{
				float num5 = Mathf.Clamp(outAngle + num4, minAngle, maxAngle);
				if (outAngle == minAngle)
				{
					if (num5 > minAngle && num < minMaxAngularThreshold)
					{
						outAngle = num5;
						lastHandProjected = vector;
					}
				}
				else if (outAngle == maxAngle)
				{
					if (num5 < maxAngle && num < minMaxAngularThreshold)
					{
						outAngle = num5;
						lastHandProjected = vector;
					}
				}
				else if (num5 == minAngle)
				{
					outAngle = num5;
					lastHandProjected = vector;
					onMinAngle.Invoke();
					if (freezeOnMin)
					{
						Freeze(hand);
					}
				}
				else if (num5 == maxAngle)
				{
					outAngle = num5;
					lastHandProjected = vector;
					onMaxAngle.Invoke();
					if (freezeOnMax)
					{
						Freeze(hand);
					}
				}
				else
				{
					outAngle = num5;
					lastHandProjected = vector;
				}
			}
			else
			{
				outAngle += num4;
				lastHandProjected = vector;
			}
		}
	}
}
