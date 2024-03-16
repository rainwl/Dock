using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliR2 : MonoBehaviour
{
    public GameObject target;
    public Quaternion initAngle;
    
    void Start()
    {
        initAngle = target.transform.rotation;
    }

    public void Rotate(Slider ss)
    {
        if (target != null)
        {
            float a = 360 * ss.value;
            target.transform.localEulerAngles = new Vector3(initAngle.x, initAngle.y + a, initAngle.z);
            Debug.Log("拖动滑柄进行旋转控制");
        }
    }
    
}
