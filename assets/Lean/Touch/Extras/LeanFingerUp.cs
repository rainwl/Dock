using UnityEngine;
using UnityEngine.Events;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Dock
{
	/// <summary>This component calls the OnUp event when a finger stops touching the screen.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerUp")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Up")]
	public class LeanFingerUp : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		public bool IgnoreStartedOverGui = true;

		/// <summary>Ignore fingers with IsOverGui?</summary>
		public bool IgnoreIsOverGui;

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		public LeanSelectable RequiredSelectable;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [FSA("onUp")] [FSA("OnUp")] [SerializeField] private LeanFingerEvent onFinger;
		
		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point based on the ScreenDepth settings.</summary>
		public Vector3Event OnPosition { get { if (onPosition == null) onPosition = new Vector3Event(); return onPosition; } } [SerializeField] private Vector3Event onPosition;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			RequiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			if (onFinger != null)
			{
				onFinger.Invoke(finger);
			}

			if (onPosition != null)
			{
				var position = ScreenDepth.Convert(finger.StartScreenPosition, gameObject);

				onPosition.Invoke(position);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Dock.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanFingerUp))]
	public class LeanFingerUp_Inspector : Lean.Common.LeanInspector<LeanFingerUp>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("IgnoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("IgnoreIsOverGui", "Ignore fingers with IsOverGui?");
			Draw("RequiredSelectable", "Do nothing if this LeanSelectable isn't selected?");

			EditorGUILayout.Separator();

			var usedA = Any(t => t.OnFinger.GetPersistentEventCount() > 0);
			var usedB = Any(t => t.OnPosition.GetPersistentEventCount() > 0);

			EditorGUI.BeginDisabledGroup(usedA && usedB);
				showUnusedEvents = EditorGUILayout.Foldout(showUnusedEvents, "Show Unused Events");
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFinger");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
				Draw("onPosition");
			}
		}
	}
}
#endif