using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour
{
    public Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
    public Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
    public Transform _trans;// 目标物体的空间变换组件  
    public Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
    public Vector3 _vec3Offset;// 偏移 

    private GameObject skin;

    protected Rigidbody rigidBody;
    public string id = "";


    public void Start()
    {
        
    }
    
    
    public void Update()
    {
        
    }
}
