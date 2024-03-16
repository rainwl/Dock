using UnityEngine;

namespace Dock
{
	/// <summary>�������������ѡ��ʱ���ĸ��ӵ���ǰ��Ϸ�������Ⱦ��������������Ⱦ��������ɫ.</summary>
	[RequireComponent(typeof(Renderer))]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSelectableRendererColor")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Renderer Color")]
	public class LeanSelectableRendererColor : LeanSelectableBehaviour
	{
		#region ���Ը�������
		public Material otherMaterial;
		private Material oriMaterial;
		private MeshRenderer meshRender;
        private new void  Start()
        {
			meshRender = this.GetComponent<MeshRenderer>();
			oriMaterial = this.GetComponent<MeshRenderer>().material;
        }
		private void ChangeMat()
        {
			meshRender.material = otherMaterial;
        }
		private void BackMat()
        {
			meshRender.material = oriMaterial;
        }
        #endregion


        /// <summary>�Ӳ������Զ���ȡDefaultColor��</summary>
        [Tooltip("�Ӳ������Զ���ȡDefaultColor��")]
		public bool AutoGetDefaultColor;

		/// <summary>ָ�������ʵ�Ĭ����ɫ.</summary>
		[Tooltip("The default color given to the materials.")]
		public Color DefaultColor = Color.white;
		
		
		/// <summary>ѡ��ʱΪ����ָ������ɫ.</summary>
		[Tooltip("The color given to the materials when selected.")]
		public Color SelectedColor = Color.green;
		

		/// <summary>����һ��ʼ��Ӧ�ñ���¡��</summary>
		[Tooltip("Should the materials get cloned at the start?")]
		public bool CloneMaterials = true;

		[System.NonSerialized]
		private Renderer cachedRenderer;//������Ⱦ��

		protected virtual void Awake()
		{
			if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();//��ȡRenderer

			if (AutoGetDefaultColor == true)
			{
				var material0 = cachedRenderer.sharedMaterial;

				if (material0 != null)
				{
					DefaultColor = material0.color;
				}
			}

			if (CloneMaterials == true)
			{
				cachedRenderer.sharedMaterials = cachedRenderer.materials;
			}
		}

		protected override void OnSelect(LeanFinger finger)
		{
			//ChangeColor(SelectedColor);
			ChangeMat();

		}

		protected override void OnDeselect()
		{
			//ChangeColor(DefaultColor);
			BackMat();
		}

		private void ChangeColor(Color color)
		{
			if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();

			var materials = cachedRenderer.sharedMaterials;

			for (var i = materials.Length - 1; i >= 0; i--)
			{
				materials[i].color = color;
			}
		}
		
		
	}
}