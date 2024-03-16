using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour
{
    Transform m_tr, m_camera;
    bool isVisible = true;
    [SerializeField]
    float pivotScale = 0.5f;
    [SerializeField]
    GameObject body;
    // Use this for initialization
    void Start()
    {
        m_tr = transform;
        m_tr.localPosition = body.transform.localPosition;

        for (int i = 0; i < m_tr.childCount; i++)//修改该物体下的子物体的渲染队列
        {
            MeshRenderer m_meshRenderer = m_tr.GetChild(i).GetComponent<MeshRenderer>();
            if (m_meshRenderer == null) continue;
            if (m_meshRenderer.material == null) continue;
            m_meshRenderer.material.renderQueue += 2000;
        }

        body.transform.SetParent(m_tr);
        m_camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
            m_tr.localScale = pivotScale * (m_tr.position - m_camera.position).magnitude * Vector3.one;//放大缩小窗口坐标轴大小不变
    }
    private void OnBecameVisible()
    {
        isVisible = true;
    }
    private void OnBecameInvisible()
    {
        isVisible = false; print("AAA");
    }

}
