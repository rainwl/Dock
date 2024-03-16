using UnityEngine;
using System.Collections.Generic;

namespace Dock
{
	/// <summary>���������ָ������ƹ켣.
	/// NOTE: ��Ҫ��������LeanTouch.RecordFingers�ļ�����.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTrail")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Trail")]
	public class LeanDragTrail : MonoBehaviour
	{
		// �����Ϊÿ����ָ�洢���������
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public LineRenderer Line;
			public float        Age;
			public float        Width;
		}

		/// <summary>���ڲ������ڴ��������ָ�ķ���. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>��������Ļ�������������֮��ת���ķ���.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.FixedDistance, Physics.DefaultRaycastLayers, 10.0f);

		/// <summary>��������Ⱦ�켣����Ԥ��.</summary>
		public LineRenderer Prefab;

		/// <summary>��켣���������.
		/// -1 = ������.</summary>
		public int MaxTrails = -1;

		/// <summary>�ɿ���ָ��ÿ���ۼ���ʧ��Ҫ������.</summary>
		public float FadeTime = 1.0f;

		/// <summary>The color of the trail start.</summary>
		public Color StartColor = Color.white;

		/// <summary>The color of the trail end.</summary>
		public Color EndColor = Color.white;

		// �⽫�洢fingers��LineRendererʵ��֮�����������
		[SerializeField]
		[HideInInspector]
		protected List<FingerData> fingerDatas = new List<FingerData>();

		/// <summary>����ѽ�Use����ΪManuallyAddedFingers������Ե��ô˷����ֶ������ָ.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>����ѽ�Use����ΪManuallyAddedFingers������Ե��ô˷����ֶ��Ƴ���ָ.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>����ѽ�Use����ΪManuallyAddedFingers������Ե��ô˷����ֶ��Ƴ�������ָ.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			// �ҵ�����Ҫ�õ���ָ
			var fingers = Use.GetFingers(true);

			for (var i = 0; i < fingers.Count; i++)
			{
				var finger = fingers[i];

				if (LeanFingerData.Exists(fingerDatas, finger) == false)
				{
					// Too many active links?
					if (MaxTrails >= 0 && LeanFingerData.Count(fingerDatas) >= MaxTrails)
					{
						continue;
					}

					if (Prefab != null)
					{
						// Spawn and activate
						var clone = Instantiate(Prefab);

						clone.gameObject.SetActive(true);

						// Register with FingerData
						var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

						fingerData.Line  = clone;
						fingerData.Age   = 0.0f;
						fingerData.Width = Prefab.widthMultiplier;
					}
				}
			}

			// Update all FingerData
			for (var i = fingerDatas.Count - 1; i >= 0; i--)
			{
				var fingerData = fingerDatas[i];

				if (fingerData.Line != null)
				{
					UpdateLine(fingerData, fingerData.Finger, fingerData.Line);

					if (fingerData.Age >= FadeTime)
					{
						Destroy(fingerData.Line.gameObject);

						fingerDatas.RemoveAt(i);
					}
				}
				else
				{
					fingerDatas.RemoveAt(i);
				}
			}
		}

		protected virtual void UpdateLine(FingerData fingerData, LeanFinger finger, LineRenderer line)
		{
			var color0 = StartColor;
			var color1 =   EndColor;

			if (finger != null)
			{
				// Reserve one point for each snapshot
				line.positionCount = finger.Snapshots.Count;

				// Loop through all snapshots
				for (var i = 0; i < finger.Snapshots.Count; i++)
				{
					var snapshot = finger.Snapshots[i];

					// Get the world postion of this snapshot
					var worldPoint = ScreenDepth.Convert(snapshot.ScreenPosition, gameObject);

					// Write position
					line.SetPosition(i, worldPoint);
				}
			}
			else
			{
				fingerData.Age += Time.deltaTime;

				var alpha = Mathf.InverseLerp(FadeTime, 0.0f, fingerData.Age);

				color0.a *= alpha;
				color1.a *= alpha;
			}

			line.startColor = color0;
			line.endColor   = color1;
		}

		protected virtual void HandleFingerUp(LeanFinger finger)
		{
			var link = LeanFingerData.Find(fingerDatas, finger);

			if (link != null)
			{
				link.Finger = null; // The line will gradually fade out in Update
			}
		}
	}
}