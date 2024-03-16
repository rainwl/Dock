using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace MRTK.Tutorials.GettingStarted
{
    public class PartAssemblyController : MonoBehaviour
    {
        public delegate void PartAssemblyControllerDelegate();//零件部件控制器委托

        #region 普通字段
        [SerializeField] private Transform locationToPlace = default;

        private const float MinDistance = 0.001f;//最近/远距离
        private const float MaxDistance = 0.1f;

        private bool shouldCheckPlacement;//应该检查位置

        private AudioSource audioSource;
        private ToolTipSpawner toolTipSpawner;
        private List<Collider> colliders;
        private List<PartAssemblyController> partAssemblyControllers;

        private Transform originalParent;//原本的父对象，位置，旋转角度
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private IEnumerator checkPlacementCoroutine;

        private bool hasAudioSource;
        private bool hasToolTip;

        private bool isPlaced;
        private bool isResetting;
        #endregion
        #region 同步字段
        private bool isPunEnabled;//是否支持PUN
        public bool IsPunEnabled
        {
            set => isPunEnabled = value;
        }
        #endregion

        private void Start()
        {
            // Check if object should check for placement。检查对象是否应检查位置
            if (locationToPlace != transform) shouldCheckPlacement = true;

            // Cache references 缓存引用
            audioSource = GetComponent<AudioSource>();
            toolTipSpawner = GetComponent<ToolTipSpawner>();

            colliders = new List<Collider>();
            if (shouldCheckPlacement)
                foreach (var col in GetComponents<Collider>())
                    colliders.Add(col);

            partAssemblyControllers = new List<PartAssemblyController>();
            foreach (var controller in FindObjectsOfType<PartAssemblyController>())
                partAssemblyControllers.Add(controller);

            var trans = transform;
            originalParent = trans.parent;
            originalPosition = trans.localPosition;
            originalRotation = trans.localRotation;

            checkPlacementCoroutine = CheckPlacement();

            // Check if object has audio source
            hasAudioSource = audioSource != null;

            // Check if object has tool tip
            hasToolTip = toolTipSpawner != null;

            // Start coroutine to continuously check if the object has been placed
            if (shouldCheckPlacement) StartCoroutine(checkPlacementCoroutine);
        }

        /// <summary>
        /// 触发放置功能
        /// Triggers the placement feature.
        /// 
        /// </summary>
        private void SetPlacement()
        {
            if (isPunEnabled)
                OnSetPlacement?.Invoke();
            else
                Set();
        }

        /// <summary>
        /// 将零件添加到部件并将零件放置在目标位置
        ///     Parents the part to the assembly and places the part at the target location.
        ///     
        /// </summary>
        public void Set()
        {
            // Update placement state
            isPlaced = true;

            // Play audio snapping sound
            if (hasAudioSource) audioSource.Play();

            // Disable ability to manipulate object
            foreach (var col in colliders) col.enabled = false;

            // Disable tool tips
            if (hasToolTip) toolTipSpawner.enabled = false;

            // Set parent and placement of object to target
            var trans = transform;
            trans.SetParent(locationToPlace.parent);
            trans.position = locationToPlace.position;
            trans.rotation = locationToPlace.rotation;
        }

        /// <summary>
        /// 触发重置放置特征。连上了unity。
        ///     Triggers the reset placement feature.
        ///     Hooked up in Unity.
        ///     
        /// </summary>
        public void ResetPlacement()
        {
            foreach (var controller in partAssemblyControllers)
                if (isPunEnabled)
                    controller.OnResetPlacement?.Invoke();
                else
                    controller.Reset();
        }

        /// <summary>
        /// 重置零件的父级和放置
        ///     Resets the part's parent and placement.
        /// </summary>
        public void Reset()
        {
            // Update placement state
            isPlaced = false;

            // Enable ability to manipulate object
            foreach (var col in colliders) col.enabled = true;

            // Enable tool tips
            if (hasToolTip) toolTipSpawner.enabled = true;

            // Reset parent and placement of object
            var trans = transform;
            trans.SetParent(originalParent);
            trans.localPosition = originalPosition;
            trans.localRotation = originalRotation;
        }

        /// <summary>
        /// 检查零件的位置，如果满足到目标的距离条件，则将其捕捉/保持到位。
        ///     Checks the part's position and snaps/keeps it in place if the distance to target conditions are met.
        /// </summary>
        private IEnumerator CheckPlacement()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);

                if (!isPlaced)
                {
                    if (Vector3.Distance(transform.position, locationToPlace.position) > MinDistance &&
                        Vector3.Distance(transform.position, locationToPlace.position) < MaxDistance)
                        SetPlacement();
                }
                else if (isPlaced)
                {
                    if (!(Vector3.Distance(transform.position, locationToPlace.position) > MinDistance)) continue;
                    var trans = transform;
                    trans.position = locationToPlace.position;
                    trans.rotation = locationToPlace.rotation;
                }
                else
                {
                    break;
                }
            }
        }


        //PUN事件委托调用
        /// <summary>
        /// 调用RestPlacement并启用PUN时引发。
        ///     Raised when RestPlacement is called and PUN is enabled.
        /// </summary>
        public event PartAssemblyControllerDelegate OnResetPlacement;

        /// <summary>
        /// 在调用SetPlacement并启用PUN时引发。
        ///     Raised when SetPlacement is called and PUN is enabled.
        /// </summary>
        public event PartAssemblyControllerDelegate OnSetPlacement;
    }
}
