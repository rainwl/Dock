using UnityEngine;
using System;
using Unity.Collections;
using UnityEngine.UI;

namespace Dock
{
	/// <summary>此组件允许您使用手指拖动手势相对于相机平移当前游戏对象.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTranslate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate")]
	public class LeanDragTranslate : MonoBehaviour
	{
        
		#region 组件默认字段
        public LeanFingerFilter Use = new LeanFingerFilter(true);//用于查找用于此组件的手指的方法.LeanFingerFilter to see more.
		[Tooltip("The camera the translation will be calculated using.\n\nNone = MainCamera.")]
		public Camera Camera;//移动时，将会计算使用的摄像机\n\nNone = MainCamera
		[Tooltip("The sensitivity of the translation.\n\n1 = Default.\n2 = Double.")]
		public float Sensitivity = 1.0f;//移动的敏感度，默认为1，双倍为2
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		public float Dampening = -1.0f;//如果您希望此组件随时间平滑地更改，则这允许您控制更改达到其目标值的速度-1 = Instantly change.1 = Slowly change.10 = Quickly change.
		[Tooltip("This allows you to control how much momenum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Dampening</b> to be above 0.")]
		[Range(0.0f, 1.0f)]
		public float Inertia;//阻尼值。这允许您控制释放所有拖动手指时保留的momenum量
		[HideInInspector]
		[SerializeField]
		private Vector3 remainingTranslation;
		#endregion

		

		#region 三个方法：添加、移除手指，移除所有手指
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
        #endregion
        #region 更新此组件手指的方法：如果当前没有selectable方法，自动添加
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
        #endregion
        protected virtual void Update()
		{			
			var oldPosition = transform.localPosition;// 储存当前位置为oldPosition
			var fingers = Use.GetFingers();//找到我们要用的手指
			var screenDelta = LeanGesture.GetScreenDelta(fingers);//根据这些手指计算屏幕增量值
			if (screenDelta != Vector2.zero)
			{
				// 执行移动方法
				if (transform is RectTransform)
				{
					TranslateUI(screenDelta);//2D移动方法
				}
				else
				{
					Translate(screenDelta);//3D移动方法
					
				}
			}
			// Increment增量
			remainingTranslation += transform.localPosition - oldPosition;
			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			// Dampen remainingDelta阻尼剩余增量
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);
			// Shift this transform by the change in delta通过增量的变化来移动此变换
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;
			if (fingers.Count == 0 && Inertia > 0.0f && Dampening > 0.0f)
			{
				newRemainingTranslation = Vector3.Lerp(newRemainingTranslation, remainingTranslation, Inertia);
			}
			// 用阻尼值更新remainingDelta
			remainingTranslation = newRemainingTranslation;
			
		}
        #region 2D移动方法
        /// <summary>
        /// 2D移动方法
        /// </summary>
        /// <param name="screenDelta"></param>
        private void TranslateUI(Vector2 screenDelta)
		{
			var camera = Camera;

			if (camera == null)
			{
				var canvas = transform.GetComponentInParent<Canvas>();

				if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
				{
					camera = canvas.worldCamera;
				}
			}

			// Screen position of the transform
			var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, transform.position);

			// Add the deltaPosition
			screenPoint += screenDelta * Sensitivity;

			// Convert back to world space
			var worldPoint = default(Vector3);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, camera, out worldPoint) == true)
			{
				transform.position = worldPoint;
			}
		}
        #endregion

        /// <summary>
        /// 3D移动方法
        /// </summary>
        /// <param name="screenDelta"></param>
        private void Translate(Vector2 screenDelta)
		{
			var camera = LeanTouch.GetCamera(Camera, gameObject); //确保摄像机存在

			if (camera != null)//如果摄像机存在，那么屏幕坐标转换为世界坐标
			{
				var screenPoint = camera.WorldToScreenPoint(transform.position);//变换的屏幕位置
				screenPoint += (Vector3)screenDelta * Sensitivity;//添加deltaPosition
				transform.position = camera.ScreenToWorldPoint(screenPoint);//转换回世界空间
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
				//找不到摄像头。将相机标记为MainCamera，或在此组件中设置一个
			}
			
		}
        

        
	}
}