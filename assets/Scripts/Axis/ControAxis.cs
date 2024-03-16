using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dock
{
    public class ControAxis : MonoBehaviour
    {
        private static GameObject axispre;//承载预制体的对象
        public static GameObject Axispre
        {
            get => axispre;
            set => axispre = value;
        }

        private static GameObject target;//承载选中的目标对象
        public static GameObject Target
        {
            get => target;
            set => target = value;
        }

        public float axis_time = 0;//坐标轴进入的时间
        public float axis_deadtime = 3;//坐标轴消失的时间

        void Start()//实例化预制体成为移动对象的子物体，并默认为隐藏状态
        {
            //实例化预制体到场景，并将状态设置为false
            GameObject Axis = (GameObject)Resources.Load("AxisPre");
            axispre = Instantiate(Axis);            
            axispre.SetActive(false);
            //axispre.transform.localScale = target.transform.localScale * 1.5f;
        }

        void Update()//如果预制体不为空，那么时刻让他的位置和父物体保持一致，如果选择了对象，那么如果预制体不为空，那么激活对象，然后在几秒后隐藏对象
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("target")))
                {
                    //var target = hit.collider.gameObject;
                    target = hit.collider.gameObject;
                    string targetName = target.name;
                    print(targetName);//打印点击的可移动物体的名称

                    axispre.transform.position = target.transform.position;//将点击的可移动物体位置赋给AxisPre
                    
                    ActiveAxis();
                    //Debug.Log("激活了AxisPre");
                    Invoke(nameof(UnActiveAxis), axis_deadtime);
                    //Debug.Log("经过了deadtime秒，隐藏了AxisPre");
                    
                }
            }
            //如果axispre处于激活状态，就让他的位置和对象保持一致
            if(axispre.activeSelf != false)
            {
                axispre.transform.position = target.transform.position;
                //Debug.Log("执行了不断将axispre位置留在对象位置的语句");                
            }
            //如果axispre处于隐藏状态，那么target为空
            if(axispre.activeSelf == false)
            {
                target = null;
            }                 
        }       
        public void ActiveAxis()//如果预制体不为空，如果是隐藏状态，那么激活
        {
            
            if (axispre.activeSelf == false)
            {
                axispre.SetActive(true);
            }
        }
        public void UnActiveAxis()//如果预制体不为空，如果是激活状态，那么隐藏
        {           
            if (axispre.activeSelf == true)
            {
                axispre.SetActive(false);
            }
        }
        
    }
}
