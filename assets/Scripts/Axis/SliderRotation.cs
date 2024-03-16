using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRotation : MonoBehaviour
{
	public delegate void SliderRotationDelegate();

	public Slider slider;

	private float rotY;
	
	//旋转的快慢
	public float SliderRoSpeed = 1f;
	
	//获得原先角度
	public Quaternion Original;

	private bool isPunEnabled;
	public bool IsPunEnabled
	{
		set => isPunEnabled = value;
	}
	void Start()
	{
		Original = transform.localRotation;
	}

	void Update()
	{
		PUNRota();
	}
	public void RoTa()
    {
		rotY = 360 * slider.value * SliderRoSpeed;
		transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + rotY, transform.rotation.z);
	}
	private void PUNRota()
    {
		if (isPunEnabled)
			OnPUNRota?.Invoke();
		else
			RoTa();
	}
	public event SliderRotationDelegate OnPUNRota;
}
