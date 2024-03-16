using UnityEngine;

namespace Dock
{
	/// <summary>此组件将在指定的时间后自动销毁此游戏对象.
	/// 注意：如果你想手动销毁这个游戏对象，那么禁用这个组件，并直接调用DestroyNow方法.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDestroy")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Destroy")]
	public class LeanDestroy : MonoBehaviour
	{
		/// <summary>此游戏对象销毁前剩余的秒数.
		/// -1 = 必须手动调用DestroyNow方法.</summary>
		[Tooltip("The amount of seconds remaining before this GameObject gets destroyed.\n\n-1 = You must manually call the DestroyNow method.")]
		public float Seconds = -1.0f;

		protected virtual void Update()
		{
			if (Seconds >= 0.0f)
			{
				Seconds -= Time.deltaTime;

				if (Seconds <= 0.0f)
				{
					DestroyNow();
				}
			}
		}

		/// <summary>You can manually call this method to destroy the current GameObject now.</summary>
		public void DestroyNow()
		{
			Destroy(gameObject);
		}
	}
}