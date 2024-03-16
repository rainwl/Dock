using UnityEngine;

namespace Dock
{
	/// <summary>此组件允许您在选中时更改附加到当前游戏对象的渲染器（例如网格渲染器）的颜色.</summary>
	[RequireComponent(typeof(Renderer))]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSelectableRendererColor")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Renderer Color")]
	public class LeanSelectableRendererColor : LeanSelectableBehaviour
	{
		#region 测试更换材质
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


        /// <summary>从材质中自动读取DefaultColor？</summary>
        [Tooltip("从材质中自动读取DefaultColor？")]
		public bool AutoGetDefaultColor;

		/// <summary>指定给材质的默认颜色.</summary>
		[Tooltip("The default color given to the materials.")]
		public Color DefaultColor = Color.white;
		
		
		/// <summary>选定时为材质指定的颜色.</summary>
		[Tooltip("The color given to the materials when selected.")]
		public Color SelectedColor = Color.green;
		

		/// <summary>材料一开始就应该被克隆吗？</summary>
		[Tooltip("Should the materials get cloned at the start?")]
		public bool CloneMaterials = true;

		[System.NonSerialized]
		private Renderer cachedRenderer;//缓存渲染器

		protected virtual void Awake()
		{
			if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();//读取Renderer

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