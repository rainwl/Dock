using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlCube : BaseCube
{
   
    new void Update()
    {
        base.Update();
        
        
    }
    private void Awake()
    {
        _trans = transform;
    }
    IEnumerator OnMouseDown()

    {
        _vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(_trans.position);// 把目标物体的世界空间坐标转换到它自身的屏幕空间坐标   
        _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);// 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）   
        _vec3Offset = _trans.position - Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace);// 计算目标物体与鼠标物体在世界空间中的偏移量
        // 鼠标左键按下   
        while (Input.GetMouseButton(0))
        {
            _vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);// 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）
            _vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace) + _vec3Offset;// 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐标
            _trans.position = _vec3TargetWorldSpace;// 更新目标物体的世界空间坐标 
            // 等待固定更新   
            yield return new WaitForFixedUpdate();
        }
    }
    
}
