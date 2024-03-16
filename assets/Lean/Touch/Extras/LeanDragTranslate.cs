using UnityEngine;
using System;
using Unity.Collections;
using UnityEngine.UI;

namespace Dock
{
	/// <summary>�����������ʹ����ָ�϶�������������ƽ�Ƶ�ǰ��Ϸ����.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTranslate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate")]
	public class LeanDragTranslate : MonoBehaviour
	{
        
		#region ���Ĭ���ֶ�
        public LeanFingerFilter Use = new LeanFingerFilter(true);//���ڲ������ڴ��������ָ�ķ���.LeanFingerFilter to see more.
		[Tooltip("The camera the translation will be calculated using.\n\nNone = MainCamera.")]
		public Camera Camera;//�ƶ�ʱ���������ʹ�õ������\n\nNone = MainCamera
		[Tooltip("The sensitivity of the translation.\n\n1 = Default.\n2 = Double.")]
		public float Sensitivity = 1.0f;//�ƶ������жȣ�Ĭ��Ϊ1��˫��Ϊ2
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		public float Dampening = -1.0f;//�����ϣ���������ʱ��ƽ���ظ��ģ��������������Ƹ��Ĵﵽ��Ŀ��ֵ���ٶ�-1 = Instantly change.1 = Slowly change.10 = Quickly change.
		[Tooltip("This allows you to control how much momenum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Dampening</b> to be above 0.")]
		[Range(0.0f, 1.0f)]
		public float Inertia;//����ֵ���������������ͷ������϶���ָʱ������momenum��
		[HideInInspector]
		[SerializeField]
		private Vector3 remainingTranslation;
		#endregion

		

		#region ������������ӡ��Ƴ���ָ���Ƴ�������ָ
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
        #endregion
        #region ���´������ָ�ķ����������ǰû��selectable�������Զ����
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
			var oldPosition = transform.localPosition;// ���浱ǰλ��ΪoldPosition
			var fingers = Use.GetFingers();//�ҵ�����Ҫ�õ���ָ
			var screenDelta = LeanGesture.GetScreenDelta(fingers);//������Щ��ָ������Ļ����ֵ
			if (screenDelta != Vector2.zero)
			{
				// ִ���ƶ�����
				if (transform is RectTransform)
				{
					TranslateUI(screenDelta);//2D�ƶ�����
				}
				else
				{
					Translate(screenDelta);//3D�ƶ�����
					
				}
			}
			// Increment����
			remainingTranslation += transform.localPosition - oldPosition;
			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			// Dampen remainingDelta����ʣ������
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);
			// Shift this transform by the change in deltaͨ�������ı仯���ƶ��˱任
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;
			if (fingers.Count == 0 && Inertia > 0.0f && Dampening > 0.0f)
			{
				newRemainingTranslation = Vector3.Lerp(newRemainingTranslation, remainingTranslation, Inertia);
			}
			// ������ֵ����remainingDelta
			remainingTranslation = newRemainingTranslation;
			
		}
        #region 2D�ƶ�����
        /// <summary>
        /// 2D�ƶ�����
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
        /// 3D�ƶ�����
        /// </summary>
        /// <param name="screenDelta"></param>
        private void Translate(Vector2 screenDelta)
		{
			var camera = LeanTouch.GetCamera(Camera, gameObject); //ȷ�����������

			if (camera != null)//�����������ڣ���ô��Ļ����ת��Ϊ��������
			{
				var screenPoint = camera.WorldToScreenPoint(transform.position);//�任����Ļλ��
				screenPoint += (Vector3)screenDelta * Sensitivity;//���deltaPosition
				transform.position = camera.ScreenToWorldPoint(screenPoint);//ת��������ռ�
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
				//�Ҳ�������ͷ����������ΪMainCamera�����ڴ����������һ��
			}
			
		}
        

        
	}
}