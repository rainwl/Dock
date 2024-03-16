using UnityEngine;
using System.Collections.Generic;

namespace Dock
{
	/// <summary>此组件在手指后面绘制轨迹.
	/// NOTE: 这要求您启用LeanTouch.RecordFingers文件设置.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTrail")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Trail")]
	public class LeanDragTrail : MonoBehaviour
	{
		// 这个类为每根手指存储额外的数据
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public LineRenderer Line;
			public float        Age;
			public float        Width;
		}

		/// <summary>用于查找用于此组件的手指的方法. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>用于在屏幕坐标和世界坐标之间转换的方法.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.FixedDistance, Physics.DefaultRaycastLayers, 10.0f);

		/// <summary>将用于渲染轨迹的线预制.</summary>
		public LineRenderer Prefab;

		/// <summary>活动轨迹的最大数量.
		/// -1 = 无限制.</summary>
		public int MaxTrails = -1;

		/// <summary>松开手指后，每条痕迹消失需要多少秒.</summary>
		public float FadeTime = 1.0f;

		/// <summary>The color of the trail start.</summary>
		public Color StartColor = Color.white;

		/// <summary>The color of the trail end.</summary>
		public Color EndColor = Color.white;

		// 这将存储fingers和LineRenderer实例之间的所有链接
		[SerializeField]
		[HideInInspector]
		protected List<FingerData> fingerDatas = new List<FingerData>();

		/// <summary>如果已将Use设置为ManuallyAddedFingers，则可以调用此方法手动添加手指.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>如果已将Use设置为ManuallyAddedFingers，则可以调用此方法手动移除手指.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>如果已将Use设置为ManuallyAddedFingers，则可以调用此方法手动移除所有手指.</summary>
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
			// 找到我们要用的手指
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