using UnityEngine;

namespace Dock
{
	/// <summary>���������ָ����ʱ����Զ����ٴ���Ϸ����.
	/// ע�⣺��������ֶ����������Ϸ������ô��������������ֱ�ӵ���DestroyNow����.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDestroy")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Destroy")]
	public class LeanDestroy : MonoBehaviour
	{
		/// <summary>����Ϸ��������ǰʣ�������.
		/// -1 = �����ֶ�����DestroyNow����.</summary>
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